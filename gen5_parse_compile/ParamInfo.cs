using System;

namespace gen5_parse_compile
{
    // Used to make it easy to tell a param's size.
    public enum ParamType
    {
        byteParam,
        wordParam,
        dwordParam
    }

    /// <summary>
    /// Used to get info on one of a script command's parameters.
    /// </summary>
    public class ParamInfo
    {
        ParamType type;
        string name;

        public ParamInfo()
        {
            type = ParamType.byteParam;
            name = string.Empty;
        }

        public byte GetParamSize()
        {
            switch (type)
            {
                case ParamType.byteParam:
                    return 0x1;
                case ParamType.wordParam:
                    return 0x2;
                case ParamType.dwordParam:
                    return 0x4;
                default:
                    Console.WriteLine("FIXME: Bad parameter size for " + Name);
                    return 0x1;
            }
        }

        public string Name
        {
            get => name;
            set => name = value;
        }
        public ParamType Type
        {
            get => type;
            set => type = value;
        }
    }
}
