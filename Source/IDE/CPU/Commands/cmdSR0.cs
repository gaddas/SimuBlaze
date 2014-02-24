namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SR0")]
    internal class cmdSR0 : cmdSR
    {
        public cmdSR0()
            : base()
        {
            base.commandString = "SR0";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00001110
            var bits = "100000" + assembler.RegisterToBits4(register) + "00001110";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
