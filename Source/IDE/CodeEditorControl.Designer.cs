partial class CodeEditorControl
{
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.txtLines = new System.Windows.Forms.RichTextBox();
        this.txtCode = new System.Windows.Forms.RichTextBox();
        this.LinesUpdaterTimer = new System.Windows.Forms.Timer(this.components);
        this.SuspendLayout();
        // 
        // txtLines
        // 
        this.txtLines.BackColor = System.Drawing.Color.Gainsboro;
        this.txtLines.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtLines.Dock = System.Windows.Forms.DockStyle.Left;
        this.txtLines.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.txtLines.Location = new System.Drawing.Point(0, 0);
        this.txtLines.Name = "txtLines";
        this.txtLines.ReadOnly = true;
        this.txtLines.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
        this.txtLines.Size = new System.Drawing.Size(50, 375);
        this.txtLines.TabIndex = 0;
        this.txtLines.Text = "";
        this.txtLines.WordWrap = false;
        this.txtLines.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Lines_MouseUp);
        // 
        // txtCode
        // 
        this.txtCode.AcceptsTab = true;
        this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtCode.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.txtCode.Location = new System.Drawing.Point(50, 0);
        this.txtCode.Name = "txtCode";
        this.txtCode.Size = new System.Drawing.Size(605, 375);
        this.txtCode.TabIndex = 1;
        this.txtCode.Text = "";
        this.txtCode.WordWrap = false;
        this.txtCode.VScroll += new System.EventHandler(this.Code_VScroll);
        this.txtCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Code_KeyUp);
        this.txtCode.TextChanged += new System.EventHandler(this.Code_TextChanged);
        // 
        // LinesUpdaterTimer
        // 
        this.LinesUpdaterTimer.Enabled = true;
        this.LinesUpdaterTimer.Interval = 1000;
        this.LinesUpdaterTimer.Tick += new System.EventHandler(this.LinesUpdaterTimer_Tick);
        // 
        // CodeEditorControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.Controls.Add(this.txtCode);
        this.Controls.Add(this.txtLines);
        this.Name = "CodeEditorControl";
        this.Size = new System.Drawing.Size(655, 375);
        this.Load += new System.EventHandler(this.CodeBox_Load);
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox txtLines;
    private System.Windows.Forms.RichTextBox txtCode;
    private System.Windows.Forms.Timer LinesUpdaterTimer;
}
