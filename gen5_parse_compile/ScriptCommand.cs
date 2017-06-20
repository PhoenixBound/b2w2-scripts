using System;
using System.Collections.Generic;
using System.Globalization;
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

        // There's probably a better place for this in the main program...but it works for now.
        bool usesXml = true;

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
                // Update paramList and size
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
                // Update paramList and size
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
            string s = null;

            // Check if XML names are allowed still
            if (usesXml)
                s = GetXmlCommandName(id, ref usesXml);

            // If s is still null (or was set to it in the the XML function), generic name is used.
            if (s == null)
                s = $"cmd{id:x}";

            return s;
        }

        // Untested. This isn't good.
        public ushort GetCommandID(string name)
        {
            ushort commandID = 0x0000;

            // First, check if it's one of the hardcoded "cmd(number)"-type names.
            if (name.StartsWith("cmd"))
            {
                string stringID = name.TrimStart("cmd".ToCharArray());


                bool parsingBool = ushort.TryParse(stringID, NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, out commandID);

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

        // Entirely untested.
        private string GetXmlCommandName(ushort id, ref bool canUseXml)
        {
            string cmdName = null;

            if (!File.Exists("../../cmd_table.xml"))
            {
                Console.WriteLine("Command table not found. Generic command names must be used.");
                canUseXml = false;
                return cmdName;
            }

            FileStream cmdTable = File.Open("../../cmd_table.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // For XmlReader validation
            // TODO: Find performance difference between this and validating once
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", "../../cmd_table.xsd");

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Schemas = schemas,
                ValidationType = ValidationType.Schema
            };
            // If it fails, trigger an exception for now.

            // TODO: Maybe enclose this in try/catch and set canUseXml if an exception is caught?
            using (XmlReader cmdTableReader = XmlReader.Create(cmdTable, settings))
            {
                cmdTableReader.MoveToContent();
                cmdTableReader.ReadToFollowing("command");

                do
                {
                    ushort.TryParse(cmdTableReader.GetAttribute("id"), NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture, out ushort xmlID);

                    if (xmlID == id)
                    {
                        if (cmdTableReader.ReadToDescendant("name"))
                        {
                            cmdName = cmdTableReader.ReadElementContentAsString();
                            break;
                        }
                        // <name> doesn't exist in this case. That's perfectly fine.
                        // It can be null, it'll just fall back to a generic name.
                        // Using a generic name doesn't cut off access to the XML file as a whole.
                    }

                } while (cmdTableReader.ReadToNextSibling("command"));
            }

            return cmdName;
        }
    }
}
