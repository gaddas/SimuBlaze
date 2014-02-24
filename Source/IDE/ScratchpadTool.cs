using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using IDE.CPU;

namespace IDE
{
    public partial class ScratchpadTool : UserControl
    {
        private DynamicByteProvider scratchpadByteProvider;
        private TheCpu cpu;

        public ScratchpadTool()
        {
            InitializeComponent();           
        }

        public void Prepare(CPU.TheCpu CPU)
        {
            cpu = CPU;
            cpu.PropertyChanged += new PropertyChangedEventHandler(CPU_PropertyChanged);
        }

        public void Enable(TheCpu CPU)
        {
            this.scratchpadByteProvider = new DynamicByteProvider(CPU.Scratchpad);
            this.scratchpadByteProvider.Changed += new EventHandler(scratchpadByteProvider_Changed);
            this.hexBox.ByteProvider = scratchpadByteProvider;

            this.Enabled = true;
        }

        void scratchpadByteProvider_Changed(object sender, EventArgs e)
        {
            this.cpu.Scratchpad = this.scratchpadByteProvider.Bytes.GetBytes();
        }

        public void Disable()
        {
            this.scratchpadByteProvider = null;
            this.hexBox.ByteProvider = scratchpadByteProvider;

            this.Enabled = false;
        }

        void CPU_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Scratchpad" && this.Enabled)
            {
                scratchpadByteProvider = new DynamicByteProvider(cpu.Scratchpad);
                this.hexBox.ByteProvider = scratchpadByteProvider;
            }
        }
    }
}
