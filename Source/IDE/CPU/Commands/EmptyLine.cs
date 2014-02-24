using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    [Command(Command = "")]
    internal class EmptyLine : CommandBase
    {
        public override bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            if (lines.Length == 0)
            {
                error = null;
                return true;
            }

            error = "Невалидна инструкция";
            return false;
        }

        public override string Format(string[] lines, string comment, string label)
        {
            var command = new string(' ', FORMAT_COMMAND_LENGTH);

            if (string.IsNullOrEmpty(comment))
            {
                return label + command;
            }
            else
            {
                return label + command + comment;
            }
        }

        public override void Execute(string[] lines, TheCpu cpu)
        {
            cpu.PC++;
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            // Do nothing
        }
    }
}
