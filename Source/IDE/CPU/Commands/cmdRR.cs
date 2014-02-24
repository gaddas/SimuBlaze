namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "RR")]
    internal class cmdRR : CommandBaseSingleOperand
    {
        public cmdRR()
            : base()
        {
            base.commandString = "RR";
        }

        protected override void ExecuteInternal(string register, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);

            cpu.CARRY = (sX & 0x01) == 0;

            sX = (byte) ((sX >> 1) | (sX << (8 - 1)));

            cpu.SetRegister(register, sX);
            cpu.ZERO = sX == 0;
            cpu.PC++;
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00001100
            var bits = "100000" + assembler.RegisterToBits4(register) + "00001100";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
