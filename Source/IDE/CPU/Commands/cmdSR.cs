namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    internal class cmdSR : CommandBaseSingleOperand
    {
        protected byte bit = 0;

        protected override void ExecuteInternal(string register, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);

            switch (this.commandString)
            {
                case "SR0":
                    bit = 0;
                    break;
                case "SR1":
                    bit = 0x80;
                    break;
                case "SRX":
                    bit = (byte)(sX & 0x80);
                    break;
                case "SRA":
                    bit = (byte) (cpu.CARRY ? 0x80 : 0);
                    break;
                default:
                    throw new ArgumentException("Невалидна операция по изместване на ляво");
            }

            cpu.CARRY = (sX & 0x01) == 0x01;

            sX = (byte)((sX >> 1) | bit);

            cpu.SetRegister(register, sX);
            cpu.ZERO = sX == 0;
            cpu.PC++;
        }   
    }
}
