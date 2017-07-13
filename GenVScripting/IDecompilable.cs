using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenVScripting
{
    /// <summary>
    /// Gives a class the ability to read its values from a script.
    /// </summary>
    interface IDecompilable
    {
        BinaryReader Reader { set; }
        void ReadValue(NumberSize size);
    }
}
