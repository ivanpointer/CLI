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
			CLIHandler.Command("Add", (arguments) =>
			{
				int number1 = Convert.ToInt32(arguments["Number1"].Value);
				int number2 = Convert.ToInt32(arguments["Number2"].Value);
				int solution = number1 + number2;

				Console.WriteLine(String.Format("{0} + {1} = {2}", number1, number2, solution));

				return CLIHandler.SUCCESS;
			});

			CLIHandler.Command("Subtract", (arguments) =>
			{
				int number1 = Convert.ToInt32(arguments["Number1"].Value);
				int number2 = Convert.ToInt32(arguments["Number2"].Value);
				int solution = number1 - number2;

				Console.WriteLine(String.Format("{0} - {1} = {2}", number1, number2, solution));

				return CLIHandler.SUCCESS;
			}, "Subtracts a /Number1 from a /Number2 and prints the solution");

			var exitCode = CLIHandler.HandleMain(args);

			Console.WriteLine(String.Format("{1}Command completed with exit code {0}{1}", exitCode, Environment.NewLine));
			Console.WriteLine("Press [Enter] to exit");
			Console.ReadLine();
			Environment.Exit(exitCode);
		}
	}

	/// <summary>
	/// A sample command for the CLI
	/// </summary>
	[Command]
	public class Multiply : ICLICommand
	{
		[Argument]
		public int Number1 { get; set; }
		[Argument]
		public int Number2 { get; set; }

		public int ExecuteCommand(Arguments arguments)
		{
			int product = Number1 * Number2;

			Console.WriteLine(String.Format("{0} * {1} = {2}", Number1, Number2, product));

			return CLIHandler.SUCCESS;
		}
	}

	/// <summary>
	/// A fuller command for the CLI
	/// </summary>
	[Command(Description = "Adds multiple values together", Command = "MAdd", HideFromUsage = false)]
	public class MultipleAdd : ICLICommand
	{
		[Argument(Description = "The first number that the others are added to", Name="N1", Required = true)]
		public int FirstNumber { get; set; }

		[Argument(Description = "The subsequent numbers to add", Name="NX")]
		public ICollection<string> OtherNumbers { get; set; }

		public int ExecuteCommand(Arguments arguments)
		{
			var runningTotal = FirstNumber;

			if (OtherNumbers != null)
				foreach (var nx in OtherNumbers)
					runningTotal += Convert.ToInt32(nx);

			if (arguments.ContainsKey("N3"))
				runningTotal += Convert.ToInt32(arguments["N3"].Value);

			Console.WriteLine(String.Format("Sum: {0}", runningTotal));

			return CLIHandler.SUCCESS;
		}
	}

}
