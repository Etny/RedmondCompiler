using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    class CodeSymbol
    {
        public string ID;
        public object Value;

        public CodeSymbol(string id, object val = null)
        {
            ID = id;
            Value = val;
        }


    }
}
