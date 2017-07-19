using System;
using System.IO;

namespace GenVScripting
{
    /// <summary>
    /// Used to expose info on one of a script command's parameters.
    /// </summary>
    public class ParamInfo : IDecompilable
    {
        NumberSize type;
        // TODO: This "safe ID" is me trying to be safe since I don't know the size of the
        // parameter beforehand. Maybe inheritance would be useful here? :/
        uint safeId;
        string name;
        BinaryReader reader;

        public ParamInfo(BinaryReader b)
        {
            type = NumberSize.Byte;
            name = string.Empty;
            reader = b;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }
        public NumberSize Type
        {
            get => type;
            set => type = value;
        }

        public BinaryReader Reader
        {
            set => reader = value;
        }

        public byte GetParamSize()
        {
            switch (type)
            {
                case NumberSize.Byte:
                    return 0x1;
                case NumberSize.Word:
                    return 0x2;
                case NumberSize.Dword:
                    return 0x4;
                default:
                    Console.WriteLine("FIXME: Bad parameter size for " + Name);
                    return 0x1;
            }
        }

        public static NumberSize ParseParamType(string s, Reference<bool> b)
        {
            switch (s)
            {
                case "byte":
                    return NumberSize.Byte;
                case "word":
                    return NumberSize.Word;
                case "dword":
                    return NumberSize.Dword;
                default:
                    Console.WriteLine("Unimplemented param type {0}. Disabling XML.", s);
                    Console.WriteLine("Please fix the command table!");
                    b.Val = false;
                    return NumberSize.Byte;
            }
        }

        public void ReadValue(NumberSize size)
        {

        }


        public override string ToString()
        {
            // TODO: Make this better
            return safeId.ToString("X");
        }
    }
}
