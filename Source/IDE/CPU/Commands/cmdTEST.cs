namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "TEST")]
    internal class cmdTEST : CommandBaseDoubleOperand
    {
        public cmdTEST()
            : base()
        {
            base.commandString = "TEST";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            var r = (byte)(sX & sY);

            var bitCount = 0;
            if ((r & 1) == 1) bitCount++;
            if ((r & 2) == 2) bitCount++;
            if ((r & 4) == 4) bitCount++;
            if ((r & 8) == 8) bitCount++;
            if ((r & 16) == 16) bitCount++;
            if ((r & 32) == 32) bitCount++;
            if ((r & 64) == 64) bitCount++;
            if ((r & 128) == 128) bitCount++;

            cpu.ZERO = (r == 0);
            cpu.CARRY = (bitCount % 2) == 1;
            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            var r = (byte)(sX & kk);

            var bitCount = 0;
            if ((r & 1) == 1) bitCount++;
            if ((r & 2) == 2) bitCount++;
            if ((r & 4) == 4) bitCount++;
            if ((r & 8) == 8) bitCount++;
            if ((r & 16) == 16) bitCount++;
            if ((r & 32) == 32) bitCount++;
            if ((r & 64) == 64) bitCount++;
            if ((r & 128) == 128) bitCount++;

            cpu.ZERO = (r == 0);
            cpu.CARRY = (bitCount % 2) == 1;
            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //010011XXXXYYYY0000
            var bits = "010011" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //010010XXXXKKKKKKKK
            var bits = "010010" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
