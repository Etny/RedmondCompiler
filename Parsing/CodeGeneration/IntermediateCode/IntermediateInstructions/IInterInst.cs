using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    interface IInterInst
    {

        void Emit(IlBuilder builder);

    }
}
