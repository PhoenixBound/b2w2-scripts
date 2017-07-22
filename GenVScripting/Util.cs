using System;
using System.IO;

namespace GenVScripting
{
    public static class Util
    {
        // Eventually, this bool will be refactored into a settings class if that's necessary.
        private static bool usesXml = true;
        public static bool UsesXml { get => usesXml; set => usesXml = value; }

        public static bool IsFileUsable(string filename)
        {
            if (!File.Exists(filename))
            {
                Log($"File {filename} does not exist. Check for typos 'n stuff.");
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
            //    Console.WriteLine("Come to think of it, how will you ever see this if your file \
            //    can't exist...?");
            //    return false;
            //}

            //if (filename.Length >= 248 && (filename.Contains("\\") || filename.Contains("/")))
            //{
            //    Console.WriteLine("That path's too long... Just like Sun and Moon's intro.");
            //    Console.WriteLine("Seriously, why you snoopin' 'round the source code, boi? \
            //    Kappa");
            //    return false;
            //}

            // ...Why did I just write all of that again...?
            // Whatever, the file *should* be okay to use. It's all just reading, no writing.

            return true;
        }

        public static void Log(string s)
        {
            // TODO: Make this write to a logfile or something.
            // Can't rely on this being in a console window in the future.
            Console.WriteLine(s);
            return;
        }
    }

    public sealed class Reference<T>
    {
        public T Val { get; set; }
    }
}
