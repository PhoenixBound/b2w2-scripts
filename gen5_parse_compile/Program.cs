﻿// Gen 5 Script Parser and Compiler
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
                   ; // set single arg to filename, for parsing
             */
            
            // **************
            // Important vars
            // **************

            // Script 180 from white 2 will be used to test.
            // TODO: use an argument for this
            string scriptFile = "6_180.bin";
            // Store script offsets for later

            // This needs to be replaced with better logic
            // Also move it to a separate "CheckFileValidity" function or something when it gets
            // more complicated than this.
            if (!File.Exists(scriptFile))
            {
                Console.WriteLine("File '{0}' could not be found. Exiting...", scriptFile);
                return;
            }

            // Let's make some magic
            ReadScriptsFromHeader(scriptFile);

            Console.ReadLine();

            return;
        }

        static void ReadScriptsFromHeader(string scr)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(scr, FileMode.Open)))
            {
                for (uint scriptOffset = reader.ReadUInt32(), currentOffset = (uint)reader.BaseStream.Position;
                    ;
                    scriptOffset = reader.ReadUInt32(), currentOffset = (uint)reader.BaseStream.Position)
                {
                    // The 16-bit value 0xFD13 marks the end of the header, so
                    // no more scripts exist if this is true
                    if ((ushort)scriptOffset == 0xFD13)
                        break;

                    // So we found a script to jump to, then.
                    Console.WriteLine("Offset: {0:X8}", scriptOffset);

                    // Seek forward, and read the script.
                    reader.BaseStream.Seek(scriptOffset, SeekOrigin.Current);
                    ReadScript(reader);

                    // After the script's been read, jump back and read the next offset.
                    reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
                }
            }

            return;
        }

        static void ReadScript (BinaryReader reader)
        {
            ScriptCommand current = new ScriptCommand();
            for (current.ID = reader.ReadUInt16(); true; current.ID = reader.ReadUInt16())
            {
                // For testing - soon this will be replaced with command's name
                Console.WriteLine("Command found: 0x{0:X}", current.ID);
                // Console.Write(GetCommandName(command));

                // TODO: Support arguments

                if (current.ID == 0x0002)
                    break;
            }

            Console.WriteLine("Script ended. Returning...");
            return;
        }
    }
}
