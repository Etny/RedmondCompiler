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
            var type = new InterType(name, new NamespaceContext(builder.Namespaces), TypeNameFromNode(node[1][0]));
            foreach (var n in node[2].Children) type.AddFlag(n.ValueString);
            builder.AddType(type);
            CompileNode(node[3]);
        }

        [CodeGenFunction("FieldDec")]
        public void CompileFieldDecleration(SyntaxTreeNode node)
        {
            var decHeader = node[0];
            string access = decHeader[0].ValueString;
            List<string> keywords = new List<string>();

            foreach (var c in decHeader[1].Children)
                keywords.Add(c.ValueString);

            var field = builder.AddField(node[1].ValueString, TypeNameFromNode(decHeader[2]), access, keywords);

            if (node.Children.Length > 2)
                field.Initializer = ToIntermediateExpression(node[2]);
            
           
        }

        private TypeName TypeNameFromNode(SyntaxTreeNode node)
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

            return new TypeName(GetName(node), builder.CurrentNamespaceContext);
        }

    }
}
