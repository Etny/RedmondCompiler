using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterBlock : InterOp
    {
        private bool _opening = true;
        private string _name;
        private TypeName _typeName = TypeName.Unknown;
        private CodeType _type = null;

        public InterBlock(string name)
        {
            _name = name;
        }

        public InterBlock(string name, TypeName type) : this(name)
        {
            _typeName = type;
        }

        public InterBlock()
        {
            _opening = false;
        }

        public override void Bind(IntermediateBuilder context)
        {
            base.Bind(context);

            if (_typeName == TypeName.Unknown) return;
            _type = context.ResolveType(_typeName);

        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);
            if (_opening)
            {
                if (_type == null)
                    builder.EmitLine(_name);
                else
                    builder.EmitLine(_name + " " + _type.ShortName);

                builder.EmitLine("{");
                builder.Output.AddIndentation();
            }
            else
            {
                builder.Output.ReduceIndentation();
                builder.EmitLine("}");
            }
        }

        public override CodeType GetResultType() => _type;
    }
}
