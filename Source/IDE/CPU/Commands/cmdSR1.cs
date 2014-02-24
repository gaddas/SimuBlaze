namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SR1")]
    internal class cmdSR1 : cmdSR
    {
        public cmdSR1()
            : base()
        {
            base.commandString = "SR1";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00001111
            var bits = "100000" + assembler.RegisterToBits4(register) + "00001111";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
