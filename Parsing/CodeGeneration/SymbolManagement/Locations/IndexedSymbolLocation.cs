using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class IndexedSymbolLocation : CodeSymbolLocation
    {
        public readonly IndexedSymbolLocationType LocationType;
        public readonly int Index;

        public static IndexedSymbolLocation Default = new IndexedSymbolLocation(IndexedSymbolLocationType.Local, 0);

        public IndexedSymbolLocation(IndexedSymbolLocationType type, int index)
        {
            LocationType = type;
            Index = index;
        }

        public IndexedSymbolLocation(IndexedSymbolLocationType type)
        {
            LocationType = type;
            Index = 0;
        }

        public override void Push(IlBuilder builder)
        {
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ld" + OpCodeSuffix + '_' + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Ld" + OpCodeSuffix), Index);
        }

        public override void Store(IlBuilder builder)
        {
            if (Index <= 3)
                builder.EmitOpCode(OpCodeUtil.GetOpcode("St" + OpCodeSuffix + '_' + Index));
            else
                builder.EmitOpCode(OpCodeUtil.GetOpcode("St" + OpCodeSuffix), Index);
        }

        public string OpCodeSuffix
            => LocationType switch
            {
                IndexedSymbolLocationType.Local => "loc",
                _ => "arg",
            };


        public enum IndexedSymbolLocationType
        {
            Argument, Local, Field
        }
    }
}
