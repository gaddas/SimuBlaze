using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    [Command(Command = "NAMEREG")]
    class dirNAMEREG : CommandBase
    {
        public override bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            if (lines.Length > 0)
            {
                if (lines[0].ToUpper() != "NAMEREG")
                {
                    error = "Невалидна инструкция";
                    return false;
                }
            }

            if (lines.Length > 1)
            {
                if (!lines[1].EndsWith(","))
                {
                    error = "Липсваща запетайка";
                    return true;
                }

                if (!cpu.ValidRegister(lines[1].Remove(lines[1].Length - 1)))
                {
                    error = "Първият аргумент на инструкцията трябва да е регистър";
                    return true;
                }
            }

            if (lines.Length == 3)
            {
                // NAMEREG sY, name
                var register = lines[1].Remove(lines[1].Length - 1).ToLower();
                var name = lines[2].ToLower();

                if (cpu.NamedRegisters.ContainsKey(name))
                {
                    cpu.NamedRegisters[name] = register; 
                }
                else
                {
                    cpu.NamedRegisters.Add(name, register);
                }

                error = null;
                return true;
            }

            error = "Невалидна инструкция";
            return false;
        }

        public override string Format(string[] lines, string comment, string label)
        {
            var command = string.Format("    {0} {1} {2}", lines[0].ToUpper(), lines[1], lines[2]);
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
            // Do nothing
        }
    }
}
