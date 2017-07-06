using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace gen5_parse_compile
{
    public class ScriptCommand
    {
        private const string cmdTableFilename = "cmd_table.xml";
        private const string cmdTableSchema = "cmd_table.xsd";

        // Private variables

        // ID of the command, like 0x0002 or 0x017A
        ushort id;

        // The name that matches with that ID
        string name;
        
        // Size of the whole command in bytes, including the command itself
        // May be removed in favor of paramList-based calculation?
        byte size;

        // Each of the command's parameters
        List<ParamInfo> paramList;

        // There's probably a better way to do this, but I don't know it.
        Reference<bool> usesXml;

        // Properties

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
                UpdateParams();
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
                UpdateParams();
                // Update paramList and size
            }
        }

        public List<ParamInfo> ParamList
        {
            get => paramList;
        }

        // Initializers
        public ScriptCommand()
        {
            // Initialize to "nop" by default
            id = 0;
            name = GetCommandName(id);
            UpdateParams();
            size = 2;
            
        }

        public ScriptCommand(Reference<bool> xml)
        {
            id = 0;
            usesXml = xml;
            name = GetCommandName(id);
            paramList = new List<ParamInfo>();
            size = 2;
        }

        // Functions. Yay.

        public string GetCommandName(ushort id)
        {
            string s = null;

            // Check if XML names are allowed still
            if (usesXml.Val)
                s = GetCommandNameFromXml(id);

            // If s is still null (or was set to it in the the XML function), generic name is used.
            s = s ?? $"cmd{id:x}";

            return s;
        }

        private string GetCommandNameFromXml(ushort id)
        {
            string cmdName = null;

            if (!File.Exists(cmdTableFilename))
            {
                Console.WriteLine("Command table not found. Generic command names must be used.");
                usesXml.Val = false;
                return cmdName;
            }

            // TODO: Maybe enclose this in try/catch and set canUseXml if an exception is caught?
            using (FileStream cmdTable = File.OpenRead(cmdTableFilename))
            {
                using (XmlReader cmdTableReader = XmlReader.Create(cmdTable,
                    InitializeXmlSettings()))
                {
                    if (SeekToXmlCommandById(cmdTableReader))
                    {
                        if (cmdTableReader.ReadToDescendant("name"))
                        {
                            cmdName = cmdTableReader.ReadElementContentAsString();
                        }
                        // <name> doesn't exist in this case. That's perfectly fine.
                        // It can be null, it'll just fall back to a generic name.
                        // Using a generic name doesn't cut off access to the whole XML file.
                    }
                }
            }

            return cmdName;
        }

        private XmlReaderSettings InitializeXmlSettings()
        {
            // For XmlReader validation
            // TODO: Find performance difference between this and validating once
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", "cmd_table.xsd");

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                Schemas = schemas,
                ValidationType = ValidationType.Schema
            };

            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);

            return settings;
        }

        private void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                Console.WriteLine("An error occurred while validating XML: " + args.Message);
                Console.WriteLine("For now, this program will stop using the XML file.");
                Console.WriteLine("Please fix errors in the XML file before compiling anything!");
            }
            else if (args.Severity == XmlSeverityType.Warning)
            {
                Console.WriteLine("Warning: the XSD validation could not occur.");
                Console.WriteLine($"Specifically, '{args.Message}'");
                Console.WriteLine("XML usage will be disabled until this is fixed.");
            }
            else
            {
                Console.WriteLine("An unknown error has occured: " + args.Message);
                Console.WriteLine("XML usage will be disabled until this is fixed.");
                Console.WriteLine("This message should have been unreachable...");
            }
            usesXml.Val = false;
            return;
        }

        // Untested. This isn't good.
        public ushort GetCommandID(string name)
        {
            ushort commandID = 0x0000;

            // First, check if it's one of the hardcoded "cmd(number)"-type names.
            // Note: this doesn't check if what comes after is actually a number!
            if (name.StartsWith("cmd"))
            {
                commandID = GetCommandIDFromGenericName(name);
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

        // Untested. This isn't good.
        private ushort GetCommandIDFromGenericName(string name)
        {
            string stringID = name.TrimStart("cmd".ToCharArray());


            bool parsingBool = ushort.TryParse(stringID, NumberStyles.HexNumber,
                CultureInfo.InvariantCulture, out ushort commandID);

            if (!parsingBool)
            {
                Console.WriteLine($"Invalid generic command '{name}.'");

                // This will probably be used to compile scripts in the future, so it makes the
                // most sense to me to have the script end early.
                commandID = 0x0002;
            }

            return commandID;
        }

        private byte GetParamsSize(List<ParamInfo> theParams)
        {
            byte paramsSize = 0;
            foreach (ParamInfo c in theParams)
            {
                paramsSize += c.GetParamSize();
            }
            return paramsSize;
        }

        private void UpdateParams()
        {
            if (!usesXml.Val)
            {
                // We don't serve their kind here.
                return;
            }

            if (!File.Exists(cmdTableFilename))
            {
                // No need to print a message to the console for this, one should exist already
                return;
            }

            if (paramList == null)
            {
                paramList = new List<ParamInfo>();
            }

            // XML time.
            using (FileStream cmdTable = File.OpenRead(cmdTableFilename))
            {
                using (XmlReader cmdTableReader = XmlReader.Create(cmdTable,
                    InitializeXmlSettings()))
                {
                    if (SeekToXmlCommandById(cmdTableReader))
                    {
                        if (cmdTableReader.ReadToDescendant("arg"))
                        {
                            // Stuff. This is where the magic begins.
                        }
                    }
                }
            }

            return;
        }

        private bool SeekToXmlCommandById(XmlReader cmdTableReader)
        {
            cmdTableReader.MoveToContent();
            cmdTableReader.ReadToFollowing("command");

            do
            {
                ushort.TryParse(cmdTableReader.GetAttribute("id"), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, out ushort xmlID);

                if (xmlID == id)
                {
                    // XML command could be found!
                    return true;
                }
            } while (cmdTableReader.ReadToNextSibling("command"));

            // XML command could not be found.
            return false;
        }
    }
}
