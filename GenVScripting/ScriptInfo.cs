﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenVScripting
{
    /// <summary>
    /// Used to expose info about one of a file's scripts.
    /// </summary>
    class ScriptInfo : IDecompilable
    {
        List<CommandInfo> commands;
        List<int> offsets;
        BinaryReader reader;

        public List<CommandInfo> Commands { get => commands; set => commands = value; }
        public List<int> Offsets { get => offsets; set => offsets = value; }
        public BinaryReader Reader { set => reader = value; }

        public ScriptInfo(BinaryReader reader)
        {
            commands = new List<CommandInfo>();
            offsets = new List<int>();
            this.reader = reader;
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

        public void ReadValue(NumberSize size)
        {
            throw new NotImplementedException();
        }
    }
}
