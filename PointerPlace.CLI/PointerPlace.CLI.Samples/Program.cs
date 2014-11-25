using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PointerPlace.CLI.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CLI.HandleMain(args);

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }

        
    }

    [Command(Description="My custom description")]
    public class MyCommand : ICLICommand
    {
        [Argument(ShortDescription="Something required",Required=true)]
        public string MyRequiredArgument { get; set; }

        [Argument(ShortDescription="Something not")]
        public string MyNormalArgument { get; set; }

        public int ExecuteCommand(Arguments arguments)
        {
            Console.WriteLine("Ack!");
            return CLI.SUCCESS;
        }
    }

}
