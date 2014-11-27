/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/26/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PointerPlace.CLI
{
	/// <summary>
	/// Provides command line interface functionality to a command
	/// line application
	/// </summary>
	public static class CLI
	{

		#region Internal Classes/Structs

		// A struct which is used to store colors for printing to the console
		internal struct PrintColors
		{
			public ConsoleColor Fore, Back;
		}

		// A struct for storing information about a manual command
		internal struct ManualCommand
		{
			public string Name;
			public Func<Arguments, int> Command;
			public string HelpText;
		}

		// A class for storing information about a command
		internal class CommandInfo
		{
			public Type Type { get; set; }
			public CommandAttribute Attribute { get; set; }
			public IEnumerable<ArgumentInfo> Arguments { get; set; }
		}

		// A structe used to store information about a particular argument
		internal struct ArgumentInfo
		{
			public PropertyInfo PropertyInfo;
			public ArgumentAttribute Attribute;
		}

		#endregion

		#region Constants

		// Exception messages
		private const string ArgumentRequiredMessage = "Argument \"{0}\" is required and not found";

		// Default colors
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

		// Other defaults and printing formats
		private const char DefaultEscapeChar = '/';
		private static readonly string UsageMessage = Environment.NewLine + " Usage:" + Environment.NewLine;
		private const string CommandPattern = "  {0}";
		private const string RequiredArgumentPattern = " {0} \"{1}\"";
		private const string OptionalArgumentPattern = " [{0} \"{1}\"]";
		private static readonly string CommandDescriptionPattern = Environment.NewLine + "    - {0}";
		private const string ArgumentDescriptionPattern = "     * {0}: {1} - {2}";
		private const string RequiredModifier = "Required";

		// The command type to save on reflection
		private static readonly Type CommandType = typeof(ICLICommand);

		// Standard exit codes
		public const int SUCCESS = 0;
		public const int COMMAND_NOT_FOUND = 1;

		#endregion

		#region Members

		// A local inmstance of the manual commands built for this CLI.  Static at a thread level.
		private static readonly ThreadLocal<IDictionary<string, ManualCommand>> ManualCommands = new ThreadLocal<IDictionary<string, ManualCommand>>(() =>
		{
			return new Dictionary<string, ManualCommand>();
		});

		#endregion

		#region Public Members

		/// <summary>
		/// The function for handling "Main".  This is the function to call
		/// for using the CLI to handle the program.
		/// </summary>
		/// <param name="args">The arguments passed to the "Main" function of a program</param>
		/// <param name="escapeCharacter">The escape character used to identify the argument names in the arguments</param>
		/// <returns>An int which is the exit code</returns>
		public static int HandleMain(string[] args, char escapeCharacter = DefaultEscapeChar)
		{
			// Parse out the arguments
			var arguments = Arguments.FromArgs(args, escapeCharacter);

			// Try manual commands first
			var commandResult = ExecuteManualCommand(arguments);

			// If we didn't execute a manual command
			//  try the attribute commands next
			if (commandResult.HasValue == false)
				commandResult = ExecuteCommand(arguments, Assembly.GetCallingAssembly());

			// If a command was executed, return its exit code
			if (commandResult.HasValue)
			{
				return commandResult.Value;
			}
			// We didn't find a command for the argument name
			//  Print the usage and return the exit code indicating
			//  that the command wasn't found
			else
			{
				PrintUsage(Assembly.GetCallingAssembly(), escapeCharacter);
				return COMMAND_NOT_FOUND;
			}
		}

		/// <summary>
		/// Adds a manual command to the CLI
		/// </summary>
		/// <param name="commandName">The name of the command</param>
		/// <param name="command">The function body of the command</param>
		/// <param name="helpText">Help text associated with the command.  Will be printed with the usage of the program</param>
		public static void Command(string commandName, Func<Arguments, int> command, string helpText = null)
		{
			ManualCommands.Value[commandName] = new ManualCommand
			{
				Name = commandName,
				Command = command,
				HelpText = helpText
			};
		}

		/// <summary>
		/// Prints the usage of the commands found in the CLI.  Searches
		/// for both manual commands, and commands identified by
		/// attributes
		/// </summary>
		/// <param name="assembly">The assembly to search for commands</param>
		/// <param name="escapeCharacter">The escape character to use for the argument names; defaults to '/'</param>
		public static void PrintUsage(Assembly assembly, char escapeCharacter = DefaultEscapeChar)
		{
			WriteLine(UsageMessage);

			var commands = GetCommands(assembly);

			foreach (var command in commands)
				if (!command.Attribute.HideFromUsage && CommandType.IsAssignableFrom(command.Type))
					PrintCommand(command, escapeCharacter);

			if (ManualCommands.IsValueCreated && ManualCommands.Value.Count > 0)
				foreach (var manualCommand in ManualCommands.Value.Values)
					PrintCommand(manualCommand, escapeCharacter);
		}

		/// <summary>
		/// Prints the usage of the commands found inthe CLI.  Searches for both
		/// manual commands and commands identified by attributes.  Searches the
		/// calling command for the arguments.
		/// </summary>
		/// <param name="escapeCharacter"></param>
		public static void PrintUsage(char escapeCharacter = DefaultEscapeChar)
		{
			PrintUsage(Assembly.GetCallingAssembly(), escapeCharacter);
		}

		#endregion

		#region Printing

		// Prints the given manual command, with the given escape character;p
		private static void PrintCommand(ManualCommand command, char escapeCharacter = DefaultEscapeChar)
		{
			string commandName = Arguments.FormatArgumentName(command.Name, escapeCharacter);
			commandName = String.Format(CommandPattern, commandName);
			Write(commandName, CommandColors);
			
			if(!String.IsNullOrEmpty(command.HelpText))
				WriteLine(String.Format(CommandDescriptionPattern, command.HelpText), CommandDescriptionColors);

			WriteLine(String.Empty);
		}

		// Prints the given command, with the given escape character;p
		private static void PrintCommand(CommandInfo command, char escapeCharacter = DefaultEscapeChar)
		{
			string commandName = String.IsNullOrEmpty(command.Attribute.Command)
				? command.Type.Name
				: command.Attribute.Command;

			commandName = Arguments.FormatArgumentName(commandName, escapeCharacter);

			commandName = String.Format(CommandPattern, commandName);

			Write(commandName, CommandColors);

			var arguments = GetArguments(command.Type);

			foreach (var argument in arguments)
				PrintArgumentInline(argument, escapeCharacter);

			if (!String.IsNullOrEmpty(command.Attribute.Description))
				WriteLine(String.Format(CommandDescriptionPattern, command.Attribute.Description), CommandDescriptionColors);
			else
				WriteLine(String.Empty);

			foreach (var argument in arguments)
				PrintArgumentDescription(argument);

			WriteLine(String.Empty);
		}

		// Prints the given argument inline, this is purposed to be on the same line as the command
		//  Uses the given escape character, defaulting to '/'
		private static void PrintArgumentInline(ArgumentInfo argument, char escapeCharacter = DefaultEscapeChar)
		{
			string argumentName = String.IsNullOrEmpty(argument.Attribute.Name)
				? argument.PropertyInfo.Name
				: argument.Attribute.Name;

			argumentName = Arguments.FormatArgumentName(argumentName, escapeCharacter);

			string description = String.IsNullOrEmpty(argument.Attribute.Description)
				? argument.PropertyInfo.Name
				: argument.Attribute.Description;

			string pattern = argument.Attribute.Required
				? RequiredArgumentPattern
				: OptionalArgumentPattern;

			string formattedArgument = String.Format(pattern, argumentName, description);

			PrintColors colors = argument.Attribute.Required
				? RequiredArgumentColors
				: OptionalArgumentColors;

			Write(formattedArgument, colors);
		}

		// Prints the description for the argument
		private static void PrintArgumentDescription(ArgumentInfo argument)
		{
			string argumentName = String.IsNullOrEmpty(argument.Attribute.Name)
				? argument.PropertyInfo.Name
				: argument.Attribute.Name;

			string description = String.IsNullOrEmpty(argument.Attribute.Description)
				? argumentName
				: argument.Attribute.Description;

			var modifierList = new List<string>();

			modifierList.Add(argument.PropertyInfo.PropertyType.Name);

			if(argument.Attribute.Required)
				modifierList.Add(RequiredModifier);

			var modifiers = String.Join(",", modifierList);

			string descriptionString = String.Format(ArgumentDescriptionPattern, argumentName, modifiers, description);

			WriteLine(descriptionString, CommandDescriptionColors);
		}

		// A "helper" method which writes the given message inline with the given colors
		//  Backs the current colors up before writing, then restores
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

		// A "helper" method which writes the given message and ends the line, with the given colors
		//  Backs the current colors up before writing, then restores
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

		#region Helpers

		// Returns a list of standard commands found within the provided assembly
		private static IEnumerable<CommandInfo> GetCommands(Assembly assembly)
		{
			CommandAttribute commandAttrib;
			CommandInfo command;
			foreach (var type in assembly.GetTypes())
			{
				commandAttrib = type.GetCustomAttributes<CommandAttribute>().FirstOrDefault();
				if (commandAttrib != null)
				{

					if (String.IsNullOrEmpty(commandAttrib.Command))
						commandAttrib.Command = type.Name;

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

		// Returns a list of commands found within the provided type, presumably a class decorated with the command attribute
		private static IEnumerable<ArgumentInfo> GetArguments(Type commandType)
		{
			ArgumentAttribute argumentAttrib;
			foreach (var property in commandType.GetProperties())
			{
				argumentAttrib = property.GetCustomAttributes<ArgumentAttribute>().FirstOrDefault();
				if (argumentAttrib != null)

					if (String.IsNullOrEmpty(argumentAttrib.Name))
						argumentAttrib.Name = property.Name;

				yield return new ArgumentInfo
				{
					PropertyInfo = property,
					Attribute = argumentAttrib
				};
			}
		}

		// Executes a manual command using the given arguments
		//  Returns null if a valid manual command for the given arguments is not found
		private static int? ExecuteManualCommand(Arguments arguments)
		{
			if (arguments.Count > 0 && ManualCommands.IsValueCreated && ManualCommands.Value.Count > 0)
			{
				// Get a hold of the key for the manual command
				var argument = arguments.Values.First(_ => _.Index == 0);
				var argumentName = argument.Name.ToLower();
				var commandKey = ManualCommands.Value.Keys.FirstOrDefault(_ => _.ToLower() == argumentName);

				// If the key for a manual command is found, execute the associated manual command
				if (String.IsNullOrEmpty(commandKey) == false)
				{
					var manualCommand = ManualCommands.Value[commandKey];
					return manualCommand.Command(arguments);
				}
			}

			// If we got here, it means that we didn't find a manual
			//  command
			return null;
		}

		// Executes a command using the given arguments, using the given assembly to find the command
		//  Returns null if a valid command for the given arguments is not found in the given assembly
		private static int? ExecuteCommand(Arguments arguments, Assembly assembly)
		{
			if (arguments.Count > 0)
			{
				// Tries to find a command using the provided arguments
				//  within the provided assembly
				var argument = arguments.Values.FirstOrDefault(_ => _.Index == 0);
				var argumentName = argument.Name.ToLower();
				var commandInfo = GetCommands(assembly).FirstOrDefault(_ => _.Attribute.Command != null && _.Attribute.Command.ToLower() == argumentName);
				commandInfo = commandInfo != null
					? commandInfo
					: GetCommands(Assembly.GetCallingAssembly()).FirstOrDefault(_ => _.Type.Name.ToLower() == argumentName);

				// If we found a command, process it
				if (commandInfo != null)
				{
					// Get the constructor for the command and
					//  get an instance of the command
					var constructorInfo = commandInfo.Type.GetConstructor(new Type[] { });
					var command = (ICLICommand)constructorInfo.Invoke(null);

					// Go over the command, populating
					//  the arguments using DI
					Argument commandArgument;
					string commandArgumentName;
					string argumentLower;
					foreach (var argumentInfo in commandInfo.Arguments)
					{
						// Figure out the argument name and get
						//  a hold of the argument
						commandArgumentName = String.IsNullOrEmpty(argumentInfo.Attribute.Name)
							? argumentInfo.PropertyInfo.Name
							: argumentInfo.Attribute.Name;
						argumentLower = commandArgumentName.ToLower();
						commandArgument = arguments.Values.FirstOrDefault(_ => _.Name.ToLower() == argumentLower);

						// If we got a hold of the argument
						//  Assign it
						if (commandArgument != null)
						{
							if (commandArgument.Value != null)
							{
								argumentInfo.PropertyInfo.GetSetMethod().Invoke(command, new object[] { Convert.ChangeType(commandArgument.Value, argumentInfo.PropertyInfo.PropertyType) });
							}
							else
							{
								argumentInfo.PropertyInfo.GetSetMethod().Invoke(command, new object[] { commandArgument.Values });
							}
						}
						else
						{
							// If the argument was not found, and is required
							//  throw an exception indicating such
							if (argumentInfo.Attribute.Required)
								throw new ArgumentException(String.Format(ArgumentRequiredMessage, commandArgumentName));
						}
					}

					// Execute the command and return the result
					return command.ExecuteCommand(arguments);
				}
			}

			// The command wasn't found,
			//  return nothing
			return null;
		}

		#endregion

	}
}
