namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "STORE")]
    internal class cmdSTORE : CommandBaseDoubleOperandAddress
    {
        public cmdSTORE()
            : base()
        {
            base.commandString = "STORE";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            cpu.WriteRAM(sY, sX);
            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            cpu.WriteRAM(kk, sX);
            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //101111XXXXYYYY0000
            var bits = "101111" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //101110XXXX00SSSSSS
            var bits = "101110" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
