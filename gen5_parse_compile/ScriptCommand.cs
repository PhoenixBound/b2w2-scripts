﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen5_parse_compile
{
    public class ScriptCommand
    {
        // ID of the command, like 0x0002 or 0x017A
        ushort id;

        // The name that matches with that ID
        string name;
        
        // Size of the whole command in bytes, including the command itself
        // May be removed in favor of paramList-based calculation?
        byte size;

        // Each of the command's parameters
        List<CommandParameter> paramList;

        public ushort ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                name = GetCommandName(id);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                id = GetCommandID(name);
            }
        }

        public ScriptCommand()
        {
            // Initialize to "nop" by default
            id = 0;
            name = GetCommandName(id);
            paramList = new List<CommandParameter>();
            size = 2;
        }

        public string GetCommandName(ushort id)
        {
            // Something something XML parsing...I'm gonna save that for later.
            return $"cmd{id:x}";
        }

        // UNTESTED AS HECK, LOOK HERE FOR PROBLEMS WITH COMPILING
        public ushort GetCommandID(string name)
        {
            ushort commandID = 0x0000;

            // First, check if it's one of the hardcoded "cmd(number)"-type names.
            if (name.Length >= 4)
            {
                bool parsingBool = false;

                if (name.StartsWith("cmd"))
                {
                    string stringID = name.TrimStart("cmd".ToCharArray());
                    parsingBool = ushort.TryParse(stringID, out commandID);
                }

                if (!parsingBool)
                {
                    Console.WriteLine($"Invalid generic command '{name}.'");

                    // This will probably be used to compile scripts in the future, so it makes the
                    // most sense to me to have the script end early.
                    commandID = 0x0002;
                }
            }
            // Otherwise, if it isn't, try loading that command name from an XML file.
            else
            {
                Console.WriteLine($"Unsupported command name '{name}'. Please use generic names.");
                Console.WriteLine("TODO: Implement XML support for command names");

                // Something something XML parsing...I'm gonna save that for later.
                commandID = 0x0002;
            }
            return commandID;
        }

        private byte GetParamsSize(List<CommandParameter> theParams)
        {
            byte paramsSize = 0;
            foreach (CommandParameter c in theParams)
            {
                paramsSize += c.GetParamSize();
            }
            return paramsSize;
        }
    }
}
