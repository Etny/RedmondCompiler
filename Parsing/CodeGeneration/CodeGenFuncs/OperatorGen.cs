﻿using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        [CodeGenFunction("BinaryOperator")]
        public void CompileBinaryOperator(SyntaxTreeNode node)
        {
            builder.EmitLine("" + node.Val);
        }
    }
}
