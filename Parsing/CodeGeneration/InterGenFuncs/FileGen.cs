using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

        [CodeGenFunction("Import")]
        public void CompileImport(SyntaxTreeNode node)
        {
            builder.AddImport(TypeNameFromNode(node[0]).Name);
        }


        [CodeGenFunction("NamespaceList")]
        public void CompileNamespaceList(SyntaxTreeNode node)
            => CompileNodes(node.Children);

        [CodeGenFunction("FileList")]
        public void CompileFileList(SyntaxTreeNode node)
        {
            builder.ClearImports();
            CompileNodes(node.Children);
        }

        [CodeGenFunction("NamespaceDec")]
        public void CompileNamespaceDec(SyntaxTreeNode node)
        {
            string name = TypeNameFromNode(node[0]).Name;

            builder.BeginNamespace(name);
            CompileNode(node[1]);
            builder.EndNameSpace();
        }
    }
}
