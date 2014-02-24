namespace IDE.CPU.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CommandBase
    {
        public static string COMMENT = ";";
        public static string LABEL = ":";
        public static char[] SEPARATORS = new char[] { ' ' };
        public static int FORMAT_COMMAND_LENGTH = 30;
        public static int FORMAT_LABEL_LENGTH = 15;

        public static void Parse(string lineText, int lineNumber, TheCpu cpu, out string[] lines)
        {
            var lineWithoutComment = lineText.Trim();
            if (lineText.Contains(COMMENT))
            {
                lineWithoutComment = lineText.Remove(lineText.IndexOf(COMMENT)).Trim();
            }
            if (lineWithoutComment.Contains(LABEL))
            {
                var label = lineWithoutComment.Remove(lineWithoutComment.IndexOf(LABEL)).Trim() + ':';
                lineWithoutComment = lineWithoutComment.Substring(lineWithoutComment.IndexOf(LABEL) + 1).Trim();

                var labelName = label.Remove(label.Length - 1).ToUpper();
                if (cpu.Labels.ContainsKey(labelName))
                {
                    cpu.Labels[labelName] = lineNumber;
                }
                else
                {
                    cpu.Labels.Add(labelName, lineNumber);
                }
            }

            lines = lineWithoutComment.Trim().Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void Parse(string lineText, int lineNumber, TheCpu cpu, out string[] lines, out string comment, out string label)
        {
            comment = string.Empty;
            label = string.Empty;

            var lineWithoutComment = lineText.Trim();
            if (lineWithoutComment.Contains(COMMENT))
            {
                comment = lineWithoutComment.Substring(lineWithoutComment.IndexOf(COMMENT)).Trim();
                lineWithoutComment = lineWithoutComment.Remove(lineWithoutComment.IndexOf(COMMENT)).Trim();
            }
            if (lineWithoutComment.Contains(LABEL))
            {
                label = lineWithoutComment.Remove(lineWithoutComment.IndexOf(LABEL)).Trim() + ':';
                lineWithoutComment = lineWithoutComment.Substring(lineWithoutComment.IndexOf(LABEL) + 1).Trim();

                var labelName = label.Remove(label.Length - 1).ToUpper();
                if (cpu.Labels.ContainsKey(labelName))
                {
                    cpu.Labels[labelName] = lineNumber;
                }
                else
                {
                    cpu.Labels.Add(labelName, lineNumber);
                }
            }

            lines = lineWithoutComment.Trim().Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
            if (label.Length < FORMAT_LABEL_LENGTH)
            {
                label += new string(' ', FORMAT_LABEL_LENGTH - label.Length);
            }
        }

        public virtual bool Verify(string[] lines, TheCpu cpu, out string error)
        {
            throw new NotImplementedException();
        }

        public virtual void Execute(string[] lines, TheCpu cpu)
        {
            throw new NotImplementedException();
        }

        public virtual string Format(string[] lines, string comment, string label)
        {
            throw new NotImplementedException();
        }

        public virtual void Assemble(string[] lines, TheCpu cpu, TheAssembler assembler)
        {
            throw new NotImplementedException();
        }
    }
}
