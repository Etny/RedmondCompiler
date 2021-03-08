using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethodSpecialName : InterMethod
    {

        public InterMethodSpecialName(string name, CodeSymbol[] args, InterType owner) : base(name, "void", args, owner) 
        {
            AddFlag("rtspecialname");
            AddFlag("specialname");

            if (name == ".ctor")
                AddFlag("instance");
            else if (name == ".cctor")
                AddFlag("static");
        }

    }
}
