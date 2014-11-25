using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointerPlace.CLI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string Description { get; set; }
        public string Command { get; set; }
        public bool HideFromUsage { get; set; }
    }
}
