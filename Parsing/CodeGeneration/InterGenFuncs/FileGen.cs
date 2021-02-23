﻿using Redmond.Parsing.CodeGeneration.IntermediateCode;
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
            string moduleName = node.ValueString;

            var mod = Assembly.Load(moduleName);

            var t = from type in mod.GetTypes() where type.Name == "Console" select type;




        }

    }
}