using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    class IlBuilder
    {

        private readonly IStringStream _output;

        public IlBuilder(IStringStream output)
        {
            _output = output;
        }

        public void EmitString(string s)
            => _output.WriteLine(s);

    }
}
