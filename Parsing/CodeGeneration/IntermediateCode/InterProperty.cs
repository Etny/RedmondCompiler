using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterProperty
    {
        public readonly UserType Owner;
        public readonly string Name, Access;
        public readonly List<string> Keywords;

        public CodeType Type;
        private TypeName _typeName;

        public CodeValue Initializer = null;

        public InterMethod Get = null, Set = null;

        public InterProperty(string name, TypeName typeName, string access, List<string> keywords, UserType owner)
        {
            Name = name;
            Owner = owner;
            Access = access;
            Keywords = keywords;
            _typeName = typeName;
        }

        public bool IsStatic => Keywords.Contains("static");

        public void SetGet(InterMethod method)
            => Get = method;
        public void SetSet(InterMethod method)
            => Set = method;

        public void Bind(IntermediateBuilder builder)
        {
            Type = builder.ResolveType(_typeName);
            Get?.Bind(builder);
            Set?.Bind(builder);
        }

        public void Emit(IlBuilder builder)
        {
            string instance = IsStatic ? "" : "instance ";
            builder.EmitLine($".property {instance}{Type.Name} {Name} ()");
            builder.EmitLine("{");
            builder.Output.AddIndentation();

            if (Get != null)
                builder.EmitLine(".get " + Get.CallSignature);

            if (Set != null)
                builder.EmitLine(".set " + Set.CallSignature);

            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
        }
    }
}
