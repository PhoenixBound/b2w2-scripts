using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen5_parse_compile
{
    public class ScriptCommand
    {
        ushort id;
        string name;

        public ushort ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                name = GetCommandName(id);
            }
        }

        public ScriptCommand()
        {
            id = 0;
            name = GetCommandName(id);
        }

        private string GetCommandName(ushort id)
        {
            // Something something XML parsing...I'm gonna save that for later.
            return $"cmd{id:X}";
        }
    }
}
