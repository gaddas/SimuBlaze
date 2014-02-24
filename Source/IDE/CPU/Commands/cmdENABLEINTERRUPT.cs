namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "ENABLE INTERRUPT")]
    class cmdENABLEINTERRUPT : CommandBaseEmpty
    {
        public cmdENABLEINTERRUPT()
            : base()
        {
            base.commandString = "ENABLE INTERRUPT";
        }

        public override void Execute(string[] lines, TheCpu cpu)
        {
            cpu.INTERRUPT_ENABLE = true;
            cpu.PC++;
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            //111100000000000001
            var bits = "111100000000000001";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}

