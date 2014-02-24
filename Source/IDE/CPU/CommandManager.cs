using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDE.CPU.Commands;
using System.Reflection;

namespace IDE.CPU
{
    public static class CommandManager
    {
        private static List<CommandBase> Commands;

        private static Dictionary<string, CommandBase> Cache;

        public static void InitializeCommands()
        {
            if (Commands == null)
            {
                Commands = new List<CommandBase>();
                Cache = new Dictionary<string, CommandBase>();

                foreach (Type type in GetCommands())
                {
                    Commands.Add((CommandBase)Activator.CreateInstance(type));
                }
            }
        }

        public static CommandBase RecognizeCommand(string line, string[] lines, TheCpu cpu, out string error)
        {
            bool isValid;

            // Keep cache small enounght
            if (Cache.Count > 1024 * 2)
            {
                Cache.Clear();
            }

            // Find first in cache
            if (Cache.ContainsKey(line))
            {
                CommandBase command = Cache[line];
                isValid = command.Verify(lines, cpu, out error);
                if (isValid)
                {
                    return command;
                }
                else
                {
                    Cache.Remove(line);
                }
            }

            // Recognize instruction
            foreach (CommandBase command in Commands)
            {
                isValid = command.Verify(lines, cpu, out error);
                if (isValid)
                {
                    Cache.Add(line, command);
                    return command;
                }
            }

            error = "Непозната инструкция";
            return null;
        }

        private static IEnumerable<Type> GetCommands()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(CommandAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
