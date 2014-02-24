namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SL1")]
    internal class cmdSL1 : cmdSL
    {
        public cmdSL1()
            : base()
        {
            base.commandString = "SL1";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00000111
            var bits = "100000" + assembler.RegisterToBits4(register) + "00000111";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
