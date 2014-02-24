using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    class CommandBaseDoubleOperandAddress : CommandBase
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
                if (lines[2].StartsWith("(") && lines[2].EndsWith(")") && cpu.ValidRegister(lines[2].Replace("(", "").Replace(")", "").Trim()))
                {
                    // CMD sX, (sY)
                    error = null;
                    return true;
                }
                else if (cpu.ValidLiteral(lines[2]))
                {
                    // CMD sX, kk
                    error = null;
                    return true;
                }
                else
                {
                    error = "Вторият аргумент на инструкцията трябва да е регистър (в скоби) или литерал";
                    return true;
                }
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
            var parameter1 = lines[1].Remove(lines[1].Length - 1);
            var parameter2 = lines[2].Replace("(", "").Replace(")", "").Trim();

            if (cpu.ValidRegister(parameter2))
            {
                // CMD sX, sY
                this.ExecuteV1(parameter1, parameter2, cpu);
            }
            else if (cpu.ValidLiteral(lines[2]))
            {
                // CMD sX, kk
                this.ExecuteV2(parameter1, lines[2], cpu);
            }
            else
            {
                throw new ArgumentException("Невалидна инструкция.");
            }
        }

        protected virtual void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            throw new NotImplementedException();
        }

        protected virtual void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            throw new NotImplementedException();
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            var parameter1 = lines[1].Remove(lines[1].Length - 1);
            var parameter2 = lines[2].Replace("(", "").Replace(")", "").Trim();

            if (cpu.ValidRegister(parameter2))
            {
                // CMD sX, sY
                this.AssembleV1(parameter1, parameter2, assembler);
            }
            else if (cpu.ValidLiteral(parameter2))
            {
                // CMD sX, kk
                this.AssembleV2(parameter1, parameter2, assembler);
            }
            else
            {
                throw new ArgumentException("Невалидна инструкция.");
            }
        }

        protected virtual void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }
    }
}
