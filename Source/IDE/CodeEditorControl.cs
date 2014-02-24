using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public partial class CodeEditorControl : UserControl
{
    #region [Fields]

    public List<string> keywordsCommands = new List<string>() { 
                                "constant",
			                    "address", 
 			                    "namereg", 
			                    "add", 
			                    "addcy", 
			                    "and", 
			                    "call", 
			                    "compare", 
			                    "diable", 
			                    "enable", 
			                    "fetch", 
			                    "input", 
			                    "jump", 
			                    "load", 
			                    "or", 
			                    "output", 
			                    "return", 
			                    "returni", 
			                    "rl", 
			                    "rr", 
			                    "sl0", 
			                    "sl1", 
			                    "sla", 
			                    "slx", 
			                    "sr0", 
			                    "sr1", 
			                    "sra", 
			                    "srx", 
			                    "store", 
			                    "sub", 
			                    "subcy", 
			                    "test", 
                                "xor",
                                "enable", "disable", "interrupt"};

    public List<string> keywordsRegisters = new List<string>() {
                            "s0", "s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "sa", "sb", "sc", "sd", "se", "sf",
                            "c", "nc", "z", };

    public List<string> keywordsConstants = new List<string>() { };
    public List<string> keywordsLabels = new List<string>() { };

    public HashSet<int> breakpointLines = new HashSet<int>();

    private int debugLine = -1;
    public int DebugLine
    {
        get
        {
            return debugLine;
        }

        set
        {
            var oldLine = debugLine;
            debugLine = value;
            this.FormatLine(oldLine);
            this.FormatLine(debugLine);
        }
    }

    private string lastText = "";
    public string CommentBegin = ";";

    private bool flag = false;
    public string SourceCode
    {
        get
        {
            return txtCode.Text;
        }

        set
        {
            txtCode.Text = value;

            int lineNumber = 0;

            flag = true;

            foreach (string line in txtCode.Lines)
            {
                this.FormatLine(lineNumber);
                lineNumber++;
            }

            flag = false;
        }
    }

    public string[] Lines
    {
        get
        {
            return txtCode.Lines;
        }
    }

    public bool ReadOnly
    {
        get
        {
            return this.txtCode.ReadOnly;
        }
        set
        {
            this.txtCode.ReadOnly = value;
        }
    }

    #endregion
    #region [Events]

    public delegate void OnCodeLineHandler(object sender, int lineNumber, string lineText, out string lineForamtted, out bool validLine);
    public event OnCodeLineHandler OnCodeLine;

    #endregion
    #region [Constructors]

    public CodeEditorControl()
    {
        InitializeComponent();
    }

    #endregion
    #region [Methods]

    public void NavigateTo(int lineNumber)
    {
        int start, end;
        this.GetStartEndIndexFromLine(lineNumber, out start, out end, txtCode.Text);

        txtCode.Focus();
        txtCode.SelectionStart = start;
        txtCode.SelectionLength = end - start;
        //txtCode.ScrollToCaret();

        this.Code_VScroll(this, EventArgs.Empty);
    }

    public void ScrollTo(int lineNumber)
    {
        int start, end;
        this.GetStartEndIndexFromLine(lineNumber, out start, out end, txtCode.Text);

        txtCode.Focus();
        txtCode.SelectionStart = start;
        txtCode.SelectionLength = 0;
        this.Code_VScroll(this, EventArgs.Empty);
    }

    public void FormatLine(int lineNumber)
    {
        this.FreezeDraw();
        int start = 0, end = 0;

        this.GetStartEndIndexFromLine(lineNumber, out start, out end, txtCode.Text);

        //// Calculate the start position of the current line
        //for (start = txtCode.SelectionStart - 1; start > 0; start--)
        //{
        //    if (txtCode.Text[start] == '\n') { start++; break; }
        //}
        //if (start == -1 && txtCode.SelectionStart == 0) start = 0;

        //// Calculate the end position of the current line.
        //for (end = txtCode.SelectionStart; end < txtCode.Text.Length; end++)
        //{
        //    if (txtCode.Text[end] == '\n') break;
        //}

        string line = "";

        try
        {
            line = txtCode.Text.Substring(start, end - start);
        }
        catch
        {
            this.UnfreezeDraw();
            return;
        }

        int sstart = txtCode.SelectionStart;
        int slen = txtCode.SelectionLength;

        if (line.Length > CommentBegin.Length - 1)
        {
            if (line.Substring(0, CommentBegin.Length) == CommentBegin)
            {
                txtCode.SelectionStart = start;
                txtCode.SelectionLength = end - start;
                txtCode.SelectionColor = Color.Green;
                txtCode.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                txtCode.SelectionStart = sstart;
                txtCode.SelectionLength = slen;
            }
            else
            {
                txtCode.SelectionStart = start;
                txtCode.SelectionLength = end - start;
                txtCode.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                txtCode.SelectionColor = Color.Black;
                txtCode.SelectionBackColor = Color.White;

                if (this.debugLine == lineNumber)
                {
                    txtCode.SelectionColor = Color.Black;
                    txtCode.SelectionBackColor = Color.Yellow;
                }
                else if (this.breakpointLines.Contains(lineNumber))
                {
                    txtCode.SelectionColor = Color.White;
                    txtCode.SelectionBackColor = Color.DarkRed;
                }
                else
                {
                    // Cut out the comment
                    string lineWithoutComments = line;
                    if (line.Contains(this.CommentBegin))
                    {
                        int commentIndex = line.IndexOf(this.CommentBegin);
                        txtCode.SelectionStart = start + commentIndex;
                        txtCode.SelectionLength = line.Length - commentIndex;
                        txtCode.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                        txtCode.SelectionColor = Color.Green;

                        lineWithoutComments = lineWithoutComments.Remove(commentIndex);
                    }

                    char[] splits = { ',', ' ', '{', '}', '(', ')', '\n', ';', '=', '[', ']', '\t' };
                    string[] tokens = lineWithoutComments.ToLower().Split(splits);
                    int index = 0;

                    foreach (string token in tokens)
                    {
                        txtCode.SelectionStart = start + index;
                        txtCode.SelectionLength = token.Length;

                        foreach (string kw in this.keywordsCommands)
                        {
                            if (kw == token)
                            {
                                txtCode.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                                txtCode.SelectionColor = Color.Blue;
                                break;
                            }
                        }

                        foreach (string kw in this.keywordsRegisters)
                        {
                            if (kw == token)
                            {
                                txtCode.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                                txtCode.SelectionColor = Color.DarkBlue;
                                break;
                            }
                        }

                        foreach (string kw in this.keywordsConstants)
                        {
                            if (kw == token)
                            {
                                txtCode.SelectionColor = Color.Blue;
                                break;
                            }
                        }

                        foreach (string kw in this.keywordsLabels)
                        {
                            if (kw == token)
                            {
                                txtCode.SelectionFont = new Font("Courier New", 12, FontStyle.Regular);
                                txtCode.SelectionColor = Color.Purple;
                                break;
                            }
                        }

                        index += token.Length + 1;
                    }
                }
            }

            txtCode.SelectionStart = sstart;
            txtCode.SelectionLength = slen;
        }

        Code_VScroll(this, EventArgs.Empty);
    }

    public enum ScrollBarType : uint
    {
        SbHorz = 0,
        SbVert = 1,
        SbCtl = 2,
        SbBoth = 3
    }

    public enum Message : uint
    {
        WM_VSCROLL = 0x0115
    }

    public enum ScrollBarCommands : uint
    {
        SB_THUMBPOSITION = 4
    }

    private const int WM_SETREDRAW = 0xB;

    [System.Runtime.InteropServices.DllImport("User32")]
    private static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    [DllImport("User32.dll")]
    public extern static int GetScrollPos(IntPtr hWnd, int nBar);

    [DllImport("User32.dll")]
    public extern static int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private void FreezeDraw()
    {
        SendMessage(txtCode.Handle, WM_SETREDRAW, 0, 0);
    }

    private void UnfreezeDraw()
    {
        SendMessage(txtCode.Handle, WM_SETREDRAW, 1, 0);
        txtCode.Invalidate(true);
    }

    private void GetStartEndIndexFromLine(int lineNumber, out int startIndex, out int endIndex, string text)
    {
        startIndex = 0;

        // Find start index of Code
        if (lineNumber > 0)
        {
            int currentNumber = 0;
            int currentIndex = 0;

            foreach (string line in txtCode.Lines)
            {
                if (currentNumber == lineNumber)
                {
                    startIndex = currentIndex;
                    break;
                }

                currentNumber++;
                currentIndex += line.Length + 1;
            }


            //int startLine = 0;
            //for (startIndex = 0; startIndex < text.Length; startIndex++)
            //{
            //    if (text[startIndex] == '\n')
            //    {
            //        startLine++;
            //    }

            //    if (text[startIndex] == '\n' && startLine == lineNumber)
            //    {
            //        startIndex++;
            //        break;
            //    }
            //}
        }

        // Find end index of Code
        endIndex = startIndex;
        for (endIndex = startIndex; endIndex < text.Length; endIndex++)
        {
            if (text[endIndex] == '\n')
            {
                break;
            }
        }
    }

    private int GetLineFromIndex(int index)
    {
        int lineNumber = 0;
        int lineIndex = 0;

        foreach (string line in txtCode.Lines)
        {
            if (lineIndex >= index)
            {
                return lineNumber;
            }

            lineNumber++;
            lineIndex += line.Length + 1;
        }

        //// Calculate the line number
        //for (int start = 0; start < index; start++)
        //{
        //    if (txtCode.Text[start] == '\n') { lineNumber++; }
        //}

        return lineNumber;
    }

    #endregion
    #region [Handlers]

    private void CodeBox_Load(object sender, EventArgs e)
    {
        txtLines.Font = new Font("Courier New", 12, FontStyle.Regular);
        txtLines.SelectAll();
        txtLines.SelectionAlignment = HorizontalAlignment.Right;

        txtCode.SelectAll();
        txtCode.SelectionIndent = 20;
        txtCode.Focus();
    }

    private void Code_TextChanged(object sender, EventArgs e)
    {
        if (flag) return;

        int lineNumber = GetLineFromIndex(txtCode.SelectionStart);
        this.FormatLine(lineNumber - 1);
    }

    private void Code_VScroll(object sender, EventArgs e)
    {
        int nPos = GetScrollPos(txtCode.Handle, (int)ScrollBarType.SbVert);
        nPos <<= 16;
        uint wParam = (uint)ScrollBarCommands.SB_THUMBPOSITION | (uint)nPos;
        SendMessage(txtLines.Handle, (int)Message.WM_VSCROLL, new IntPtr(wParam), new IntPtr(0));

        this.UnfreezeDraw();
    }

    private void Code_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            int sstart = txtCode.SelectionStart;
            int slen = txtCode.SelectionLength;

            int start = 0, end = 0, lineNumber = 0;

            // Calculate the line number
            for (start = 0; start < txtCode.SelectionStart; start++)
            {
                if (txtCode.Text[start] == '\n') { lineNumber++; }
            }

            // We want prevous line
            lineNumber--;

            this.GetStartEndIndexFromLine(lineNumber, out start, out end, txtCode.Text);
            string line = txtCode.Text.Substring(start, end - start);

            if (this.OnCodeLine != null)
            {
                bool isLineValid;
                string lineFormatted;
                this.OnCodeLine(this, lineNumber, line, out lineFormatted, out isLineValid);

                //if (isLineValid)
                //{
                //    this.FreezeDraw();

                //    txtCode.Text = txtCode.Text.Remove(start) + lineFormatted + txtCode.Text.Substring(end);

                //    for (int formatStart = 0; formatStart <= start; formatStart++)
                //    {
                //        if (txtCode.Text[formatStart] == '\n')
                //        {
                //            txtCode.SelectionStart = formatStart;
                //            txtCode.SelectionLength = 0;

                //            Code_TextChanged(this, EventArgs.Empty);
                //        }
                //    }

                //    txtCode.SelectionStart = sstart - line.Length + lineFormatted.Length;
                //    txtCode.SelectionLength = slen;

                //    Code_VScroll(this, EventArgs.Empty);
                //}
            }
        }
    }

    private void Lines_MouseUp(object sender, MouseEventArgs e)
    {
        FreezeDraw();

        int selectedIndex = txtLines.GetCharIndexFromPosition(new Point(e.X, e.Y));
        int selectedLine = 0;

        // Find start and end index of Lines, and the selected line number
        int currentIndex = 0;
        int currentLineStartIndex = 0;
        int currentLineEndIndex = 0;
        for (currentIndex = 0; currentIndex < selectedIndex; currentIndex++)
        {
            if (txtLines.Text[currentIndex] == '\n') { selectedLine++; currentLineStartIndex = currentIndex + 1; }
        }

        for (currentIndex = selectedIndex; currentIndex < txtLines.Text.Length; currentIndex++)
        {
            if (txtLines.Text[currentIndex] == '\n') { currentLineEndIndex = currentIndex; break; }
        }

        // Find start and end index of Code
        int startIndex = 0;
        int endIndex = 0;
        this.GetStartEndIndexFromLine(selectedLine, out startIndex, out endIndex, txtCode.Text);

        // Modify formatting
        txtCode.SelectionStart = startIndex;
        txtCode.SelectionLength = endIndex - startIndex;

        if (breakpointLines.Contains(selectedLine))
        {
            breakpointLines.Remove(selectedLine);
            txtCode.SelectionBackColor = txtCode.BackColor;

            txtLines.SelectionStart = currentLineStartIndex;
            txtLines.SelectionLength = currentLineEndIndex - currentLineStartIndex;
            txtLines.SelectionFont = new Font("Courier New", 12, FontStyle.Bold);
            txtLines.SelectionColor = Color.Black;
            txtLines.SelectionBackColor = Color.White;
        }
        else
        {
            breakpointLines.Add(selectedLine);
            txtCode.SelectionBackColor = Color.DarkRed;

            txtLines.SelectionStart = currentLineStartIndex;
            txtLines.SelectionLength = currentLineEndIndex - currentLineStartIndex;
            txtLines.SelectionFont = new Font("Courier New", 12, FontStyle.Bold);
            txtLines.SelectionColor = Color.White;
            txtLines.SelectionBackColor = Color.DarkRed;
        }

        UnfreezeDraw();

    }

    private void LinesUpdaterTimer_Tick(object sender, EventArgs e)
    {
        if (lastText == txtCode.Text)
        { }
        else
        {
            txtLines.Text = "";

            int lineCounter = txtCode.Text.Split('\n').Length;
            string lineText = "";

            for (int i = 1; i < lineCounter + 2; i++)
            {
                lineText = i.ToString() + "\n";
                txtLines.AppendText(lineText);

                if (breakpointLines.Contains(i - 1))
                {
                    txtLines.SelectionStart = txtLines.Text.Length - lineText.Length;
                    txtLines.SelectionLength = lineText.Length;
                    txtLines.SelectionFont = new Font("Courier New", 12, FontStyle.Bold);
                    txtLines.SelectionColor = Color.White;
                    txtLines.SelectionBackColor = Color.DarkRed;
                    txtLines.SelectionStart = 0;
                    txtLines.SelectionLength = 0;
                }
            }

            txtLines.SelectionStart = 0;
            txtLines.SelectionLength = txtLines.Text.Length;
            txtLines.SelectionFont = new Font("Courier New", 12, FontStyle.Bold);

            Code_VScroll(this, new EventArgs());
            lastText = txtCode.Text;
        }
    }

    #endregion
}
