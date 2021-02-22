using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class CodeSymbol : CodeValue
    {
        public string ID;



        [SyntaxFunction("makeSymbol")]
        public static CodeSymbol MakeSymbol(string id, string type)
            => new CodeSymbol(id, type);

        [SyntaxFunction("makeSymbol")]
        public static CodeSymbol MakeSymbol(string id, string type, object val)
            => new CodeSymbol(id, type, val);

        public CodeSymbol(string id, string type, object value = null) : base(type, value)
        {
            ID = id;
            Value = "ID with name " + id;
        }

        public override string ToString()
            => ID + " of type " + Type.Name;

        public override OpCode PushCode => OpCodes.Ldarg_0;


    }
}
