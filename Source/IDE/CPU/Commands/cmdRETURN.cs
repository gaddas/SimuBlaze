namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "RETURN")]
    class cmdRETURN : CommandBaseCondition
    {
        public cmdRETURN()
            : base()
        {
            base.commandString = "RETURN";
        }

        protected override void ExecuteInternal(TheCpu cpu, bool value)
        {
            if (value)
            {
                cpu.PC = cpu.Stack.Pop() + 1;
                cpu.OnPropertyChanged(cpu.PropertyName(() => cpu.Stack));
            }
            else
            {
                cpu.PC++;
            }
        }

        protected override void AssembleInternal(TheAssembler assembler)
        {
            //101010000000000000
            var bits = "101010000000000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalZ(TheAssembler assembler)
        {
            //101011000000000000
            var bits = "101011000000000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalNZ(TheAssembler assembler)
        {
            //101011010000000000
            var bits = "101011010000000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalC(TheAssembler assembler)
        {
            //101011100000000000
            var bits = "101011100000000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }

        protected override void AssembleInternalNC(TheAssembler assembler)
        {
            //101011110000000000
            var bits = "101011110000000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
