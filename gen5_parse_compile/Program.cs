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
        static void Main(string[] args)
        {
            /* if (args.Length == 0)
                   ; // show compiler help
               if (args.Length == 1)
                   ; // set single arg to filename, for parsing   
             */
            
            // **************
            // Important vars
            // **************

            // Script 180 from white 2 will be used to test.
            // TODO: use an argument for this
            string scriptFile = "6_180.bin";
            // Store script offsets for later
            List<uint> offsets = new List<uint>();

            if (File.Exists(scriptFile))
            {
                // ********************************************************************
                // Start by finding all the offsets, and adding them to the offset list
                // ********************************************************************
                using (BinaryReader reader = new BinaryReader(File.Open(scriptFile, FileMode.Open)))
                {
                    Console.WriteLine("File opened: {0}", scriptFile);
                    // 0xFD13 is the magic ushort that ends the offset section.
                    while (offsets.Count == 0 || (offsets[offsets.Count - 1] & 0xFD13) != 0xFD13)
                    {
                        offsets.Add(reader.ReadUInt32());
                        // Debug: write offsets as hex strings
                        Console.WriteLine("New offset: {0}", offsets[offsets.Count - 1].ToString("X8"));
                    }

                    // Remove the last offset, the FD13 offset doesn't actually exist
                    offsets.RemoveAt(offsets.Count - 1);

                    // ************************************************************************
                    // Alright. We have the offsets, now let's follow them to find the scripts.
                    // ************************************************************************

                    for (int index = 0; index < offsets.Count; index++)
                    {
                        // Reset the position of the stream to 0x0, and seek to the first script offset
                        // Desired offset = (read position) + (listed offset), where read position is
                        // whatever offset the stream is at after reading a value.
                        //
                        // This should be refactored to be all at once with the offset finding, to be
                        // honest. It's ugly to find the offset again.
                        reader.BaseStream.Seek((index + 1) * 4 + offsets[index], SeekOrigin.Begin);
                        Console.WriteLine("Finding data at offset {0}", reader.BaseStream.Position.ToString("X8"));
                        ushort command = 0;
                        while (command != 0x2)
                        {
                            command = reader.ReadUInt16();
                            // TODO: Add argument support, like from a separate function.
                            // TODO: Make arg count read from a separate file?
                            // TODO: Add command names
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("File " + scriptFile + " does not exist. Dang it.");
            }

            Console.ReadLine();

            return;
        }
    }
}
