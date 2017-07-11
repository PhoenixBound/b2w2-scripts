// Gen 5 Script Parser and Compiler
using System;

namespace GenVScripting
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

            // Script 858 from white 2 will be used to test. Feel free to use your own file.
            // Not sure why you'd want to use this program, of course! Heh.
            string scriptFile = "a056_858.bin";
            if (args.Length > 0)
                scriptFile = args[0];

            if (!Util.IsFileUsable(scriptFile))
            {
                Console.WriteLine($"File '{scriptFile}' couldn't be used, see above.");
                return;
            }

            // Let's make some magic
            GenVScriptReader.ReadScriptsFromHeader(scriptFile);

            Console.WriteLine("Testing has finished. Exit the program already.");
            Console.ReadLine();

            return;
        }

        
    }
}
