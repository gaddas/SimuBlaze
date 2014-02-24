using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    class CommandBaseEmpty : CommandBase
    {
        protected string commandString = "1234567890";

        public override bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            if (string.Join(" ", lines).ToUpper() == commandString)
            {
                error = null;
                return true;
            }

            error = "Невалидна инструкция";
            return false;
        }

        public override string Format(string[] lines, string comment, string label)
        {
            var command = string.Format("    {0}", string.Join(" ", lines).ToUpper());
            if (command.Length < FORMAT_COMMAND_LENGTH)
            {
                command += new string(' ', FORMAT_COMMAND_LENGTH - command.Length);
            }
            
            if (string.IsNullOrEmpty(comment))
            {
                return label + command;
            }
            else
            {
                return label + command + comment;
            }
        }
    }
}
