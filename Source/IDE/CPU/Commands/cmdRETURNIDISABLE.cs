namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "RETURNI DISABLE")]
    class cmdRETURNIDISABLE : CommandBaseEmpty
    {
        public cmdRETURNIDISABLE()
            : base()
        {
            base.commandString = "RETURNI DISABLE";
        }

        public override void Execute(string[] lines, TheCpu cpu)
        {
            cpu.PC = cpu.Stack.Pop();
            cpu.OnPropertyChanged(cpu.PropertyName(() => cpu.Stack));

            cpu.ZERO = cpu.PRESERVED_ZERO;
            cpu.CARRY = cpu.PRESERVED_CARRY;
            cpu.INTERRUPT_ENABLE = false;          
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            //111000000000000000
            var bits = "111000000000000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}

