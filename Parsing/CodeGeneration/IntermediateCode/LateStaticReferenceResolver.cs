using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    //TODO: Come up with a catchy name
    class LateStaticReferenceResolver : InterOp
    {
        private string[] _ids;

        public CodeType Type;
        public bool IsStatic = false;
        public bool IsFieldOrProperty => !IsStatic;

        private FieldOrPropertySymbol _field = null;

        public LateStaticReferenceResolver(SyntaxTreeNode node)
        {
            switch (node.Op)
            {
                case "Identifier":
                    _ids = new string[] { node.ValueString};
                    break;

                case "IdentifierExpression":
                    _ids = new string[] { node.ValueNode.ValueString };
                    break;

                case "MemberAccess":
                    var n = node;
                    List<string> ids = new List<string>();

                    while(n.Op == "MemberAccess")
                    {
                        ids.Add(n[0].ValueString);
                        n = n[1];
                    }

                    ids.Add(n.ValueNode.ValueString);

                    _ids = ids.ToArray();
                    Array.Reverse(_ids);
                    break;
            }
        }

        public override void Bind(IntermediateBuilder context)
        {
            int i = 0;
            for(; i < _ids.Length; i++)
            {
                Type = context.ResolveType(string.Join('.', _ids[0..(i+1)]));
                if (Type != null) break;
            }

            IsStatic = i+1 >= _ids.Length;
            if (!IsStatic)
            {
                int _fieldStart = i + 1;
                _field = new FieldOrPropertySymbol(Type as UserType, _ids[_fieldStart]);

                for (int j = _fieldStart + 1; j < _ids.Length; j++)
                    _field = new FieldOrPropertySymbol(_field, _ids[j]);

                _field.Bind(context);
                Type = _field.Type;
            }
        }

        public FieldOrPropertySymbol GetReferencedFieldOrProperty()
        {
            if (IsStatic) return null;
            return _field;
        }

        public override void Emit(IlBuilder builder)
        {
            if (IsStatic) return;

            builder.PushValue(GetReferencedFieldOrProperty());
        }

        public override CodeType GetResultType()
        {
            return Type;
        }
    }
}
