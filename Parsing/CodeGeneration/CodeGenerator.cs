using Redmond.Output;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        private readonly SyntaxTreeNode _tree;
        private readonly IlBuilder builder;

        public CodeGenerator(SyntaxTreeNode tree)
        {
            _tree = tree;
            builder = new IlBuilder(new ConsoleStream());

            if (_codeGenFunctions.Count <= 0)
                _InitCodeGenFunctions();

            CompileNode(tree);
        }

        private void CompileNode(SyntaxTreeNode node)
        {
            if (!_codeGenFunctions.ContainsKey(node.Op.ToLower()))
                throw new Exception("Unkown SyntaxNode Operator: " + node.Op);

            _codeGenFunctions[node.Op.ToLower()].Invoke(this, new object[] { node });
        }

    }
}
