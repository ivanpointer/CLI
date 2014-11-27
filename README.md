# CLI (Command Line Interface)
The CLI utility provides a quick and simple way to automatically parse arguments and wire up different commands for the command line process.  The CLI also defines a way to setup a clean and formatted "Usage" printout for the CLI.  This is best suited for "job" or "process" style applications, as the interface provided is quite simple.  However, this should address the majority of all console applications.

Two methods of defining commands are provided by the CLI utility; a simpler and more vage method using delegates, and a more difinitive and precise method.  I'll walk through using each with an example here, starting with the simpler method.

# Using Delegate Functions
Let's get right down to it:

![Delegate Example](http://i.imgur.com/NNEIm1s.png)

1. The first thing to look it is where we add an "Add" command.  This is done by calling `CLI.Command(string commandName, Func<Arguments, int> command, string helpText = null)`.  `commandName` defines the name of the command, `command` defines the function body of the command, and the optional `helpText` is used for the usage printout as text to describe the command.
2. X
3. X
