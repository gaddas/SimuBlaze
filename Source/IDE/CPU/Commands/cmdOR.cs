namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "OR")]
    internal class cmdOR : CommandBaseDoubleOperand
    {
        public cmdOR()
            : base()
        {
            base.commandString = "OR";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            var r = (byte)(sX | sY);

            cpu.SetRegister(register1, r);
            cpu.ZERO = (r == 0);
            cpu.CARRY = false;
            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            var r = (byte)(sX | kk);

            cpu.SetRegister(register, r);
            cpu.ZERO = (r == 0);
            cpu.CARRY = false;
            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //001011XXXXYYYY0000
            var bits = "001011" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //001100XXXXKKKKKKKK
            var bits = "001100" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
