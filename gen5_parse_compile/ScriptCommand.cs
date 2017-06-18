using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

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
            if (name.StartsWith("cmd"))
            {
                string stringID = name.TrimStart("cmd".ToCharArray());

                bool parsingBool = ushort.TryParse(stringID, out commandID);

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
                // Something something XML parsing...I'm gonna save that for later.
                Console.WriteLine($"Unsupported command name '{name}'. Please use generic names.");
                Console.WriteLine("TODO: Implement XML support for command names");

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

        private string GetXmlCommandName(ushort id, ref bool canUseXml)
        {
            string cmdName = null;

            if (!File.Exists("cmd_table.xml"))
            {
                Console.WriteLine("Command table not found. Generic command names must be used.");
                canUseXml = false;
                return cmdName;
            }

            if (!IsCommandTableValid("cmd_table.xml"))
            {
                Console.WriteLine("Command table failed to validate, forcing generic names.");
                canUseXml = false;
                return cmdName;
            }

            FileStream cmdTable = File.Open("cmd_table.xml", FileMode.Open, FileAccess.Read);

            using (XmlReader cmdTableReader = XmlReader.Create(cmdTable))
            {
                cmdTableReader.MoveToContent();
                if (!cmdTableReader.ReadToDescendant("command"))
                {
                    Console.WriteLine("That...isn't a good XML file, I think.");
                    Console.WriteLine("Please add <command> elements to your XML file.");
                    return cmdName;
                }
            }

            return cmdName;
        }

        private bool IsCommandTableValid(string filename)
        {
            bool ret = true;

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", "cmd_table.xsd");

            XDocument tableDoc = XDocument.Load(filename);
            tableDoc.Validate(schemas, (o, e) =>
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Can't use the command table, see above and the XML schema.");
                ret = false;
            });

            return ret;
        }
    }
}
