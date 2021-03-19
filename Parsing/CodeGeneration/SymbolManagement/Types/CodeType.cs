using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    abstract class CodeType
    {
        public string Name { get; protected set; }
        public string ShortName { get; protected set; }

        public CodeType StoredType { get; protected set; }


        private static Dictionary<string, CodeType> _types = new Dictionary<string, CodeType>();

        public static CodeType ByName(string name)
           => _types.GetValueOrDefault(name);

        protected CodeType(params string[] names)
        {
            Name = names[0];
            ShortName = Name;
            StoredType = this;
            foreach(string n in names)
                if(n != "") _types.Add(n, this);
        }

        public abstract CodeType GetWiderType(CodeType otherType);

        public abstract AssignType CanAssignTo(CodeType fieldType);

        public virtual OpCode GetPushCode()
            => throw new NotImplementedException();

        public abstract void ConvertFrom(CodeValue val, IlBuilder builder);

        public virtual void ConvertTo(CodeValue val, IlBuilder builder, CodeType to) { to.ConvertFrom(val, builder); }



        public virtual void BindConversion(IntermediateBuilder context, CodeValue from) { }

        public override bool Equals(object obj)
           => obj is CodeType type && type.Name == Name;

        public static bool operator ==(CodeType lhs, object rhs) => rhs == null ? lhs is null : lhs.Equals(rhs);
        public static bool operator !=(CodeType lhs, object rhs) => rhs == null ? lhs is object : lhs is null || !lhs.Equals(rhs);



        public static BoxedType Int32_Boxed = new BoxedType(typeof(int));
        public static BoxedType Int64_Boxed = new BoxedType(typeof(long));
        public static BoxedType Int8_Boxed = new BoxedType(typeof(byte));
        public static BoxedType Int16_Boxed = new BoxedType(typeof(char));
        public static BoxedType Bool_Boxed = new BoxedType(typeof(bool));
        public static BoxedType Real4_Boxed = new BoxedType(typeof(float));
        public static BoxedType Real8_Boxed = new BoxedType(typeof(double));


        //TODO: Add pointers
        //Define basic CLI types
        public static BasicType Int32 = new BasicType("I4", 1, Int32_Boxed, "int32", "int");
        public static BasicType Int64 = new BasicType("I8", 2, Int64_Boxed, "int64", "long");
        public static BasicType NativeInt = new BasicType("I", 3, "native");
        public static BasicType Real = new BasicType(4, "real");
        public static CodeType Object = UserType.NewUserType(typeof(object));



        //Define derived types
        public static DerivedType Int8 = new DerivedType(.5f, Int32, Int8_Boxed, "byte", "int8");
        public static DerivedType Bool = new DerivedType(.5f, Int32, Bool_Boxed, "bool");
        public static DerivedType Int16 = new DerivedType(.7f, Int32, Int16_Boxed, "short", "int16", "char");
        public static DerivedType Real4 = new DerivedType("R4", 4, Real, Real4_Boxed, "float32", "float", "real4");
        public static DerivedType Real8 = new DerivedType("R8", 4.5f, Real, Real8_Boxed, "float64", "double", "decimal", "real8");

        public static StringType String = new StringType();
        public static VoidType Void = new VoidType();

    }
}
