namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "SUB")]
    internal class cmdSUB : CommandBaseDoubleOperand
    {
        public cmdSUB()
            : base()
        {
            base.commandString = "SUB";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            var r = (byte) ((sX - sY) % 256);

            cpu.SetRegister(register1, r);
            cpu.ZERO = ((sX- sY) == 0);
            cpu.CARRY = ((sX - sY) < 0);
            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            var r = (byte) ((sX - kk) % 256);

            cpu.SetRegister(register, r);
            cpu.ZERO = ((sX - kk) == 0);
            cpu.CARRY = ((sX - kk) < 0);
            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //011101XXXXYYYY0000
            var bits = "011101" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //011100XXXXKKKKKKKK
            var bits = "011100" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
