namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "COMPARE")]
    internal class cmdCOMPARE : CommandBaseDoubleOperand
    {
        public cmdCOMPARE()
            : base()
        {
            base.commandString = "COMPARE";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            cpu.ZERO = sX == sY;
            cpu.CARRY = sX > sY;
            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            var r = (byte)(sX & kk);

            cpu.ZERO = sX == kk;
            cpu.CARRY = sX > kk;
            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //010101XXXXYYYY0000
            var bits = "010101" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //010100XXXXKKKKKKKK
            var bits = "010100" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
