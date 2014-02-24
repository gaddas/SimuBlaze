using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IDE
{
    public partial class RegistersTool : UserControl
    {
        public RegistersTool()
        {
            InitializeComponent();
        }

        public void Enable(CPU.TheCpu cpu)
        {
            this.bsCPU.DataSource = typeof(CPU.TheCpu);
            this.bsCPU.DataSource = cpu;
            this.Enabled = true;
        }

        public void Disable()
        {
            this.bsCPU.DataSource = typeof(CPU.TheCpu);
            this.Enabled = false;
        }
    }
}
