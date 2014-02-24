namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SRA")]
    internal class cmdSRA : cmdSR
    {
        public cmdSRA()
            : base()
        {
            base.commandString = "SRA";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00001000
            var bits = "100000" + assembler.RegisterToBits4(register) + "00001000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
