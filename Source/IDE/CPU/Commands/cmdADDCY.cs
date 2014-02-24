namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "ADDCY")]
    internal class cmdADDCY : CommandBaseDoubleOperand
    {
        public cmdADDCY()
            : base()
        {
            base.commandString = "ADDCY";
        }

        protected override void ExecuteV1(string register1, string register2, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register1);
            var sY = cpu.GetRegister(register2);

            var CARRY = (cpu.CARRY ? 1 : 0);
            var r = (byte)((sX + sY + CARRY) % 256);

            cpu.SetRegister(register1, r);
            cpu.ZERO = (r == 0);
            cpu.CARRY = ((sX + sY + CARRY) > 0xFF);
            cpu.PC++;
        }

        protected override void ExecuteV2(string register, string literal, TheCpu cpu)
        {
            var sX = cpu.GetRegister(register);
            var kk = cpu.GetLiteral(literal);

            var CARRY = (cpu.CARRY ? 1 : 0);
            var r = (byte)((sX + kk + CARRY) % 256);

            cpu.SetRegister(register, r);
            cpu.ZERO = (r == 0);
            cpu.CARRY = ((sX + kk + CARRY) > 0xFF);
            cpu.PC++;
        }

        protected override void AssembleV1(string register1, string register2, TheAssembler assembler)
        {
            //011011XXXXYYYY0000
            var bits = "011011" + assembler.RegisterToBits4(register1) + assembler.RegisterToBits4(register2) + "0000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleV2(string register, string literal, TheAssembler assembler)
        {
            //011010XXXXKKKKKKKK
            var bits = "011010" + assembler.RegisterToBits4(register) + assembler.LiteralToBits8(literal);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
