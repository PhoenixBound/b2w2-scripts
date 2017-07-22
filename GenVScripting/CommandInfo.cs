using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace GenVScripting
{
    /// <summary>
    /// Used to expose info about a script command.
    /// </summary>
    public class CommandInfo : IDecompilable
    {
        private const string cmdTableFilename = "cmd_table.xml";
        private const string cmdTableSchema = "cmd_table.xsd";

        // Private variables
        private ushort id;
        private string name;
        private List<ParamInfo> paramList;
        private BinaryReader reader;

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

        public BinaryReader Reader { set => reader = value; }

        /// <summary>
        /// Initializer for reading from compiled scripts.
        /// </summary>
        /// <param name="reader">Used for reading the command's ID and data from the script.</param>
        public CommandInfo(BinaryReader reader)
        {
            // Initialize to current command about to be read
            this.reader = reader;
            ReadValue(NumberSize.Word);
            // The below aren't necessary for the current plan of ReadValue.
            // name = GetCommandName(id);
            // UpdateParams();
        }

        /// <summary>
        /// Initializer for writing a compiled script. Only a skeleton right now.
        /// </summary>
        /// <param name="writer">Writes a compiled script.</param>
        public CommandInfo(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        // Functions. Yay.

        public string GetCommandName(ushort id)
        {
            string s = null;

            // Use XML names...unless we aren't using XML in the first place.
            if (Util.UsesXml)
                s = GetCommandNameFromXml(id);

            // If s is still null (or was set to it in the the XML function), generic name is used.
            return s ?? $"cmd{id:x}";
        }

        private string GetCommandNameFromXml(ushort id)
        {
            string cmdName = null;

            if (!File.Exists(cmdTableFilename))
            {
                Util.Log("Command table not found. Generic command names must be used.");
                Util.UsesXml = false;
                return cmdName;
            }
            
            using (XmlReader cmdTableReader = XmlReader.Create(cmdTableFilename,
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

            return cmdName;
        }

        private XmlReaderSettings InitializeXmlSettings()
        {
            // For XmlReader validation
            // TODO: Find performance difference between this and validating once
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", cmdTableSchema);

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
                Util.Log("An error occurred while validating XML: " + args.Message);
                Util.Log("For now, this program will stop using the XML file.");
                Util.Log("Please fix errors in the XML file before compiling anything!");
            }
            else if (args.Severity == XmlSeverityType.Warning)
            {
                Util.Log("Warning: the XSD validation could not occur.");
                Util.Log($"Specifically, '{args.Message}'");
                Util.Log("XML usage will be disabled until this is fixed.");
            }
            else
            {
                Util.Log("An unknown error has occured: " + args.Message);
                Util.Log("XML usage will be disabled until this is fixed.");
                Util.Log("This message should have been unreachable...");
            }
            Util.UsesXml = false;
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
                Util.Log($"Unsupported command name '{name}'. Please use generic names.");
                Util.Log("TODO: Implement XML support for command names");

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
                Util.Log($"Invalid generic command '{name}.'");

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
            if (!Util.UsesXml || !File.Exists(cmdTableFilename))
            {
                // We don't serve their kind here.
                return;
            }

            if (paramList == null)
            {
                paramList = new List<ParamInfo>();
            }

            // If a CommandInfo is being reused, this needs to happen!
            // It is "update" params, not "add" params.
            paramList.Clear();

            // XML time.
            using (XmlReader cmdTableReader = XmlReader.Create(cmdTableFilename,
                InitializeXmlSettings()))
            {
                if (SeekToXmlCommandById(cmdTableReader))
                {
                    if (cmdTableReader.ReadToDescendant("arg"))
                    {
                        // Stuff. This is where the magic begins.
                        do
                        {
                            // Populate a new ParamInfo since we've found an "arg"
                            paramList.Add(new ParamInfo(reader)
                            {
                                Type = ParamInfo.ParseParamType(cmdTableReader
                                    .GetAttribute("type")),
                                // TODO: Make parts around this check for null. No empty strings!
                                Name = cmdTableReader.GetAttribute("name") ?? string.Empty
                            });
                        } while (cmdTableReader.ReadToNextSibling("arg"));
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

        public void ReadValue(NumberSize size)
        {
            throw new NotImplementedException();
            // Read a value for the ID, and set the ID property to update other vars
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder(name);

            foreach (ParamInfo p in paramList)
            {
                s.Append(' ');
                s.Append(p.ToString());
            }

            return s.ToString();
        }
    }
}
