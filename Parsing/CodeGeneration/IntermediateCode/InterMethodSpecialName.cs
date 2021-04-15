using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethodSpecialName : InterMethod
    {

        public InterMethodSpecialName(string name, ArgumentSymbol[] args, InterType owner, List<string> flags) : base(name, new BasicTypeName("void"), args, owner, flags)
        {
            AddFlag("rtspecialname");
            AddFlag("specialname");
        }


        
    }
}
