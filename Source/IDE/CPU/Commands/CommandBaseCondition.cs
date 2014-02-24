using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    class CommandBaseCondition : CommandBase
    {
        protected string commandString = "1234567890";

        public override bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            if (lines.Length > 0 && lines[0].ToUpper() == commandString)
            {
                if (lines.Length == 1)
                {
                    // CMD Address
                    error = null;
                    return true;
                }
                else if (lines.Length > 1)
                {
                    if (lines[1] == "C" || lines[1] == "NC" || lines[1] == "Z" || lines[1] == "NZ")
                    {
                        // CMD Condition, Address
                        error = null;
                        return true;
                    }
                    else
                    {
                        error = "Първият аргумент трябва да е условие";
                        return true;
                    }
                }
                else
                {
                    error = "Първият аргумент трябва да е условие";
                    return true;
                }
            }

            error = "Невалидна инструкция";
            return false;
        }

        public override string Format(string[] lines, string comment, string label)
        {
            var command = "";
            if (lines.Length == 2)
            {
                command = string.Format("    {0} {1}", lines[0].ToUpper(), lines[1].ToUpper());
            }
            else
            {
                command = string.Format("    {0}", lines[0].ToUpper());
            }
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
            if (lines.Length == 2)
            {
                switch (lines[1])
                {
                    case "C":
                        ExecuteInternal(cpu, cpu.CARRY);
                        break;
                    case "NC":
                        ExecuteInternal(cpu, !cpu.CARRY);
                        break;
                    case "Z":
                        ExecuteInternal(cpu, cpu.ZERO);
                        break;
                    case "NZ":
                        ExecuteInternal(cpu, !cpu.ZERO);
                        break;
                    default:
                        throw new ArgumentException("Невалидно състояние за инструкция JUMP");
                }
            }
            else if (lines.Length == 1)
            {
                ExecuteInternal(cpu, true);
            }
            else
            {
                throw new ArgumentException("Невалидна инструкция.");
            }
        }

        protected virtual void ExecuteInternal(TheCpu cpu, bool value)
        {
            throw new NotImplementedException();
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            if (lines.Length == 2)
            {
                switch (lines[1].Remove(lines[1].Length - 1))
                {
                    case "C":
                        AssembleInternalC(assembler);
                        break;
                    case "NC":
                        AssembleInternalNC(assembler);
                        break;
                    case "Z":
                        AssembleInternalZ(assembler);
                        break;
                    case "NZ":
                        AssembleInternalNZ(assembler);
                        break;
                    default:
                        throw new ArgumentException("Невалидно състояние за инструкция JUMP");
                }
            }
            else if (lines.Length == 1)
            {
                AssembleInternal(assembler);
            }
            else
            {
                throw new ArgumentException("Невалидна инструкция.");
            }
        }

        protected virtual void AssembleInternal(TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalC(TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalNC(TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalZ(TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalNZ(TheAssembler assembler)
        {
            throw new NotImplementedException();
        }
    }
}
