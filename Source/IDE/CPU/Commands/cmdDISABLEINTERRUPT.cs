namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "DISABLE INTERRUPT")]
    class cmdDISABLEINTERRUPT : CommandBaseEmpty
    {
        public cmdDISABLEINTERRUPT()
            : base()
        {
            base.commandString = "DISABLE INTERRUPT";
        }

        public override void Execute(string[] lines, TheCpu cpu)
        {
            cpu.INTERRUPT_ENABLE = false;
            cpu.PC++;
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            //111100000000000000
            var bits = "111100000000000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}

