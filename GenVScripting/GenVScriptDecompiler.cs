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
            for (current.ReadFromCompiled(); true; current.ReadFromCompiled())
            {
                Console.WriteLine(current.ToString());
                // or something along those lines. Possibly including a ScriptInfo to iterate thru.

                if (current.ID == 0x0002)
                    break;
            }

            Util.Log("Script ended. Returning...");
            return;
        }
    }
}
