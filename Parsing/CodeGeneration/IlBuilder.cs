using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    class IlBuilder
    {
        public OutputStream Output;

        public IlBuilder(OutputStream output)
        {
            Output = output;
        }

        public void EmitOpCode(OpCode opCode, params object[] operands)
        {
            EmitString(opCode.ToString());

            foreach (var s in operands)
                EmitString(" " + s);

            EmitLine("");
        }

        public void EmitByte(byte b)
            => Output.WriteByte(b);

        public void EmitString(string s)
            => Output.WriteString(s);

        public void EmitLine(string s)
            => Output.WriteLine(s);

    }
}
