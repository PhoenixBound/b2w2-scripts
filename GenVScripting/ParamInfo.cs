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

        /// <summary>
        /// Initializer for reading from compiled scripts.
        /// </summary>
        /// <param name="b">Used for reading the param's value from the script.</param>
        public ParamInfo(BinaryReader b)
        {
            type = NumberSize.Byte;
            name = string.Empty;
            reader = b;
        }

        /// <summary>
        /// Retrieves and sets the param's name.
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Retrieves and sets the param's size.
        /// </summary>
        public NumberSize Type
        {
            get => type;
            set => type = value;
        }

        /// <summary>
        /// Sets the BinaryReader from which to read a param's value.
        /// </summary>
        public BinaryReader Reader
        {
            set => reader = value;
        }

        /// <summary>
        /// Gets the size in bytes of a parameter.
        /// </summary>
        /// <returns>The param's length in bytes.</returns>
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
                    Util.Log("FIXME: Bad parameter size for " + Name);
                    throw new NotImplementedException();
                    // return 0x1;
            }
        }

        /// <summary>
        /// Parses <see cref="string"/>s describing param sizes into <see cref="NumberSize"/>s.
        /// </summary>
        /// <param name="s">The <see cref="string"/> of a param size in the command table.</param>
        /// <returns>The <see cref="NumberSize"/> matching the string.</returns>
        public static NumberSize ParseParamType(string s)
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
                    Util.Log($"Unimplemented param type {s}. Disabling XML.");
                    Util.Log("Please fix the command table!");
                    Util.UsesXml = false;
                    return NumberSize.Word;
                    // TODO: replace this^^ with an exception and a handler.
            }
        }

        /// <summary>
        /// Reads from a compiled script to find the values contained.
        /// </summary>
        /// <param name="size">The size of the value to read.</param>
        public void ReadFromCompiled(NumberSize size)
        {
            switch (size)
            {
                case NumberSize.Byte:
                    safeId = reader.ReadByte();
                    break;
                case NumberSize.Word:
                    safeId = reader.ReadUInt16();
                    break;
                case NumberSize.Dword:
                    safeId = reader.ReadUInt32();
                    break;
                default:
                    // This should only be used by code, so an exception is fine.
                    throw new ArgumentException($"Unknown NumberSize '{size}' sent to ReadFromCompiled.");
            }
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current object.</returns>
        public override string ToString()
        {
            // The size of the param is in bytes, and a byte is two digits. EZ.
            // TODO: See the difference a StringBuilder makes here.
            return "0x" + safeId.ToString($"X{GetParamSize() * 2}");
        }
    }
}
