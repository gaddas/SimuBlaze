namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "XOR")]
    internal class cmdXOR : CommandBaseDoubleOperand
    {
        public cmdXOR()
            : base()
        {
            base.commandString = "XOR";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            var r = (byte)(sX ^ sY);

            cpu.SetRegister(register1, r);
            cpu.ZERO = (r == 0);
            cpu.CARRY = false;
            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            var r = (byte)(sX ^ kk);

            cpu.SetRegister(register, r);
            cpu.ZERO = (r == 0);
            cpu.CARRY = false;
            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //001111XXXXYYYY0000
            var bits = "001111" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //001110XXXXKKKKKKKK
            var bits = "001110" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
