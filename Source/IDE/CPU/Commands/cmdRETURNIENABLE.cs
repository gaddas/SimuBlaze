namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Command(Command = "RETURNI ENABLE")]
    class cmdRETURNIENABLE : CommandBaseEmpty
    {
        public cmdRETURNIENABLE()
            : base()
        {
            base.commandString = "RETURNI ENABLE";
        }

        public override void Execute(string[] lines, TheCpu cpu)
        {
            cpu.PC = cpu.Stack.Pop();
            cpu.OnPropertyChanged(cpu.PropertyName(() => cpu.Stack));

            cpu.ZERO = cpu.PRESERVED_ZERO;
            cpu.CARRY = cpu.PRESERVED_CARRY;
            cpu.INTERRUPT_ENABLE = true;
        }

        public override void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            //111000000000000001
            var bits = "111000000000000001";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}

