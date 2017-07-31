using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenVScripting
{
    /// <summary>
    /// Gives a class the ability to read its values from a compiled script.
    /// </summary>
    public interface IDecompilable
    {
        BinaryReader Reader { set; }
        void Decompile();
    }
}
