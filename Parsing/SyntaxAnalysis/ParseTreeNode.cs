using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class ParseTreeNode : TreeNode<ParseTreeNode>
    {

        public ProductionEntry Value;

        public ParseTreeNode(ProductionEntry value)
        {
            Value = value;
        }

        public override string ToString()
            => Value.ToString();


        
    }
}
