/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/26/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

using System;

namespace PointerPlace.CLI
{
    /// <summary>
    /// An attribute which defines a property of a class as an argument for a command
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ArgumentAttribute : Attribute
    {
        /// <summary>
        /// The name of the attribute.  This is optional and is used to override the name of the property in the command class.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// An optional description for the property which is displayed to the end user in the usage printout
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A boolean flag indicating whether or not the argument is required
        /// </summary>
        public bool Required { get; set; }
    }
}
