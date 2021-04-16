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
                context.AddGenericParameter(gen.ValueString);

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

        [CodeGenFunction("PropertyDec")]
        public void CompilePropertyDecleration(SyntaxTreeNode node)
        {
            var decHeader = node[0];
            string access = decHeader[0].ValueString;
            var type = TypeNameFromNode(decHeader[2]);
            List<string> keywords = new List<string>();

            foreach (var c in decHeader[1].Children)
                keywords.Add(c.ValueString);

            string name = node[1].ValueString;

            InterMethod getMethod = null, setMethod = null;

            bool auto = false;

            InterProperty property = builder.AddProperty(name, type, access, keywords);
            InterField backingField = null;

            foreach (var dec in node[2].Children)
            {
                if (!auto && dec.Op == "AutoAccessorDec")
                {
                    auto = true;
                    backingField = builder.AddField(name + "__backingField", type, access, keywords);
                }

                bool get = dec[0].ValueString == "get";

                
                List<string> funcKeywords = new List<string>(keywords);
                funcKeywords.Add(dec[1].ValueString);

                PushNewTable();

                if (get)
                    getMethod = builder.AddMethod("get_" + name, type, new ArgumentSymbol[] { }, funcKeywords);
                else
                {
                    var arg = new ArgumentSymbol("value", type, 0);
                    setMethod = builder.AddMethod("set_" + name, new BasicTypeName("void"), new ArgumentSymbol[] { arg }, funcKeywords);
                }

                if (!auto) 
                    CompileNode(dec[2]);
                else
                {
                    if (get)
                    {
                        var resolver = new LateReferenceResolver(builder.CurrentNamespaceContext, backingField.Name);
                        resolver.SetOwner(builder.CurrentMethod);
                        builder.AddInstruction(new InterRet(new InterOpValue(resolver)));
                    }
                    else
                        builder.AddInstruction(new InterCopy(backingField.Symbol, GetFirst("value")));
                }

                Tables.Pop();
            }

            if (auto && node[3].Children.Length > 0)
                backingField.Initializer = ToIntermediateExpression(node[3][0]);
        
            if (getMethod != null) property.SetGet(getMethod);
            if (setMethod != null) property.SetSet(setMethod);

        }

        [CodeGenFunction("ReadonlyPropertyDec")]
        public void CompileReadonlyPropertyDecleration(SyntaxTreeNode node)
        {
            var decHeader = node[0];
            string access = decHeader[0].ValueString;
            var type = TypeNameFromNode(decHeader[2]);
            List<string> keywords = new List<string>();

            foreach (var c in decHeader[1].Children)
                keywords.Add(c.ValueString);

            string name = node[1].ValueString;

            var property = builder.AddProperty(name, type, access, keywords);

            PushNewTable();
            var get = builder.AddMethod("get_" + name, type, new ArgumentSymbol[] { }, keywords);
            builder.AddInstruction(new InterRet(ToIntermediateExpression(node[2])));
            Tables.Pop();

            property.SetGet(get);
        }

        private string GetName(SyntaxTreeNode node)
        {
            if (node.Children.Length == 1)
                return node[0].ValueString;
            else
                return GetName(node[0]) + '.' + node[1].ValueString;
        }

        private TypeName TypeNameFromNode(SyntaxTreeNode node)
        {
            switch (node.Op)
            {
                case "GenericType":
                    TypeName name = TypeNameFromNode(node[0]);
                    TypeName[] parameters = new TypeName[node[1].Children.Length];

                    for (int i = 0; i < parameters.Length; i++)
                        parameters[i] = TypeNameFromNode(node[1][i]);

                    return new GenericTypeName(name, builder.CurrentNamespaceContext, parameters);

                case "Array":
                    return new ArrayTypeName(TypeNameFromNode(node[0]), builder.CurrentNamespaceContext);

                case "Type":
                    return new BasicTypeName(CodeType.ByName(node.ValueString).Name);

                default:
                    return new BasicTypeName(GetName(node), builder.CurrentNamespaceContext);

            }
        }

    }
}
