namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "CALL")]
    class cmdCALL : CommandBaseConditionSingleAddress
    {
        public cmdCALL()
            : base()
        {
            base.commandString = "CALL";
        }

        protected override void ExecuteInternal(string address, TheCpu cpu, bool value)
        {
            if (value)
            {
                cpu.Stack.Push(cpu.PC);
                cpu.OnPropertyChanged(cpu.PropertyName(() => cpu.Stack));

                var addressValue = cpu.GetAddress(address);
                cpu.PC = addressValue;

                if (cpu.Stack.Count > 31)
                {
                    throw new ArgumentException("Препълване на стекът");
                }
            }
            else
            {
                cpu.PC++;
            }
        }

        protected override void AssembleInternal(string address, TheAssembler assembler)
        {
            //11000000AAAAAAAAAA
            var bits = "11000000" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalZ(string address, TheAssembler assembler)
        {
            //11000100AAAAAAAAAA
            var bits = "11000100" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalNZ(string address, TheAssembler assembler)
        {
            //11000101AAAAAAAAAA
            var bits = "11000101" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalC(string address, TheAssembler assembler)
        {
            //11000110AAAAAAAAAA
            var bits = "11000110" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalNC(string address, TheAssembler assembler)
        {
            //11000111AAAAAAAAAA
            var bits = "11000111" + assembler.AddressToBits10(address);
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
