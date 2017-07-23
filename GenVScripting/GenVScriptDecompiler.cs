using System;
using System.IO;

namespace GenVScripting
{
    static class GenVScriptDecompiler
    {
        public static void ReadScriptsFromHeader(string scr)
        {
            // For more info about the script format of Gen V games (specifically B2W2), see
            // Kaphotic's and pichu2001's threads on scripting, accessible from here:
            // https://projectpokemon.org/forums/forums/topic/25852-b2w2-scripting-thread/
            // This program will also (hopefully) serve as documentation.

            using (BinaryReader reader = new BinaryReader(File.OpenRead(scr)))
            {
                // Sorry, all of you staring at this, horrified. Sorry.
                for (uint scriptOffset = reader.ReadUInt32(),
                        currentOffset = (uint)reader.BaseStream.Position;
                    (ushort)scriptOffset != 0xFD13;
                    scriptOffset = reader.ReadUInt32(),
                        currentOffset = (uint)reader.BaseStream.Position)
                {
                    // So we found a script to jump to, then.
                    Console.WriteLine($"Offset: 0x{scriptOffset:X8}");

                    // Seek forward, and read the script.
                    reader.BaseStream.Seek(scriptOffset, SeekOrigin.Current);
                    ReadScript(reader);

                    // After the script's been read, jump back and read the next offset.
                    reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
                }
            }

            return;
        }

        private static void ReadScript(BinaryReader reader)
        {
            CommandInfo current = new CommandInfo(reader);
            for (current.ReadValue(NumberSize.Word); true; current.ReadValue(NumberSize.Word))
            {
                Console.WriteLine(current.ToString());
                // or something along those lines. Possibly including a ScriptInfo to iterate thru.

                if (current.ID == 0x0002)
                    break;
            }

            Util.Log("Script ended. Returning...");
            return;
        }

        private static string ReadParamValue(BinaryReader reader, NumberSize type)
        {
            string paramValue = "0x";

            switch (type)
            {
                // TODO: Should a StringBuilder be used for performance?
                case NumberSize.Byte:
                    paramValue += reader.ReadByte().ToString("X2");
                    break;
                case NumberSize.Word:
                    paramValue += reader.ReadUInt16().ToString("X4");
                    break;
                case NumberSize.Dword:
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
