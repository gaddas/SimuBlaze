using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.CPU.Commands
{
    class CommandBaseConditionSingleAddress : CommandBase
    {
        protected string commandString = "1234567890";

        public override bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            if (lines.Length > 0 && lines[0].ToUpper() == commandString)
            {
                if (lines.Length > 1 && cpu.ValidAddress(lines[1]))
                {
                    // CMD Address
                    error = null;
                    return true;
                }
                else if (lines.Length > 1)
                {
                    if (!lines[1].EndsWith(","))
                    {
                        error = "Липсваща запетайка";
                        return true;
                    }

                    var lines1 = lines[1].Remove(lines[1].Length - 1);
                    if (lines1 == "C" || lines1 == "NC" || lines1 == "Z" || lines1 == "NZ")
                    {
                        if (lines.Length > 2 && cpu.ValidAddress(lines[2]))
                        {
                            // CMD Condition, Address
                            error = null;
                            return true;
                        }
                        else
                        {
                            error = "Вторият аргумент трябва да е адрес или етикет";
                            return true;
                        }
                    }
                    else
                    {
                        error = "Първият аргумент трябва да е адрес, етикет или условие";
                        return true;
                    }
                }
                else
                {
                    error = "Първият аргумент трябва да е адрес, етикет или условие";
                    return true;
                }
            }

            error = "Невалидна инструкция";
            return false;
        }

        public override string Format(string[] lines, string comment, string label)
        {
            var command = "";
            if (lines.Length == 3)
            {
                command = string.Format("    {0} {1} {2}", lines[0].ToUpper(), lines[1], lines[2]);
            }
            else
            {
                command = string.Format("    {0} {1}", lines[0].ToUpper(), lines[1]);
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
            if (lines.Length == 3)
            {
                switch (lines[1].Remove(lines[1].Length - 1))
                {
                    case "C":
                        ExecuteInternal(lines[2], cpu, cpu.CARRY);
                        break;
                    case "NC":
                        ExecuteInternal(lines[2], cpu, !cpu.CARRY);
                        break;
                    case "Z":
                        ExecuteInternal(lines[2], cpu, cpu.ZERO);
                        break;
                    case "NZ":
                        ExecuteInternal(lines[2], cpu, !cpu.ZERO);
                        break;
                    default:
                        throw new ArgumentException("Невалидно състояние за инструкция JUMP");
                }
            }
            else if (lines.Length == 2)
            {
                ExecuteInternal(lines[1], cpu, true);
            }
            else
            {
                throw new ArgumentException("Невалидна инструкция.");
            }
        }

        protected virtual void ExecuteInternal(string address, TheCpu cpu, bool value)
        {
            throw new NotImplementedException();
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            if (lines.Length == 3)
            {
                switch (lines[1].Remove(lines[1].Length - 1))
                {
                    case "C":
                        AssembleInternalC(lines[2], assembler);
                        break;
                    case "NC":
                        AssembleInternalNC(lines[2], assembler);
                        break;
                    case "Z":
                        AssembleInternalZ(lines[2], assembler);
                        break;
                    case "NZ":
                        AssembleInternalNZ(lines[2], assembler);
                        break;
                    default:
                        throw new ArgumentException("Невалидно състояние за инструкция JUMP");
                }
            }
            else if (lines.Length == 2)
            {
                AssembleInternal(lines[1], assembler);
            }
            else
            {
                throw new ArgumentException("Невалидна инструкция.");
            }
        }

        protected virtual void AssembleInternal(string address, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalC(string address, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalNC(string address, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalZ(string address, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }

        protected virtual void AssembleInternalNZ(string address, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }
    }
}
