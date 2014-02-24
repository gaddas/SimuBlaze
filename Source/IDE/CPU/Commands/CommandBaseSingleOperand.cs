using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    class CommandBaseSingleOperand : CommandBase
    {
        protected string commandString = "1234567890";

        public override bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            if (lines.Length > 0)
            {
                if (lines[0].ToUpper() != commandString)
                {
                    error = "Невалидна инструкция";
                    return false;
                }
            }

            if (lines.Length == 2)
            {
                if (cpu.ValidRegister(lines[1]))
                {
                    // CMD sX
                    error = null;
                    return true;
                }
                else
                {
                    error = "Първият аргумент на инструкцията трябва да е регистър";
                    return true;
                }
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
            // CMD sX
            this.ExecuteInternal(lines[1], cpu);
        }

        protected virtual void ExecuteInternal(string register, TheCpu cpu)
        {
            throw new NotImplementedException();
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            // CMD sX
            this.AssembleInternal(lines[1], assembler);
        }

        protected virtual void AssembleInternal(string register, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }
    }
}
