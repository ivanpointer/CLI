/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/26/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PointerPlace.CLI
{
	/// <summary>
	/// A dictionary of arguments.  Provides helper functions for parsing out arguments from the string[] args parameter of the
	/// main function for a command line program.
	/// </summary>
	public class Arguments : Dictionary<string, Argument>
	{

		#region Constants and Members

		// The default escape character used for argument names
		private const char DefaultEscapeChar = '/';
		// The regex pattern used to parse out argument names
		private const string ArgumentPatternFormat = @"^\{0}(?<ArgumentName>[\w\d]+)$";
		
		// The compiled regex pattern for the arguments, this is built at construction using the provided/default escape character
		private Regex ArgumentPattern { get; set; }
		// The escape character used to build this set of arguments
		public char EscapeChar { get; private set; }

		#endregion

		/// <summary>
		/// Builds the arguments dictionary, using the provided escape character for argument names, which defaults to '/'
		/// </summary>
		/// <param name="escapeChar">The escape character to use for argument names; defaults to '/'</param>
		private Arguments(char escapeChar = DefaultEscapeChar, bool ignoreCase = true)
			: base(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal)
		{
			ArgumentPattern = new Regex(String.Format(ArgumentPatternFormat, escapeChar, RegexOptions.IgnoreCase | RegexOptions.Multiline));
		}

		/// <summary>
		/// Builds an instance of this Arguments dictionary using the provided arguments (from the command line) and escape character, which defaults to '/'
		/// </summary>
		/// <param name="args">The string[] args from the Main function of the command line program</param>
		/// <param name="escapeChar">The escape character to use for argument names; defaults to '/'</param>
		/// <param name="ignoreCase">Whether or not commands and arguments are to be treated case insensitive.  Defaults to true.</param>
		/// <returns>The built Arguments dictionary, from the provided args, using the provided escape character</returns>
		public static Arguments FromArgs(string[] args, char escapeChar = DefaultEscapeChar, bool ignoreCase = true)
		{
			// Get a new instance of our arguments dictionary, providing the escape character
			var arguments = new Arguments(escapeChar, ignoreCase);

			// Check to see if we have any arguments at all
			if (args != null && args.Length > 0)
			{
				string argumentName;
				Argument argument = null;
				string argumentString;
				Match match;

				// Iterate over each of the arguments
				for (int index = 0; index < args.Length; index++)
				{
					// Check to see if the argument is an argument name
					argumentString = args[index];
					if (arguments.ArgumentPattern.IsMatch(argumentString))
					{
						// Check to see if the previous argument exists, and if it doesn't
						//  have a value set, mark it as a flag
						if (argument != null && Argument.IsArgumentSet(argument) == false)
							argument.IsFlag = true;

						// Get a hold of the match for the argument name
						//  Pull out the argument name from the argument
						match = arguments.ArgumentPattern.Match(argumentString);
						argumentName = match.Groups["ArgumentName"].Value;

						// Build the new argument, assign its name and index
						//  and add it to the dictionary
						argument = new Argument(argumentName)
						{
							Index = index
						};
						arguments[argumentName] = argument;
					}
					// If the argument is an argument value
					else
					{
						// Check to see if we have already initialized an argument
						if (argument != null)
						{
							// If the argument already has a value,
							//  Convert it to "values"
							if (argument.IsSet)
							{
								if (argument.Values == null)
								{
									argument.Values = new List<string>();
									argument.Values.Add(argument.Value);
									argument.Value = null;
								}

								argument.Values.Add(argumentString);
							}
							// If we haven't yet assigned a value,
							//  set it
							else
							{
								argument.Value = argumentString;
							}
						}
						else
						{
							// We have an argument value without an argument name
							throw new ArgumentException("Argument value found without argument name");
						}
					}
				}

				// If we have an argument, without a value
				//  mark it as a flag
				if (argument.IsSet == false)
					argument.IsFlag = true;
			}

			// Return the built argument dictionary
			return arguments;
		}

		/// <summary>
		/// Formats the given argument name with the given escape character, which defaults to '/'
		/// </summary>
		/// <param name="argumentName">The argument name to format</param>
		/// <param name="escapeChar">The escape character for the argument</param>
		/// <returns>The formatted argument name</returns>
		public static string FormatArgumentName(string argumentName, char escapeChar = DefaultEscapeChar)
		{
			return String.Format("{0}{1}", escapeChar, argumentName);
		}

		/// <summary>
		/// Formats the argument name using the escape character assigned to this arguments dictionary
		/// </summary>
		/// <param name="argumentName">The argument name to format</param>
		/// <returns>The formatted argument name</returns>
		public string FormatArgumentName(string argumentName)
		{
			return FormatArgumentName(argumentName, EscapeChar);
		}

		/// <summary>
		/// Checks this arguments dictionary for the flag, returning true if the flag is set
		/// </summary>
		/// <param name="flagName">The name of the flag to check for</param>
		/// <returns>A bool indicating whether or not the flag is set</returns>
		public bool CheckFlag(string flagName)
		{
			var argument = this[flagName];
			if (argument != null)
				return argument.IsFlag;

			return false;
		}

		/// <summary>
		/// Overriden index operator to make this null safe
		/// </summary>
		/// <param name="key">The key of the argument to retrieve</param>
		/// <returns>The argument identified by key, or null if not found</returns>
		new public Argument this[string key]
		{
			get
			{
				return this.ContainsKey(key)
					? base[key]
					: null;
			}
			set
			{
				base[key] = value;
			}
		}

	}
}