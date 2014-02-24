namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SL0")]
    internal class cmdSL0 : cmdSL
    {
        public cmdSL0()
            : base()
        {
            base.commandString = "SL0";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00000110
            var bits = "100000" + assembler.RegisterToBits4(register) + "00000110";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
