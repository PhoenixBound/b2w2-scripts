using System;
using System.IO;

namespace gen5_parse_compile
{
    static class GenVScriptReader
    {
        public static void ReadScriptsFromHeader(string scr)
        {
            // For more info about the script format of Gen V games (specifically B2W2), see
            // Kaphotic's and pichu2001's threads on scripting, accessible from here:
            // https://projectpokemon.org/forums/forums/topic/25852-b2w2-scripting-thread/
            // This program will also (hopefully) serve as documentation.

            Reference<bool> canUseXml = new Reference<bool>()
            {
                Val = true
            };

            using (BinaryReader reader = new BinaryReader(File.OpenRead(scr)))
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

        private static void ReadScript(BinaryReader reader, Reference<bool> xml)
        {
            ScriptCommand current = new ScriptCommand(xml);
            for (current.ID = reader.ReadUInt16(); true; current.ID = reader.ReadUInt16())
            {
                Console.Write(current.Name + ' ');

                // TODO: Support arguments
                foreach (ParamInfo c in current.ParamList)
                {
                    Console.Write(ReadParamValue(reader, c.Type));
                    Console.Write(' ');
                }

                Console.Write("\n");

                if (current.ID == 0x0002)
                    break;
            }

            Console.WriteLine("Script ended. Returning...");
            return;
        }

        // Untested.
        private static string ReadParamValue(BinaryReader reader, ParamType type)
        {
            string paramValue = "0x";

            switch (type)
            {
                // TODO: Should a StringBuilder be used for performance?
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
                    paramValue = "WHAT TYPE OF PARAMETER IS THAT? THIS PROGRAM'S BUGGY.";
                    break;
            }

            return paramValue;
        }
    }
}
