/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/26/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

namespace PointerPlace.CLI
{
	/// <summary>
	/// An interface that defines the expected behavior of a CLI command
	/// </summary>
	public interface ICLICommand
	{
		/// <summary>
		/// Performs the action of the command
		/// </summary>
		/// <param name="arguments">The arguments parsed from the command line arguments</param>
		/// <returns>An int representing the exit code of the function</returns>
		int ExecuteCommand(Arguments arguments);
	}
}
