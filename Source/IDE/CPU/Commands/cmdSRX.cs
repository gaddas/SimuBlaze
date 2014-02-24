namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SRX")]
    internal class cmdSRX : cmdSR
    {
        public cmdSRX()
            : base()
        {
            base.commandString = "SRX";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00001010
            var bits = "100000" + assembler.RegisterToBits4(register) + "00001010";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
