﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    abstract class CodeType
    {
        public string Name;

        private static Dictionary<string, CodeType> _types = new Dictionary<string, CodeType>();

        public static CodeType ByName(string name)
           => _types.GetValueOrDefault(name);

        protected CodeType(params string[] names)
        {
            Name = names[0];
            foreach(string n in names)
                if(n != "") _types.Add(n, this);
        }

        public abstract CodeType GetWiderType(CodeType otherType);

        public virtual OpCode GetPushCode()
            => throw new NotImplementedException();

        public abstract void Convert(CodeValue val, IlBuilder builder);


        public virtual void BindConversion(IntermediateBuilder context, CodeValue from) { }

        public override bool Equals(object obj)
           => obj is CodeType type && type.Name == Name;

        public static bool operator ==(CodeType lhs, object rhs) => rhs == null ? lhs is null : lhs.Equals(rhs);
        public static bool operator !=(CodeType lhs, object rhs) => rhs == null ? lhs is object : lhs is null || !lhs.Equals(rhs);



        //TODO: Add pointers
        //Define basic CLI types
        public static BasicType Int32 = new BasicType("I4", 1, "int32", "int");
        public static BasicType Int64 = new BasicType("I8", 2, "int64", "long");
        public static BasicType NativeInt = new BasicType("I", 3, "native");
        public static BasicType Real = new BasicType(4, "real");
        public static CodeType Object = new UserType(typeof(object));


        //Define derived types
        public static DerivedType Int8 = new DerivedType(.5f, Int32, "byte", "int8");
        public static DerivedType Bool = new DerivedType(.5f, Int32, "bool");
        public static DerivedType Int16 = new DerivedType(.7f, Int32, "short", "int16", "char");
        public static DerivedType Real4 = new DerivedType("R4", Real, "float32", "float", "real4");
        public static DerivedType Real8 = new DerivedType("R8", 4.5f, Real, "float64", "double", "decimal", "real8");
        public static StringType String = new StringType();

        public static VoidType Void = new VoidType();

    }
}