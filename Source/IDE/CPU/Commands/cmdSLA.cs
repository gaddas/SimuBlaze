namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    [Command(Command = "SLA")]
    internal class cmdSLA : cmdSL
    {
        public cmdSLA()
            : base()
        {
            base.commandString = "SLA";
        }

        protected override void AssembleInternal(string register, TheAssembler assembler)
        {
            //100000XXXX00000000
            var bits = "100000" + assembler.RegisterToBits4(register) + "00000000";
            assembler.Instruction(assembler.address, bits);
            assembler.address++;
        }
    }
}
