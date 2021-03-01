using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    abstract class InterInst
    {

        protected InterMethod Owner;

        public void SetOwner(InterMethod method)
            => Owner = method;

        public abstract void Emit(IlBuilder builder);

        public virtual void Bind(IntermediateBuilder context) { }

    }
}
