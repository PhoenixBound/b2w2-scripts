// Gen 5 Script Parser and Compiler
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen5_parse_compile
{
    class Program
    {
        public static void Main(string[] args)
        {
            /* if (args.Length == 0)
                   ; // show compiler help
               if (args.Length == 1)
                   ; // the rest of the program as it stands now
             */

            // Script 180 from white 2 will be used to test. Feel free to use your own file.
            // Not sure why you'd want to use this program, of course! Heh.
            string scriptFile = "6_180.bin";
            if (args.Length > 0)
                scriptFile = args[0];

            if (!Util.IsFileUsable(scriptFile))
            {
                Console.WriteLine($"File '{scriptFile}' couldn't be used, see above.");
                return;
            }

            // Let's make some magic
            ReadScriptsFromHeader(scriptFile);

            Console.WriteLine("Testing has finished. Exit the program already.");
            Console.ReadLine();

            return;
        }

        static void ReadScriptsFromHeader(string scr)
        {
            // For more info about the script format of Gen V games (specifically B2W2), see
            // Kaphotic's and pichu2001's threads on scripting, accessible from here:
            // https://projectpokemon.org/forums/forums/topic/25852-b2w2-scripting-thread/
            // This program will also (hopefully) serve as documentation.

            Reference<bool> canUseXml = new Reference<bool>()
            {
                Val = true
            };

            using (BinaryReader reader = new BinaryReader(File.Open(scr, FileMode.Open,
                FileAccess.Read)))
            {
                for (uint scriptOffset = reader.ReadUInt32(),
                        currentOffset = (uint)reader.BaseStream.Position;
                    (ushort)scriptOffset != 0xFD13;
                    scriptOffset = reader.ReadUInt32(),
                        currentOffset = (uint)reader.BaseStream.Position)
                {
                    // So we found a script to jump to, then.
                    Console.WriteLine($"Offset: {scriptOffset:X8}");

                    // Seek forward, and read the script.
                    reader.BaseStream.Seek(scriptOffset, SeekOrigin.Current);
                    ReadScript(reader, canUseXml);

                    // After the script's been read, jump back and read the next offset.
                    reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
                }
            }

            return;
        }

        static void ReadScript(BinaryReader reader, Reference<bool> xml)
        {
            ScriptCommand current = new ScriptCommand(xml);
            for (current.ID = reader.ReadUInt16(); true; current.ID = reader.ReadUInt16())
            {
                Console.Write(current.Name + ' ');

                // TODO: Support arguments
                // foreach (CommandParameter c in current.paramList)
                //     Console.Write(ReadParamValue(reader, c.Type));

                Console.Write("\n");

                if (current.ID == 0x0002)
                    break;
            }

            Console.WriteLine("Script ended. Returning...");
            return;
        }

        static string ReadParamValue(BinaryReader reader, ParamType type)
        {
            string paramValue = "0x";

            switch (type)
            {
                case ParamType.byteParam:
                    paramValue += reader.ReadByte().ToString("X2");
                    break;
                case ParamType.wordParam:
                    paramValue += reader.ReadUInt16().ToString("X4");
                    break;
                case ParamType.dwordParam:
                    paramValue += reader.ReadUInt32().ToString("X8");
                    break;
                default:
                    paramValue = "ERROR PARSING PARAMETER. CODE BUGS. YAY.";
                    break;
            }

            return paramValue;
        }
    }
}
