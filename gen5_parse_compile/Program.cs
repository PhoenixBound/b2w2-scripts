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
            string scriptFile = args[0] ?? "6_180.bin";

            if (!IsFileUsable(scriptFile))
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
            using (BinaryReader reader = new BinaryReader(File.Open(scr, FileMode.Open, FileAccess.Read)))
            {
                for (uint scriptOffset = reader.ReadUInt32(), currentOffset = (uint)reader.BaseStream.Position;
                    (ushort)scriptOffset != 0xFD13;
                    scriptOffset = reader.ReadUInt32(), currentOffset = (uint)reader.BaseStream.Position)
                {
                    // So we found a script to jump to, then.
                    Console.WriteLine($"Offset: {scriptOffset:X8}");

                    // Seek forward, and read the script.
                    reader.BaseStream.Seek(scriptOffset, SeekOrigin.Current);
                    ReadScript(reader);

                    // After the script's been read, jump back and read the next offset.
                    reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
                }
            }

            return;
        }

        /// <summary>Reads a single script from the current offset of the BinaryReader.</summary>
        static void ReadScript (BinaryReader reader)
        {
            ScriptCommand current = new ScriptCommand();
            for (current.ID = reader.ReadUInt16(); true; current.ID = reader.ReadUInt16())
            {
                Console.Write(current.Name + ' ');

                // TODO: Support arguments

                Console.Write("\n");

                if (current.ID == 0x0002)
                    break;
            }

            Console.WriteLine("Script ended. Returning...");
            return;
        }

        static bool IsFileUsable(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File {filename} does not exist. Check for typos 'n stuff.");
                return false;
            }

            // All of this should be unnecessary if we know the file doesn't exist! Heh.

            //if (string.IsNullOrWhiteSpace(filename))
            //{
            //    Console.WriteLine("Something's not right with the filename.");
            //    Console.WriteLine("Make sure it doesn't have invalid characters!");
            //    return false;
            //}

            //foreach (char c in filename)
            //{
            //    foreach (char d in Path.GetInvalidFileNameChars())
            //    {
            //        if (c == d)
            //        {
            //            Console.WriteLine("No. Bad filename characters. Bad.");
            //            return false;
            //        }
            //    }
            //}

            //if (filename.Length >= 260)
            //{
            //    Console.WriteLine("That filename is too long...probably.");
            //    Console.WriteLine("Please let me know if it isn't!");
            //    Console.WriteLine("Come to think of it, how will you ever see this if your file can't exist...?");
            //    return false;
            //}

            //if (filename.Length >= 248 && (filename.Contains("\\") || filename.Contains("/")))
            //{
            //    Console.WriteLine("That path's too long... Just like Sun and Moon's intro.");
            //    Console.WriteLine("Seriously, why you snoopin' 'round the source code, boi? Kappa");
            //    return false;
            //}

            // ...Why did I just write all of that again...?
            // Whatever, the file *should* be okay to use. It's all just reading, no writing.

            return true;
        }
    }
}
