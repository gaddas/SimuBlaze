using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    [Command(Command = "ADDRESS")]
    class dirADDRESS : CommandBase
    {
        public override bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            if (lines.Length > 0)
            {
                if (lines[0].ToUpper() != "ADDRESS")
                {
                    error = "Невалидна инструкция";
                    return false;
                }
            }

            if (lines.Length == 2)
            {
                if (!cpu.ValidAddress(lines[1]))
                {
                    error = "Първият аргумент на инструкцията трябва да е адрес";
                    return true;
                }

                error = null;
                return true;
            }

            error = "Невалидна инструкция";
            return false;
        }

        public override string Format(string[] lines, string comment, string label)
        {
            var command = string.Format("    {0} {1}", lines[0].ToUpper(), lines[1]);
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

        public override void Execute(string[] lines, TheCpu cpu)
        {
            cpu.PC++;
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            var address = cpu.GetAddress(lines[1]);
            assembler.address = address;
        } 
    }
}
