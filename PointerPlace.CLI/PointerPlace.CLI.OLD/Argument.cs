/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/26/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

using System.Collections.Generic;

namespace PointerPlace.CLI
{
    /// <summary>
    /// Represents a single argument from the command line
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// The name of the argument
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Whether or not the argument is a flag.
        /// The fact that the flag exists means that it is "set", or "true"
        /// </summary>
        public bool IsFlag { get; set; }
        /// <summary>
        /// The position of the argument within the arguments recieved from the command line
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The value of the argument.  This is null if there is more than one value assocaited with the argument.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// A collection of values for the argument.  This is null if there is only a single value.
        /// </summary>
        public ICollection<string> Values { get; set; }
        /// <summary>
        /// Returns true if "IsFlag" is set, or Value or Values has a value
        /// </summary>
        public bool IsSet
        {
            get
            {
                return IsFlag || Value != null || Values != null;
            }
        }

        /// <summary>
        /// Constructs a new argument with the given name
        /// </summary>
        /// <param name="name">The name of the argument being constructed</param>
        public Argument(string name)
        {
            Name = name;
            Values = null;
        }

        /// <summary>
        /// A static helper method which returns a bool indicating whether or not the given argument is set
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <returns>A bool indicating whether or not the given argument is set</returns>
        public static bool IsArgumentSet(Argument argument)
        {
            return argument != null && argument.IsSet;
        }
    }
}
