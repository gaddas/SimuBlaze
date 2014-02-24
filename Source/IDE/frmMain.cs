using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils;
using IDE.CPU;
using IDE.CPU.Commands;

namespace IDE
{
    public partial class frmMain : Form, IDisposable
    {
        public frmMain()
        {
            InitializeComponent();
        }

        public TheCpu CPU;
        public bool debugEnabled;
        public bool debugPaused;
        public bool debugStep;
        private string[] debugLines;
        private HashSet<int> debugBreakpoints;

        #region [Methods]

        private void EnableFileControls(bool enabled)
        {
            this.codeEditor.Enabled = enabled;

            this.bbiFileNew.Enabled = true;
            this.bbiFileOpen.Enabled = true;
            this.bbiFileClose.Enabled = enabled;
            this.bbtFileSave.Enabled = enabled;

            this.bbiFormatCode.Enabled = enabled;
            this.bbiCheckSyntax.Enabled = enabled;
            this.bbiBinariesAssemble.Enabled = enabled;
            this.bbiBinariesDisassemble.Enabled = !enabled;

            this.EnableDebuggingControls(enabled, false);
        }

        private delegate void EnableDebuggingControlsHandler(bool enabled, bool debuggingStarted);
        private void EnableDebuggingControls(bool enabled, bool debuggingStarted)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EnableDebuggingControlsHandler(this.EnableDebuggingControls), enabled, debuggingStarted);
            }
            else
            {

                if (CPU != null)
                {
                    CPU.Reset();
                }

                this.bbiDebugNext.Enabled = false;
                this.bbiDebugPause.Enabled = debuggingStarted && enabled;
                this.bbiDebugStop.Enabled = debuggingStarted && enabled;
                this.bbiDebugRun.Enabled = !debuggingStarted && enabled;

                this.codeEditor.ReadOnly = debuggingStarted && enabled;

                if (debuggingStarted)
                {
                    this.ribbonControlMain.SelectedPage = this.ribbonPageDebug;
                    this.registersTool.Enable(this.CPU);
                    this.stackTool.Enable();
                    this.scratchpadTool.Enable(this.CPU);
                }
                else
                {
                    this.ribbonControlMain.SelectedPage = this.ribbonPageFile;
                    this.registersTool.Disable();
                    this.stackTool.Disable();
                    this.scratchpadTool.Disable();
                }
            }
        }

        private void CodeOpen(string code)
        {
            this.codeEditor.keywordsLabels.Clear();
            this.codeEditor.keywordsConstants.Clear();
            this.codeEditor.breakpointLines.Clear();
            this.codeEditor.SourceCode = code;

            this.CPU = new TheCpu(this);
            this.stackTool.Prepare(this.CPU);
            this.scratchpadTool.Prepare(this.CPU);

            this.EnableFileControls(true);
        }

        private void CodeClose()
        {
            this.codeEditor.SourceCode = string.Empty;
            this.CPU = null;
            this.EnableFileControls(false);
        }

        private delegate void ShowMessageDelegate(string title, string message, ToolTipIconType icon);
        private void ShowMessage(string title, string message, ToolTipIconType icon)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new ShowMessageDelegate(this.ShowMessage), title, message, icon);
            }
            else
            {
                var show = DevExpress.Utils.ToolTipController.DefaultController.CreateShowArgs();
                show.Title = title;
                show.ToolTip = message;
                show.ToolTipType = ToolTipType.SuperTip;
                show.IconType = icon;
                show.IconSize = ToolTipIconSize.Large;
                show.ToolTipLocation = ToolTipLocation.LeftTop;
                show.AutoHide = false;
                DevExpress.Utils.ToolTipController.DefaultController.ShowHint(show, this.codeEditor);

                this.AddInformationLine(message);
            }
        }

        private delegate void AddInformationLineDelegate(string line);
        private void AddInformationLine(string line)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.txtOutput.InvokeRequired)
            {
                this.txtOutput.Invoke(new AddInformationLineDelegate(this.AddInformationLine), line);
            }
            else
            {
                this.txtOutput.AppendText(line + System.Environment.NewLine);
            }
        }

        private delegate void SetStatusTextDelegate(string text);
        private void SetStatusText(string text)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new SetStatusTextDelegate(this.SetStatusText), text);
            }
            else
            {
                this.barStaticItemCurrentLine.Caption = text;
            }
        }

        private delegate void SetDebugLineDelegate(int lineNumber);
        private void SetDebugLine(int lineNumber)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new SetDebugLineDelegate(this.SetDebugLine), lineNumber);
            }
            else
            {
                this.barStaticItemCurrentLine.Caption = string.Format("Изпълняване на линия {0}", lineNumber + 1);
                this.codeEditor.DebugLine = lineNumber;
                this.codeEditor.ScrollTo(lineNumber);
            }
        }

        private void StartDebug()
        {
            this.debugEnabled = true;
            this.debugPaused = false;
            this.debugLines = codeEditor.Lines;
            this.debugBreakpoints = codeEditor.breakpointLines;
            this.EnableDebuggingControls(true, true);

            Thread t = new Thread(this.DebugWorkerThread);
            t.Start();
        }

        private void StopDebug()
        {
            this.debugEnabled = false;
            this.debugPaused = false;
            this.EnableDebuggingControls(true, false);
            SetDebugLine(-1);
        }

        private void PauseDebug()
        {
            this.debugPaused = true;
            this.bbiDebugRun.Enabled = true;
            this.bbiDebugNext.Enabled = true;
        }

        private void DebugWorkerThread()
        {
            string error;
            string lineText;
            string comment;
            string label;
            string[] lineParsed;

            while (this.debugEnabled)
            {
                while (CPU.PC < debugLines.Length && this.debugEnabled)
                {
                    SetDebugLine(CPU.PC);

                    Thread.Sleep(100);
                    Application.DoEvents();

                    if (this.debugBreakpoints.Contains(CPU.PC))
                    {
                        this.PauseDebug();
                    }

                    while (this.debugPaused)
                    {
                        if (this.debugStep)
                        {
                            this.debugStep = false;
                            break;
                        }
                        Thread.Sleep(500);
                    }

                    lineText = debugLines[CPU.PC];

                    if (lineText.Trim() == string.Empty)
                    {
                        CPU.PC++;
                        continue;
                    }

                    CommandBase.Parse(lineText, CPU.PC, this.CPU, out lineParsed, out comment, out label);
                    CommandBase command = CommandManager.RecognizeCommand(lineText, lineParsed, this.CPU, out error);

                    if (command == null || error != null)
                    {
                        this.StopDebug();

                        errorsControl.AddError(CPU.PC + 1, error);
                        errorsControl.AddError(0, "Симулацията е прекратена поради открити грешки в кодът.");
                        this.SetStatusText("Симулацията е прекратена поради открити грешки в кодът.");
                        this.AddInformationLine("Симулацията е прекратена поради открити грешки в кодът.");
                        this.ShowMessage("Грешка при симулацията", error, ToolTipIconType.Error);

                        return;
                    }
                    else
                    {
                        try
                        {
                            command.Execute(lineParsed, this.CPU);
                        }
                        catch (Exception ex)
                        {
                            this.StopDebug();

                            this.AddInformationLine("Грешка при изпълнение на инструкцията:");
                            this.AddInformationLine(lineText);
                            this.ShowMessage("Грешка при изпълнение на инструкция", ex.ToString(), ToolTipIconType.Error);

                            return;
                        }
                    }
                }

                if (this.debugEnabled)
                {
                    this.ShowMessage("Препълване на програмният", "Препълване на програмният брояч.\nСимулацията е продължава от адрес 0x000.", ToolTipIconType.Information);
                    Thread.Sleep(1000);

                    CPU.PC = 0;
                }
            }

            this.SetDebugLine(-1);
            this.ShowMessage("Край на симулацията", "Симулацията е прекратена от потребителят.", ToolTipIconType.Information);
            this.SetStatusText("Симулацията е прекратена от потребителят.");
        }

        #endregion

        #region [Handlers]

        private void frmMain_Shown(object sender, EventArgs e)
        {
            this.ribbonControlMain.SelectedPage = this.ribbonPageFile;
        }

        private void bbiFileNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.CPU != null)
            {
                if (MessageBox.Show("Отворен файл", "Искате ли да затворите файлът без записване?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }

            AddInformationLine("Отваряне на нов файл");
            this.CodeOpen(string.Empty);
        }

        private void bbiFileOpen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.CPU != null)
            {
                if (MessageBox.Show("Искате ли да затворите файлът без записване?", "Отворен файл", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }

            var ofd = new OpenFileDialog();
            ofd.Filter = "PSM Файлове (*.PSM)|*.psm";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var sr = new System.IO.StreamReader(ofd.FileName, true);
                var source = sr.ReadToEnd();
                sr.Close();

                AddInformationLine("Отваряне на файл " + ofd.FileName);

                this.CodeOpen(source);
            }
        }

        private void bbtFileSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "PSM Файлове (*.PSM)|*.psm";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var sw = new System.IO.StreamWriter(sfd.FileName);
                sw.Write(this.codeEditor.SourceCode);
                sw.Flush();
                sw.Close();

                AddInformationLine("Записване на файл " + sfd.FileName);

                this.ShowMessage("Файлът записан", "файлът беше записан в " + sfd.FileName, ToolTipIconType.Information);
            }
        }

        private void bbiFileClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Потвърдете затварянето на файлът", "Затваряне на файл", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AddInformationLine("Затваряне на файл");
                this.CodeClose();
            }
        }

        private void codeEditor_OnCodeLine(object sender, int lineNumber, string lineText, out string lineForamtted, out bool validLine)
        {
            string error;
            string[] lines;
            string comment;
            string label;

            CommandBase.Parse(lineText, lineNumber, this.CPU, out lines, out  comment, out  label);

            CommandBase command = CommandManager.RecognizeCommand(lineText, lines, this.CPU, out error);

            if (command != null)
            {
                lineForamtted = lineText;
                validLine = true;

                if (error != null)
                {
                    errorsControl.AddError(lineNumber + 1, error);
                }
                else
                {
                    errorsControl.RemoveError(lineNumber + 1);
                    lineForamtted = command.Format(lines, comment, label);
                }
            }
            else
            {
                errorsControl.AddError(lineNumber, error);

                lineForamtted = lineText;
                validLine = false;
            }
        }

        private void errorsControl_OnNavigateTo(object sender, int lineNumber)
        {
            if (this.CPU != null)
            {
                this.codeEditor.Focus();
                this.codeEditor.NavigateTo(lineNumber);
            }
        }

        private void bbiCheckSyntax_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string[] lines = codeEditor.Lines;
            string error;

            errorsControl.ClearErrors();
            CPU.Literals.Clear();
            CPU.NamedRegisters.Clear();

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                string lineText = lines[lineNumber];

                if (lineText.Trim() == string.Empty) continue;

                string[] lineParsed;
                CommandBase.Parse(lineText, lineNumber, this.CPU, out lineParsed);
                CommandBase command = CommandManager.RecognizeCommand(lineText, lineParsed, this.CPU, out error);

                if (command == null || error != null)
                {
                    errorsControl.AddError(lineNumber + 1, error);
                }
            }

            AddInformationLine("Грешки открити при проверката: " + errorsControl.Errors.Length);
            dockPanelOutput.Show();
        }

        private void bbiFormatCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string[] lines = (string[])codeEditor.Lines.Clone();
            string error;

            errorsControl.ClearErrors();
            CPU.Literals.Clear();
            CPU.NamedRegisters.Clear();

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                string lineText = lines[lineNumber];

                if (lineText.Trim() == string.Empty) continue;

                string comment;
                string label;
                string[] lineParsed;

                CommandBase.Parse(lineText, lineNumber, this.CPU, out lineParsed, out comment, out label);
                CommandBase command = CommandManager.RecognizeCommand(lineText, lineParsed, this.CPU, out error);

                if (command == null || error != null)
                {
                    errorsControl.AddError(lineNumber + 1, error);
                }
                else
                {
                    lines[lineNumber] = command.Format(lineParsed, comment, label);
                }
            }

            codeEditor.SourceCode = string.Join("\n", lines);

            AddInformationLine("Грешки открити при форматирането на кодът: " + errorsControl.Errors.Length);
            dockPanelOutput.Show();
        }

        private void bbiDebugRun_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.debugEnabled && this.debugPaused)
            {
                this.debugPaused = false;
                this.bbiDebugRun.Enabled = false;
                this.bbiDebugNext.Enabled = false;
            } 
            else
            {
                this.StartDebug();
            }
        }

        private void bbiDebugPause_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.PauseDebug();
        }

        private void bbiDebugStop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.StopDebug();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.debugEnabled)
            {
                MessageBox.Show("Програмата е в процес на симулация и не може да бъде затворена.", "Затваряне на приложението", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
            }
        }

        private void bbiDebugNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.debugStep = true;
        }

        private void bbiBinariesAssemble_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "VHDL Source Code (*.VHD)|*.vhd|Verilog Source Code (*.V)|*.v";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var outputFilename = sfd.FileName;

                var assembler = new TheAssembler(this.CPU);
                var result = assembler.Assemble(codeEditor.Lines, this.CPU, outputFilename);
                if (string.IsNullOrEmpty(result))
                {
                    ShowMessage("Файлът е обработен", "Файлът е успешно асемблиран и запазен под името " + outputFilename, ToolTipIconType.Information);
                }
                else
                {
                    ShowMessage("Файлът е обработен", "Грешка при деасемблиране: " + result, ToolTipIconType.Error);
                }
            }

            this.Cursor = Cursors.Default;
        }

        private void bbiBinariesDisassemble_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "VHDL Source Code (*.VHD)|*.vhd|Verilog Source Code (*.V)|*.v";

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PicoBlaze Sourec Code (*.PSM)|*.psm";

            if (ofd.ShowDialog() == DialogResult.OK && sfd.ShowDialog() == DialogResult.OK)
            {
                var inputFilename = ofd.FileName;
                var outputFilename = sfd.FileName;

                var assembler = new TheAssembler(this.CPU);
                var result = assembler.Disassemble(inputFilename, outputFilename);
                if (string.IsNullOrEmpty(result))
                {
                    ShowMessage("Файлът е обработен", "Файлът е успешно деасемблиран и запазен под името " + outputFilename, ToolTipIconType.Information);
                }
                else
                {
                    ShowMessage("Файлът е обработен", "Грешка при деасемблиране: " + result, ToolTipIconType.Error);
                }
            }
        }

        #endregion

        #region IDisposable Members

        private bool isDisposed = false;

        void IDisposable.Dispose()
        {
            this.isDisposed = true;
            base.Dispose();
        }

        #endregion




    }
}
