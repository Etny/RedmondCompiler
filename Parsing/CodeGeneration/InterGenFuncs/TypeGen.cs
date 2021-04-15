using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

        
        [CodeGenFunction("Class")]
        public void CompileClass(SyntaxTreeNode node)
        {
            string name = node[0].ValueString;

            var context = new ResolutionContext(builder.Namespaces);

            foreach (var gen in node[3].Children)
                context.GenericParameters.Add(gen.ValueString);

            var type = new InterType(name, context, TypeNameFromNode(node[1][0]), context.GenericParameters.Count);
            foreach (var n in node[2].Children) type.AddFlag(n.ValueString);
            builder.AddType(type);
            CompileNode(node[4]);
            builder.CurrentType = null;
        }

        [CodeGenFunction("FieldDec")]
        public void CompileFieldDecleration(SyntaxTreeNode node)
        {
            var decHeader = node[0];
            string access = decHeader[0].ValueString;
            List<string> keywords = new List<string>();

            foreach (var c in decHeader[1].Children)
                keywords.Add(c.ValueString);

            foreach(var n in node[1].Children)
            {
                var field = builder.AddField(n[0].ValueString, TypeNameFromNode(decHeader[2]), access, keywords);

                if (n.Children.Length > 1)
                    field.Initializer = ToIntermediateExpression(n[1]);
            }
           
        }

        private TypeName TypeNameFromNode1(SyntaxTreeNode node)
        {
            string GetName(SyntaxTreeNode node)
            {
                if (node.Op == "Array")
                    return GetName(node[0]) + "[]";

                if (node.Op == "Type")
                    return CodeType.ByName(node.ValueString).Name;

                if (node.Children.Length == 1)
                    return node[0].ValueString;
                else
                    return GetName(node[0]) + '.' + node[1].ValueString;
            }

            return new BasicTypeName(GetName(node), builder.CurrentNamespaceContext);
        }

        private string GetName1(SyntaxTreeNode node)
        {
            if (node.Children.Length == 1)
                return node[0].ValueString;
            else
                return GetName1(node[0]) + '.' + node[1].ValueString;
        }

        private TypeName TypeNameFromNode(SyntaxTreeNode node)
        {
            switch (node.Op)
            {
                case "GenericType":
                    TypeName name = TypeNameFromNode1(node[0]);
                    TypeName[] parameters = new TypeName[node[1].Children.Length];

                    for (int i = 0; i < parameters.Length; i++)
                        parameters[i] = TypeNameFromNode1(node[1][i]);

                    return new GenericTypeName(name, builder.CurrentNamespaceContext, parameters);

                case "Array":
                    return new ArrayTypeName(TypeNameFromNode1(node[0]), builder.CurrentNamespaceContext);

                case "Type":
                    return new BasicTypeName(CodeType.ByName(node.ValueString).Name);

                default:
                    return new BasicTypeName(GetName1(node), builder.CurrentNamespaceContext);

            }
        }

    }
}
