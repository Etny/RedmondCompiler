using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    interface IInterMember
    {
        void Emit(IlBuilder builder);
        void Bind(IntermediateBuilder builder);

        void BindSubMembers(IntermediateBuilder builder);

    }
}
