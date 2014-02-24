namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class CommandAttribute : Attribute
    {
        public string Command { get; set; }
    }
}
