using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Common
{
    static class OpCodeUtil
    {


        public static OpCode GetOpcode(string name)
        {
            foreach (var f in typeof(OpCodes).GetFields())
            {
                if (f.Name == name)
                    return (OpCode)f.GetValue(null);
            }
            return OpCodes.Nop;
        }

    }
}
