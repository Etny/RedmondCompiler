using Redmond.Common;
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

        public CodeSymbolLocation Location;


        public CodeSymbol(string id, string typeName, object value = null) : this(id, typeName, CodeSymbolLocation.Default, value) { }


        public CodeSymbol(string id, string typeName, CodeSymbolLocation location, object value = null) : base(typeName, value)
        {
            ID = id;
            Location = location;
            Value = "ID with name " + id;
        }

        public CodeSymbol(string id, CodeType type, CodeSymbolLocation location, object value = null) : base(type, value)
        {
            ID = id;
            Location = location;
            Value = "ID with name " + id;
        }


        public override string ToString()
            => ID + " of type " + Type.Name;

        public override void Push(IlBuilder builder)
            => Location.Push(builder);

        public void Store(IlBuilder builder)
            => Location.Store(builder);



    }

    public struct CodeSymbolLocation
    {
        public readonly CodeSymbolLocationType LocationType;
        public readonly int Index;

        public static CodeSymbolLocation Default = new CodeSymbolLocation(CodeSymbolLocationType.Local, 0);

        public CodeSymbolLocation(CodeSymbolLocationType type, int index)
        {
            LocationType = type;
            Index = index;
        }

        internal void Push(IlBuilder builder)
        {
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ld" + OpCodeSuffix + '_' + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ld" + OpCodeSuffix), Index);
        }

        internal void Store(IlBuilder builder)
        {
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("St" + OpCodeSuffix + '_' + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("St" + OpCodeSuffix), Index);
        }

        public string OpCodeSuffix 
            => LocationType switch {
                    CodeSymbolLocationType.Local => "loc",
                    CodeSymbolLocationType.Field => "fld",
                    _ => "arg",
                };
            
        

    }

    public enum CodeSymbolLocationType
    {
        Argument, Local, Field
    }
}
