/*
 * (C)2014 Ivan Andrew Pointer (ivan@pointerplace.us)
 * Date: 11/24/2014
 * License: Apache License 2 (https://github.com/ivanpointer/Scheduler/blob/master/LICENSE)
 * GitHub: https://github.com/ivanpointer/Scheduler
 */

namespace PointerPlace.CLI
{
    public class Argument
    {
        public string Name { get; private set; }
        public bool IsFlag { get; set; }
        public int Index { get; set; }

        public string Value { get; set; }

        public Argument(string name)
        {
            Name = name;
        }
    }
}
