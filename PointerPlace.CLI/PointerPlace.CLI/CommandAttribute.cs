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
	/// An attribute identifying a class as a command class
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class CommandAttribute : Attribute
	{
		/// <summary>
		/// The description of the command, used when printing the usage of the program
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// The command name, if not set, the name of the class will be used
		/// </summary>
		public string Command { get; set; }
		/// <summary>
		/// A flag that if set, will hide the command from the usage when printed
		/// </summary>
		public bool HideFromUsage { get; set; }
	}
}
