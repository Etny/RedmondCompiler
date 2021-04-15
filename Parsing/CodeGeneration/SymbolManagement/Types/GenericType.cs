using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class GenericType : UserType
    {
        public readonly CodeType[] GenericParameters;
        public int Arity => GenericParameters.Length;


        public static UserType NewGenericType(UserType type, CodeType[] parameters)
        {
            if (type is InterUserType)
                return new InterGenericType((type as InterUserType).GetInterType(), parameters);
            else
                return new GenericType(type, parameters);
        }

        protected GenericType(params CodeType[] parameters)
        {
            GenericParameters = parameters;
        }

        protected GenericType(UserType type, params CodeType[] parameters) : this(parameters)
        {
            _type = type.GetNativeType();

            string paramsNames = "";

            foreach (var p in parameters)
                paramsNames += p.Name + ',';

            paramsNames = paramsNames[0..^1];
            string addition = $"<{paramsNames}>";

            Name = type.Name + addition;
            ShortName = type.ShortName + addition;
            StoredType = this;
        }

        public override IEnumerable<IMethodWrapper> GetConstructors(IntermediateBuilder context)
        {
            foreach (var f in _type.GetConstructors())
                yield return new GenericConstructorInfoWrapper(f, context, this);
        }

        public override IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _type.GetMethods())
                yield return new GenericMethodInfoWrapper(f, context, this);
        }

        public override IEnumerable<IPropertyWrapper> GetIndexers(IntermediateBuilder context)
        {
            foreach (var f in _type.GetProperties())
            {
                if (f.GetIndexParameters().Length > 0)
                    yield return new GenericPropertyInfoWrapper(f, context, this);
            }
        }

        public override IEnumerable<IPropertyWrapper> GetProperties(IntermediateBuilder context)
        {
            foreach (var f in _type.GetProperties())
                yield return new GenericPropertyInfoWrapper(f, context, this);
        }

    }

    class InterGenericType : InterUserType
    {

        public readonly CodeType[] GenericParameters;
        public int Arity => GenericParameters.Length;

        public InterGenericType(InterType interType, params CodeType[] parameters) : base(interType)
        {
            _intertype = interType;
            GenericParameters = parameters;
            string paramsNames = "";

            foreach (var p in parameters)
                paramsNames += p.Name + ',';

            paramsNames = paramsNames[0..^1];
            string addition = $"<{paramsNames}>";

            Name += addition;
            ShortName += addition;
            StoredType = this;
        }

        public override IEnumerable<IMethodWrapper> GetConstructors(IntermediateBuilder context)
        {
            foreach (var f in _intertype.Constructors)
                yield return new GenericInterMethodWrapper(f, context, this);
        }

        public override IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _intertype.Methods)
                yield return new GenericInterMethodWrapper(f, context, this);
        }

        //public override IEnumerable<IPropertyWrapper> GetIndexers(IntermediateBuilder context)
        //{
        //    foreach (var f in _type.GetProperties())
        //    {
        //        if (f.GetIndexParameters().Length > 0)
        //            yield return new GenericPropertyInfoWrapper(f, context, this);
        //    }
        //}

        //public override IEnumerable<IPropertyWrapper> GetProperties(IntermediateBuilder context)
        //{
        //    foreach (var f in _type.GetProperties())
        //        yield return new GenericPropertyInfoWrapper(f, context, this);
        //}

    }

    class GenericParameterType : CodeType
    {
        private CodeType _type;
        public readonly int Index;

        public GenericParameterType(CodeType baseType, int index) : base("")
        {
            _type = baseType;
            Index = index;

            Name = "!" + index;
            ShortName = "!" + index;
            StoredType = baseType;
        }

        public override AssignType CanAssignTo(CodeType fieldType)
        {
            if(fieldType is GenericParameterType)
            {
                var other = fieldType as GenericParameterType;
                return other.Index == Index ? AssignType.CanAssign : AssignType.CannotAssign;
            }else 
                return _type.CanAssignTo(fieldType);
        }

        public override void ConvertFrom(CodeValue val, IlBuilder builder)
            => _type.ConvertFrom(val, builder);

        public override CodeType GetWiderType(CodeType otherType)
            => _type.GetWiderType(otherType);
    }
}
