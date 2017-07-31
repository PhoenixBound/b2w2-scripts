using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenVScripting
{
    // The abstraction in this program/library jumps directly from commands to script files, with
    // no individual "scripts" in between. That's because scripts *aren't guaranteed to be modular*
    // or anything like that. Multiple scripts could jump to a simple "end" offset shared by all of
    // them. Nothing stops scripts from having their values unexpectedly aligned, as well.
    // 
    // Instead, this program deals with script "entrypoints." Entrypoints' header order is pre-
    // served, but they're added to the decompiled script in their actual file order (i.e. going
    // from smallest offset to largest offset). They're similar enough to normal offsets that they
    // don't get any special abstraction offsets don't.

    /// <summary>
    /// Used to expose info about all the scripts in a file.
    /// </summary>
    class ScriptFileInfo : IDecompilable
    {
        List<CommandInfo> commands;
        List<int> offsets;
        BinaryReader reader;

        public List<CommandInfo> Commands { get => commands; set => commands = value; }
        public List<int> Offsets { get => offsets; set => offsets = value; }
        public BinaryReader Reader { set => reader = value; }

        public ScriptFileInfo(BinaryReader reader)
        {
            commands = new List<CommandInfo>();
            offsets = new List<int>();
            Reader = reader;
        }

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

        /// <summary>
        /// Reads from a compiled script to find the values contained.
        /// </summary>
        /// <param name="size">Not used for this method.</param>
        public void Decompile()
        {
            
            throw new NotImplementedException();
        }
    }
}
