/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/26/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PointerPlace.CLI.Test
{
	/// <summary>
	/// Contains the unit tests for the CLI
	/// </summary>
	[TestClass]
	public class TestArguments
	{
		[TestMethod]
		public void TestSingleArgument()
		{
			var args = new string[]
			{
				"/Hello", "World!"
			};

			var arguments = Arguments.FromArgs(args);

			Assert.IsTrue(arguments.Count == 1);
			Assert.IsTrue(arguments.ContainsKey("Hello"));

			var helloArgument = arguments["Hello"];

			Assert.IsTrue(helloArgument.Value == "World!");
			Assert.IsFalse(helloArgument.IsFlag);
		}

		[TestMethod]
		public void TestSingleFlag()
		{
			var args = new string[] { "/HelloFlag" };

			var arguments = Arguments.FromArgs(args);

			Assert.IsTrue(arguments.Count == 1);
			Assert.IsTrue(arguments.ContainsKey("HelloFlag"));

			var helloFlag = arguments["HelloFlag"];

			Assert.IsTrue(helloFlag.IsFlag);
		}

		[TestMethod]
		public void TestTrailingFlag()
		{
			var args = new string[]
			{
				"/Hello", "World!", "/GoodbyeFlag"
			};

			var arguments = Arguments.FromArgs(args);

			Assert.IsTrue(arguments.Count == 2);
			Assert.IsTrue(arguments.ContainsKey("GoodbyeFlag"));

			var goodbyeFlag = arguments["GoodbyeFlag"];

			Assert.IsTrue(goodbyeFlag.IsFlag);
		}

		[TestMethod]
		public void TestConcurrentFlag()
		{
			var args = new string[]
			{
				"/Hello", "World!", "/ImAFlag", "/GoodbyeFlag"
			};

			var arguments = Arguments.FromArgs(args);

			Assert.IsTrue(arguments.Count == 3);
			Assert.IsTrue(arguments.ContainsKey("ImAFlag"));

			var imAFlag = arguments["ImAFlag"];

			Assert.IsTrue(imAFlag.IsFlag);
		}

		[TestMethod]
		public void TestTrailingArgument()
		{
			var args = new string[]
			{
				"/Hello", "World!", "/Goodbye", "Argument"
			};

			var arguments = Arguments.FromArgs(args);

			Assert.IsTrue(arguments.Count == 2);
			Assert.IsTrue(arguments.ContainsKey("Goodbye"));

			var goodbyeArgument = arguments["Goodbye"];

			Assert.IsTrue(goodbyeArgument.Value == "Argument");
			Assert.IsFalse(goodbyeArgument.IsFlag);
		}

		[TestMethod]
		public void TestOrphanedValue()
		{
			var args = new string[]
			{
				"/Hello", "World!", "Orphan!"
			};

			var arguments = Arguments.FromArgs(args);

			Assert.IsTrue(arguments.Count == 1);
			Assert.IsTrue(arguments.ContainsKey("Hello"));

			var goodbyeArgument = arguments["Hello"];

			Assert.IsTrue(goodbyeArgument.IsSet);
			Assert.IsTrue(goodbyeArgument.Values.Count == 2);
			Assert.IsTrue(goodbyeArgument.Values.Contains("World!"));
			Assert.IsTrue(goodbyeArgument.Values.Contains("Orphan!"));
		}
	}

}
