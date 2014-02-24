namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "JUMP")]
    class cmdJUMP : CommandBaseConditionSingleAddress
    {
        public cmdJUMP()
            : base()
        {
            base.commandString = "JUMP";
        }

        protected override void ExecuteInternal(string address, TheCpu cpu, bool value)
        {
            if (value)
            {
                var addressValue = cpu.GetAddress(address);
                cpu.PC = addressValue;
            }
            else
            {
                cpu.PC++;
            }
        }

        protected override void AssembleInternal(string address, TheAssembler assembler)
        {
            //11010000AAAAAAAAAA
            var bits = "11010000" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalZ(string address, TheAssembler assembler)
        {
            //11010100AAAAAAAAAA
            var bits = "11010100" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalNZ(string address, TheAssembler assembler)
        {
            //11010101AAAAAAAAAA
            var bits = "11010101" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalC(string address, TheAssembler assembler)
        {
            //11010110AAAAAAAAAA
            var bits = "11010110" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalNC(string address, TheAssembler assembler)
        {
            //11010111AAAAAAAAAA
            var bits = "11010111" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
