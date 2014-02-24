namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "INPUT")]
    internal class cmdINPUT : CommandBaseDoubleOperandAddress
    {
        public cmdINPUT()
            : base()
        {
            base.commandString = "INPUT";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            sX = cpu.Input(sY);
            cpu.SetRegister(register1, sX);

            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            sX = cpu.Input(kk);
            cpu.SetRegister(register, sX);

            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //000101XXXXYYYY0000
            var bits = "000101" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //000100XXXXPPPPPPPP
            var bits = "000100" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
