using Redmond.Output;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Output.Error;
using Redmond.Output.Error.Exceptions;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        private Stack<SymbolTable> Tables = new Stack<SymbolTable>();
        private SymbolTable CurrentTable { get => Tables.Peek(); }


        private readonly SyntaxTreeNode _tree;
        private readonly IlBuilder builder;

        public CodeGenerator(SyntaxTreeNode tree)
        {
            _tree = tree;
            builder = new IlBuilder(new ConsoleStream());

            if (_codeGenFunctions.Count <= 0)
                _InitCodeGenFunctions();

            PushNewTable();
            CompileNode(tree);
        }

        private void CompileNode(SyntaxTreeNode node)
        {
            if (!_codeGenFunctions.ContainsKey(node.Op.ToLower()))
                throw new Exception("Unkown SyntaxNode Operator: " + node.Op);

            _codeGenFunctions[node.Op.ToLower()].Invoke(this, new object[] { node });
        }

        private void Push(CodeValue val)
        {
            builder.EmitOpCode(val.Type.PushCode, val.Value);
        }

        private void CompileNodes(params SyntaxTreeNode[] nodes)
        {
            foreach (var node in nodes)
                CompileNode(node);
        }

        private void PushNewTable() 
            => Tables.Push(new SymbolTable());

        private CodeSymbol GetFirst(string ID)
            => GetFirst(ID, out bool l);

        private CodeSymbol GetFirst(string ID, out bool local)
        {
            local = true;

            foreach (var t in Tables)
            {
                if (t.Contains(ID)) return t[ID];
                local = false;
            }

            return null;
        }

        private void MatchNode(SyntaxTreeNode node, string s, params string[] sr)
        {
            if (node.Op.ToLower() != s.ToLower() && !sr.ToList().Contains(node.Op.ToLower()))
                throw new Exception("uuuh...");

            CompileNode(node);
        }

    }
}
