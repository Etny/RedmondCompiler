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


        public CodeSymbol(string id, string type, CodeSymbolLocation location, object value = null) : base(type, value)
        {
            ID = id;
            Location = location;
            Value = "ID with name " + id;
        }

        public override string ToString()
            => ID + " of type " + Type.Name;

        public override OpCode PushCode => Location.GetPushOpcode();


    }

    public struct CodeSymbolLocation
    {
        public readonly CodeSymbolLocationType LocationType;
        public readonly int Index;

        public CodeSymbolLocation(CodeSymbolLocationType type, int index)
        {
            LocationType = type;
            Index = index;
        }

        public OpCode GetPushOpcode()
            => OpCodeUtil.GetOpcode("Ld" + OpCodeSuffix + '_' + Index);

        public OpCode GetStoreOpcode()
            => OpCodeUtil.GetOpcode("St" + OpCodeSuffix + '_' + Index);

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
