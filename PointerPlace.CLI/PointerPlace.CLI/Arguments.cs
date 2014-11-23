/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/24/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PointerPlace.CLI
{
	public class Arguments : Dictionary<string, Argument>
	{
		private static readonly Regex SingleArgumentPattern = new Regex(@"^/(?<ArgumentName>[\w\d]+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
		private static readonly Regex FullArgumentPattern = new Regex(@"^/(?<ArgumentName>[\w\d]+)=(?<ArgumentValue>[.]+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		public static Arguments FromArgs(string[] args)
		{
			var arguments = new Arguments();

			if (args != null && args.Length > 0)
			{
				Argument nextArgument;
				string argumentName;
				string argumentValue;
				Argument previousArgument = null;
				bool previousValueSet = false;
				string argument;
				Match match;

				for (int index = 0; index < args.Length; index++)
				{
					argument = args[index];
					if (FullArgumentPattern.IsMatch(argument))
					{
						match = FullArgumentPattern.Match(argument);
						argumentName = match.Groups["ArgumentName"].Value;
						argumentValue = match.Groups["ArgumentValue"].Value;

						nextArgument = new Argument(argumentName);
						nextArgument.Value = argumentValue;
						arguments[argumentName] = nextArgument;

						previousArgument = nextArgument;
						previousValueSet = true;
					}
					else if (SingleArgumentPattern.IsMatch(argument))
					{
						if (previousArgument != null && previousValueSet == false)
							previousArgument.IsFlag = true;

						match = SingleArgumentPattern.Match(argument);
						argumentName = match.Groups["ArgumentName"].Value;

						nextArgument = new Argument(argumentName);
						arguments[argumentName] = nextArgument;

						previousArgument = nextArgument;
						previousValueSet = false;
					}
					else
					{
						argumentValue = argument;

						if (previousArgument != null && previousValueSet == false)
						{
							previousArgument.Value = argument;
							previousValueSet = true;
						}
						else
						{
							throw new ArgumentException("Argument value found without argument name");
						}
					}
				}

				if (previousValueSet == false)
					previousArgument.IsFlag = true;
			}

			return arguments;
		}

	}
}