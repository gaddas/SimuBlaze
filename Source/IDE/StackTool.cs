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
    public partial class StackTool : UserControl
    {
        private CPU.TheCpu _cpu;
        private List<StackLineEntry> List = new List<StackLineEntry>();

        public class StackLineEntry
        {
            public string Line { get; set; }
            public int LineNo { get; set; }
        }

        public StackTool()
        {
            InitializeComponent();
        }

        public void Prepare(CPU.TheCpu cpu)
        {
            _cpu = cpu;
            _cpu.PropertyChanged += new PropertyChangedEventHandler(CPU_PropertyChanged);
        }

        public void Enable()
        {
            this.Enabled = true;
        }

        public void Disable()
        {
            this.Enabled = false;
        }

        void CPU_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Stack")
            {
                List.Clear();

                foreach (var line in this._cpu.Stack)
                {
                    List.Add(new StackLineEntry()
                    {
                        Line = string.Format("инструкция на 0x{0:X3}", line + 1),
                        LineNo = line
                    });
                }

                this.gridControl1.DataSource = List;
                this.gridView1.RefreshData();
            }
        }
    }
}
