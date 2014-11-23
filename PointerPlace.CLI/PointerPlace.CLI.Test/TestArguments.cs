using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace PointerPlace.CLI.Test
{
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

            try
            {
                var arguments = Arguments.FromArgs(args);
            }
            catch (Exception cause)
            {
                Assert.IsTrue(cause is ArgumentException);
            }
        }
    }

}
