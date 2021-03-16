using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    abstract class InterInst
    {
        public string Label = "";

        protected InterMethod Owner;

        public void SetOwner(InterMethod method)
            => Owner = method;

        public virtual void Emit(IlBuilder builder) 
        { 
            if (Label == "") return;

            builder.Output.ReduceIndentationForLine();
            builder.EmitString(Label + ": "); 
        }

        public virtual void Bind(IntermediateBuilder context) { }

    }
}
