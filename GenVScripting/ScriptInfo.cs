using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenVScripting
{
    /// <summary>
    /// Used to expose info about one of a file's scripts.
    /// </summary>
    class ScriptInfo
    {
        List<CommandInfo> commands;

        public List<CommandInfo> Commands { get => commands; set => commands = value; }

        public string PrintScript()
        {
            foreach (CommandInfo c in commands)
            {
                Console.WriteLine(c.ToString());
                // Add offsets in there somewhere
            }

            // TODO: Replace this with the script as a string...or refactor all of this.
            return string.Empty;
        }
    }
}
