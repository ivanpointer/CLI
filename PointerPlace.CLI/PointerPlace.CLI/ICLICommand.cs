using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointerPlace.CLI
{
    public interface ICLICommand
    {
        int ExecuteCommand(Arguments arguments);
    }
}
