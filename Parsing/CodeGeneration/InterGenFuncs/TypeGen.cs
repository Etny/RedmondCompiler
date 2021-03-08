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
            builder.AddType(new InterType(name));
            CompileNode(node[1]);
        }

        [CodeGenFunction("FieldDec")]
        public void CompileFieldDecleration(SyntaxTreeNode node)
        {
            var dec = node[0];
            var field = builder.AddField(dec[0].ValueString, dec[1].ValueString);

            if(dec.Children.Length > 2)
                builder.CurrentType.Initializer.AddInstruction(new InterCopy(field.Symbol, ToIntermediateExpression(dec[2])));
           
        }



    }
}
