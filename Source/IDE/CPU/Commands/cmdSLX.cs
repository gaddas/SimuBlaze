namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SLX")]
    internal class cmdSLX : cmdSL
    {
        public cmdSLX()
            : base()
        {
            base.commandString = "SLX";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00000100
            var bits = "100000" + assembler.RegisterToBits4(register) + "00000100";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
