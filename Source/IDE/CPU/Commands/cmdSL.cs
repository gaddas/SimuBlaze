namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    internal class cmdSL : CommandBaseSingleOperand
    {
        protected byte bit = 0;

        protected override void ExecuteInternal(string register, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);

            switch (this.commandString)
            {
                case "SL0":
                    bit = 0;
                    break;
                case "SL1":
                    bit = 1;
                    break;
                case "SLX":
                    bit = (byte) (sX & 0x01);
                    break;
                case "SLA":
                    bit = (byte) (cpu.CARRY ? 1 : 0);
                    break;
                default:
                    throw new ArgumentException("Невалидна операция по изместване на ляво");
            }

            cpu.CARRY = (sX & 0x80) == 0x80;

            sX = (byte)((sX << 1) | bit);

            cpu.SetRegister(register, sX);
            cpu.ZERO = sX == 0;
            cpu.PC++;
        }
    }
}
