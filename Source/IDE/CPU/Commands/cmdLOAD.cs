namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "LOAD")]
    internal class cmdLOAD : CommandBaseDoubleOperand
    {
        public cmdLOAD()
            : base()
        {
            base.commandString = "LOAD";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sY = cpu.GetRegister(register2);

            cpu.SetRegister(register1, sY);

            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var kk = cpu.GetLiteral(literal);

            cpu.SetRegister(register, kk);

            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //000001XXXXYYYY0000
            var bits = "000001" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
        
        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //000000XXXXKKKKKKKK
            var bits = "000000" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
