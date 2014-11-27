# CLI (Command Line Interface)
The CLI utility provides a quick and simple way to automatically parse arguments and wire up different commands for the command line process.  The CLI also defines a way to setup a clean and formatted "Usage" printout for the CLI.  This is best suited for "job" or "process" style applications, as the interface provided is quite simple.  However, this should address the majority of all console applications.

Two methods of defining commands are provided by the CLI utility; a simpler and more vage method using delegates, and a more difinitive and precise method.  I'll walk through using each with an example here, starting with the simpler method.

## Arguments
The CLI utility provides a couple classes `Argument` and `Arguments` which assist in parsing the `string[] args` arguments passed to a console application from the command line. `Argument` defines a single argument, and `Arguments` is a dictionary which houses the `Argument` instances.  These are used as an integral part of the CLI utility, so let's a deeper look into these objects.

### Argument Class
The `Argument` class represents a single argument on the command line.  Let's step through each of the properties of `Argument`:

![Argument Class](http://i.imgur.com/iKUd0QL.png)

1. **Name**: The name of the argument.  This is the name that will be used on the command line to identify the argument.
2. **IsFlag**: Indicates whether or not the argument is a flag.  The argument is deemed a flag if it is specified on the command line without any value.  Flag arguments are designed to be used as "switches" for a command line application.
3. **Index**: Indicates the position of the argument in the original arguments passed to the application.  The arguments are stored in a dictionary and therefore loose their original order after being parsed.  This is used to identify which of these arguments is instead the "command".
4. **Value**: The value of the argument, if the argument is not a flag and does not have multiple values.
5. **Values**: A collection of values for the argument, if more than one value is provided for the command on the command line.
6. **IsSet**: More of a "helper" property.  This is used to determine if any value is set to the argument.  This property is true if `IsFlag` is true, or `Value` or `Values` has any value; otherwise false.
7. **Argument (Default Constructor)**: The default constructor which expects the name of the argument as a parameter.
8. **IsArgumentSet**: Another "helper" method used to determine if a value is set for the argument.  This differs from `IsSet` in that a `null` argument can be passed in.  This simply protects against null arguments.

### Arguments Dictionary
The `Arguments` dictionary is an extention of `Dictionary<string, Argument>` which is used to parse and house the arguments from the command line.  There are only two main functionalities of the `Arguments` dictionary that are extensions over the standard dictionary behavior.  Let's take a look at the function signatures:

![Arguments Dictionary](http://i.imgur.com/Y0o9deK.png)

1. **FromArgs**: This function is used to create a new instance of `Arguments` from the given `string[] args` which are intended to be recieved from the command line in the `Main` function.  The `FromArgs` function allows the implementer to optionally supply an escape character and an ignore case setting.  `escapeChar` is the escape character that is used to signify the command and argument names.  The escape character is defaulted to `/`.  Command and argument names are prefixed with this value, whereas value arguments are not.  The `ignoreCase` attribute is used to control whether the command and argument names are case insensitive.  This defaults to true, indicating that the command and argument names are to be treated in a case insensitive manner.
2. **FormatArgumentName**: This function is used to format a given argument name into the format expected on the command line using the provided escape character.  The static version of this function allows you to specify the `escapeChar` used for the argument name.  The instance version of this function uses the escape character assigned to this instance of `Arguments`

## Using Delegate Functions
The first approach allows for a simpler, slimmer solution and is best suited for smaller console apps that are just used to do just a few small tasks.  Everything can be contained inside the `Main` method of `Program.cs`.  Let's get right down to it:

![Delegate Example](http://i.imgur.com/sf19znU.png)

1. The first thing to look it is where we add an "Add" command.  This is done by calling `CLI.Command(string commandName, Func<Arguments, int> command, string helpText = null)`.  `commandName` defines the name of the command, `command` defines the function body of the command, and the optional `helpText` is used for the usage printout as text to describe the command.  This first example shows this method being called without any help text.  This is the simplest possible implementation of a command in the CLI.
2. The second command is basically the same as the first, except that in this case, the help text is provided.
3. Lastly once we are done setting up the commands, we call `HandleMain` to have CLI handle the routing.  The full signature of the `HandleMain` function is: `CLI.HandleMain(string[] args, char escapeCharacter = DefaultEscapeChar, bool ignoreCase = true)`.  This example shows only the simples implementation: passing in the `string[] args`.  The escape character and ignore case arguments are optional:
  1. **escapeCharacter**: The character that is used to signify a command name.  The escape character comes before the command name.  The default escape character is `/`.
  2. **ignoreCase**: Ignore case indicates whether or not to use case insensitive searches on the command and argument names.  By default, ignore case is enabled, meaning command and argument names are not case sensitive.

## Using Classes and Attributes
Classes and attributes are a more advanced method of using the CLI utility.  It is best suited for a larger or more "public facing" console application and allows for more encapsulation and modularizaiton.  This approach also provides more methods for including more detailed information in the usage printout, making this more useful as a "shared" interface.  Let's start by taking a look at the simplest implementation of a class decorated with the attributes:

![Command Class Example](http://i.imgur.com/qXMv8GK.png)

1. The first step is to implement the `ICLICommand` interface and decorate your class with the `Command` attribute.  Without both, the CLI utility will ignore the class.
2. Next, you will need to define your arguments.  These are properties decorated with the `Argument` attribute.  The types of the attributes are restricted: either a `ICollection<string>` or primitive types, such as `string`, `int` or any other type directly convertable from a `string` are supported.
3. The last piece to do is to flesh out the body of the `ExecuteCommand(Arguments arguments)` function.  This function performs some action and returns an integer which is intended to be used as the exit code for the application.

