using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PointerPlace.CLI
{
    public static class CLI
    {

        #region Internal Classes/Structs

        internal struct PrintColors
        {
            public ConsoleColor Fore, Back;
        }

        internal class CommandInfo
        {
            public Type Type { get; set; }
            public CommandAttribute Attribute { get; set; }
            public IEnumerable<ArgumentInfo> Arguments { get; set; }
        }

        internal struct ArgumentInfo
        {
            public PropertyInfo PropertyInfo;
            public ArgumentAttribute Attribute;
        }

        #endregion

        #region Constants

        private static readonly PrintColors DefaultColors = new PrintColors
        {
            Back = ConsoleColor.Black,
            Fore = ConsoleColor.Gray
        };

        private static readonly PrintColors CommandColors = new PrintColors
        {
            Back = ConsoleColor.Black,
            Fore = ConsoleColor.Yellow
        };

        private static readonly PrintColors RequiredArgumentColors = new PrintColors
        {
            Back = ConsoleColor.Black,
            Fore = ConsoleColor.Cyan
        };

        private static readonly PrintColors OptionalArgumentColors = new PrintColors
        {
            Back = ConsoleColor.Black,
            Fore = ConsoleColor.Gray
        };

        private static readonly PrintColors CommandDescriptionColors = new PrintColors
        {
            Back = ConsoleColor.Black,
            Fore = ConsoleColor.DarkGray
        };

        private static readonly string UsageMessage = Environment.NewLine + " Usage:" + Environment.NewLine;
        private const string CommandPattern = "  {0}";
        private const string RequiredArgumentPattern = " {0} \"{1}\"";
        private const string OptionalArgumentPattern = " [{0} \"{1}\"]";
        private static readonly string CommandDescriptionPattern = Environment.NewLine + "    - {0}";
        private const string ArgumentDescriptionPattern = "     * {0}: {1} - {2}";
        private const string RequiredModifier = "Required";

        private static readonly Type CommandType = typeof(ICLICommand);
        
        public const int SUCCESS = 0;
        public const int COMMAND_NOT_FOUND = 1;

        #endregion

        private static IEnumerable<CommandInfo> GetCommands(Assembly assembly)
        {
            CommandAttribute commandAttrib;
            CommandInfo command;
            foreach (var type in assembly.GetTypes())
            {
                commandAttrib = type.GetCustomAttributes<CommandAttribute>().FirstOrDefault();
                if (commandAttrib != null)
                {
                    command = new CommandInfo
                    {
                        Type = type,
                        Attribute = commandAttrib,
                        Arguments = GetArguments(type)
                    };

                    yield return command;
                }
            }
        }

        private static IEnumerable<ArgumentInfo> GetArguments(Type commandType)
        {
            ArgumentAttribute argumentAttrib;
            foreach (var property in commandType.GetProperties())
            {
                argumentAttrib = property.GetCustomAttributes<ArgumentAttribute>().FirstOrDefault();
                if (argumentAttrib != null)
                    yield return new ArgumentInfo
                    {
                        PropertyInfo = property,
                        Attribute = argumentAttrib
                    };
            }
        }

        private static ICLICommand GetCommand(Arguments arguments)
        {
            if (arguments.Count > 0)
            {
                var argument = arguments.Values.FirstOrDefault(_ => _.Index == 0);
                var commandInfo = GetCommands(Assembly.GetCallingAssembly()).FirstOrDefault(_ => _.Attribute.Command == argument.Name);
                commandInfo = commandInfo != null
                    ? commandInfo
                    : GetCommands(Assembly.GetCallingAssembly()).FirstOrDefault(_ => _.Type.Name == argument.Name);

                if (commandInfo != null)
                {
                    var constructorInfo = commandInfo.Type.GetConstructor(new Type[] { });
                    return (ICLICommand)constructorInfo.Invoke(null);
                }
            }

            return null;
        }

        public static void PrintUsage(Assembly assembly)
        {
            WriteLine(UsageMessage);

            var commands = GetCommands(assembly);

            foreach (var command in commands)
                if (!command.Attribute.HideFromUsage && CommandType.IsAssignableFrom(command.Type))
                    PrintCommand(command);
        }

        public static void PrintUsage()
        {
            PrintUsage(Assembly.GetCallingAssembly());
        }

        public static int HandleMain(string[] args)
        {
            var arguments = Arguments.FromArgs(args);
            var command = GetCommand(arguments);

            if (command != null)
            {
                return command.ExecuteCommand(arguments);
            }
            else
            {
                PrintUsage(Assembly.GetCallingAssembly());
            }

            return COMMAND_NOT_FOUND;
        }

        #region Printing

        private static void PrintCommand(CommandInfo command)
        {
            string commandName = String.IsNullOrEmpty(command.Attribute.Command)
                ? command.Type.Name
                : command.Attribute.Command;

            commandName = Arguments.FormatArgumentName(commandName);

            commandName = String.Format(CommandPattern, commandName);

            Write(commandName, CommandColors);

            var arguments = GetArguments(command.Type);

            foreach (var argument in arguments)
                PrintArgumentInline(argument);

            if (!String.IsNullOrEmpty(command.Attribute.Description))
                WriteLine(String.Format(CommandDescriptionPattern, command.Attribute.Description), CommandDescriptionColors);

            foreach (var argument in arguments)
                PrintArgumentDescription(argument);

            WriteLine(String.Empty);
        }

        private static void PrintArgumentInline(ArgumentInfo argument)
        {
            string argumentName = String.IsNullOrEmpty(argument.Attribute.Name)
                ? argument.PropertyInfo.Name
                : argument.Attribute.Name;

            argumentName = Arguments.FormatArgumentName(argumentName);

            string shortDescription = String.IsNullOrEmpty(argument.Attribute.ShortDescription)
                ? argument.PropertyInfo.Name
                : argument.Attribute.ShortDescription;

            string pattern = argument.Attribute.Required
                ? RequiredArgumentPattern
                : OptionalArgumentPattern;

            string formattedArgument = String.Format(pattern, argumentName, shortDescription);

            PrintColors colors = argument.Attribute.Required
                ? RequiredArgumentColors
                : OptionalArgumentColors;

            Write(formattedArgument, colors);
        }

        private static void PrintArgumentDescription(ArgumentInfo argument)
        {
            string argumentName = String.IsNullOrEmpty(argument.Attribute.Name)
                ? argument.PropertyInfo.Name
                : argument.Attribute.Name;

            string shortDescription = String.IsNullOrEmpty(argument.Attribute.ShortDescription)
                ? argumentName
                : argument.Attribute.ShortDescription;

            string description = String.IsNullOrEmpty(argument.Attribute.LongDescription)
                ? shortDescription
                : argument.Attribute.LongDescription;

            var modifierList = new List<string>();

            modifierList.Add(argument.PropertyInfo.PropertyType.Name);

            if(argument.Attribute.Required)
                modifierList.Add(RequiredModifier);

            var modifiers = String.Join(",", modifierList);

            string descriptionString = String.Format(ArgumentDescriptionPattern, argumentName, modifiers, description);

            WriteLine(descriptionString, CommandDescriptionColors);
        }

        private static void Write(string message, PrintColors? colors = null)
        {
            if (!colors.HasValue)
                colors = DefaultColors;

            var backup = new PrintColors
            {
                Back = Console.BackgroundColor,
                Fore = Console.ForegroundColor
            };

            try
            {
                Console.BackgroundColor = colors.Value.Back;
                Console.ForegroundColor = colors.Value.Fore;

                Console.Write(message);
            }
            finally
            {
                Console.BackgroundColor = backup.Back;
                Console.ForegroundColor = backup.Fore;
            }
        }

        private static void WriteLine(string message, PrintColors? colors = null)
        {
            if (!colors.HasValue)
                colors = DefaultColors;

            var backup = new PrintColors
            {
                Back = Console.BackgroundColor,
                Fore = Console.ForegroundColor
            };

            try
            {
                Console.BackgroundColor = colors.Value.Back;
                Console.ForegroundColor = colors.Value.Fore;

                Console.WriteLine(message);
            }
            finally
            {
                Console.BackgroundColor = backup.Back;
                Console.ForegroundColor = backup.Fore;
            }
        }

        #endregion

    }
}
