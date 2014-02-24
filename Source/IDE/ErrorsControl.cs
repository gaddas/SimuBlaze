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
    public partial class ErrorsControl : UserControl
    {
        public class ErrorLine
        {
            public int Line { get; set; }
            public string Error { get; set; }

            public override int GetHashCode()
            {
                return this.Line.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var errorLine2 = obj as ErrorLine;

                return errorLine2 != null && errorLine2.Line == this.Line;
            }
        }

        private HashSet<ErrorLine> errors = new HashSet<ErrorLine>();

        public ErrorLine[] Errors
        {
            get
            {
                return this.errors.ToArray();
            }
        }

        private List<ErrorLine> errorsList = new List<ErrorLine>();

        public void ClearErrors()
        {
            errors.Clear();
            errorsList.Clear();

            this.gridControl1.DataSource = null;
            this.gridControl1.DataSource = this.Errors;
        }

        public void RemoveError(int line)
        {
            var errorLine = new ErrorLine();
            errorLine.Line = line;
            errorLine.Error = string.Empty;

            if (errors.Contains(errorLine))
            {
                errors.Remove(errorLine);
            }

            this.gridControl1.DataSource = null;
            this.gridControl1.DataSource = this.Errors;
        }

        private delegate void AddErrorDelegate(int line, string error);
        public void AddError(int line, string error)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddErrorDelegate(this.AddError), line, error);
            }
            else
            {
                var errorLine = new ErrorLine();
                errorLine.Line = line;
                errorLine.Error = error;

                if (errors.Contains(errorLine))
                {
                    errors.Remove(errorLine);
                }
                errors.Add(errorLine);

                this.gridControl1.DataSource = null;
                this.gridControl1.DataSource = this.Errors;
            }
        }

        public ErrorsControl()
        {
            InitializeComponent();
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column == gcImage && e.IsGetData)
            {
                //Set an icon with index 0
                e.Value = this.imageList1.Images[0];
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.ClearErrors();
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            if (this.OnNavigateTo != null)
            {
                int row = gridView1.GetSelectedRows()[0];
                int line = ((ErrorLine)gridView1.GetRow(row)).Line - 1;

                this.OnNavigateTo(this, line);
            }
        }

        #region [Events]

        public delegate void OnNavigateToHandler(object sender, int lineNumber);
        public event OnNavigateToHandler OnNavigateTo;

        #endregion
    }
}
