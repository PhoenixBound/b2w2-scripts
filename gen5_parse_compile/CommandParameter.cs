using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen5_parse_compile
{
    // Used to make it easy to tell a param's size.
    enum ParamType
    {
        byteParam,
        wordParam,
        dwordParam
    }

    public class CommandParameter
    {
        ParamType type;
        string name;

        public CommandParameter()
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
    }
}
