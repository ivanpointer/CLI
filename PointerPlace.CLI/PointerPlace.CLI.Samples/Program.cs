/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/26/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

using System;
using System.Collections.Generic;

namespace PointerPlace.CLI.Samples
{
	/// <summary>
	/// A sample program using the CLI
	/// </summary>
	public class Program
	{
		public static void Main(string[] args)
		{
			CLI.Command("Hello", (arguments) =>
			{
				Console.WriteLine("Hello, World!");
				return CLI.SUCCESS;
			}, "My help text");

			var exitCode = CLI.HandleMain(args, '-');

			Console.WriteLine(String.Format("{1}Command completed with exit code {0}{1}", exitCode, Environment.NewLine));
			Console.WriteLine("Press [Enter] to exit");
			Console.ReadLine();
			Environment.Exit(exitCode);
		}

		
	}

	/// <summary>
	/// A sample command for the CLI
	/// </summary>
	[Command(Description="My custom description",Command="DoSomething")]
	public class MyCommand : ICLICommand
	{
		[Argument(Description="Something required",Required=true)]
		public string MyRequiredArgument { get; set; }

		[Argument(Description = "Something not")]
		public string MyNormalArgument { get; set; }

		public int ExecuteCommand(Arguments arguments)
		{
			Console.WriteLine("Ack!");
			return CLI.SUCCESS;
		}
	}

	/// <summary>
	/// A sample command for the CLI
	/// </summary>
	[Command]
	public class TargetValuesCommand : ICLICommand
	{
		[Argument]
		public ICollection<string> TestValues { get; set; }

		public int ExecuteCommand(Arguments arguments)
		{
			foreach (var testValue in TestValues)
				Console.WriteLine(testValue);

			return CLI.SUCCESS;
		}
	}

}
