using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    class IntermediateBuilder
    {
        public List<InterType> Types = new List<InterType>();

        public InterType CurrentType = null;
        public InterMethod CurrentMethod = null;

        public IntermediateBuilder()
        {
            AddType(new InterType("TestProject.TestType"));
        }

        public InterType AddType(InterType type)
        {
            CurrentType = type;
            Types.Add(type);
            return type;
        }

        public InterMethod AddMethod(InterMethod method)
        {
            CurrentMethod = method;
            CurrentType.AddMember(method);
            return method;
        }

        public IInterInst AddInstruction(IInterInst inst)
        {
            CurrentMethod.AddInstruction(inst);
            return inst;
        }

        public void Emit(IlBuilder builder)
        {
            foreach (InterType t in Types)
                t.Emit(builder);
        }

    }
}
