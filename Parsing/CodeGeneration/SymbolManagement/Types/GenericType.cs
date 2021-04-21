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

        public readonly UserType UninstantiatedType;

        public override string ArgumentName => _argumentName;
        private string _argumentName;

        public static UserType NewGenericType(UserType type, CodeType[] parameters)
        {
            if (type is InterUserType)
                return new InterGenericType((type as InterUserType).GetInterType(), parameters);
            else
                return new GenericType(type, parameters);
        }

        public CodeType InstantiateGenericType(Type t, IntermediateBuilder context)
        {
            if (!t.IsGenericType)
            {
                if (!t.IsArray)
                {
                    if (!t.IsGenericParameter)
                        return context.ToCodeType(t).StoredType;
                    else
                        return new GenericParameterType(GenericParameters[t.GenericParameterPosition], t.GenericParameterPosition);
                }
                else
                    return new ArrayType(InstantiateGenericType(t.GetElementType(), context));
            }
            else
            {
                if (!t.ContainsGenericParameters)
                    return context.ToCodeType(t).StoredType;
                else
                {
                    var g = t.GetGenericArguments();
                    CodeType[] _argTypes = new CodeType[g.Length];

                    for (int i = 0; i < _argTypes.Length; i++)
                        _argTypes[i] = InstantiateGenericType(g[i], context);

                    return NewGenericType(ToUserType(context.ToCodeType(t, false)), _argTypes);
                }
            }
        }

        protected GenericType(params CodeType[] parameters)
        {
            GenericParameters = parameters;
        }


        protected GenericType(UserType type, params CodeType[] parameters) : this(parameters)
        {
            _type = type.GetNativeType();
            _valuetype = type.ValueType;
            UninstantiatedType = type;

            string paramsNames = "";

            foreach (var p in parameters)
                paramsNames += p.ArgumentName + ',';

            paramsNames = paramsNames[0..^1];
            string addition = $"<{paramsNames}>";

            _argumentName = type.Name + addition;

            paramsNames = "";

            foreach (var p in parameters)
                paramsNames += p.Name + ',';

            paramsNames = paramsNames[0..^1];
            string fullAddition = $"<{paramsNames}>";

            Name = type.Name + fullAddition;
            ShortName = type.ShortName + fullAddition;
            StoredType = this;
        }

        public override bool IsGeneric => true;

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

        public override IEnumerable<IFieldWrapper> GetFields(IntermediateBuilder context)
        {
            foreach (var f in _type.GetFields())
                yield return new GenericFieldInfoWrapper(f, context, this);
        }

    }

    class InterGenericType : InterUserType
    {

        public readonly CodeType[] GenericParameters;
        public int Arity => GenericParameters.Length;

        public override string ArgumentName => _argumentName;
        private string _argumentName;

    
        public InterGenericType(InterType interType, params CodeType[] parameters) : base(interType)
        {
            GenericParameters = parameters;

            string paramsNames = "";

            foreach (var p in parameters)
                paramsNames += p.ArgumentName + ',';

            paramsNames = paramsNames[0..^1];
            string addition = $"<{paramsNames}>";

            _argumentName = interType.Name + addition;

            paramsNames = "";

            foreach (var p in parameters)
                paramsNames += p.Name + ',';

            paramsNames = paramsNames[0..^1];
            string fullAddition = $"<{paramsNames}>";

            Name = $"class {interType.FullName}" + fullAddition;
            ShortName = $"{interType.FullName}" + fullAddition;
            StoredType = this;
        }

        public CodeType InstantiateGenericType(CodeType t)
        {
            if (t is GenericParameterType)
                return (t as GenericParameterType).Instatiate(GenericParameters);
            else if (t is ArrayType)
                return new ArrayType(InstantiateGenericType((t as ArrayType).TypeOf));
            else if (t is GenericType)
            {
                var g = t as GenericType;
                CodeType[] _argTypes = new CodeType[g.GenericParameters.Length];

                for (int i = 0; i < _argTypes.Length; i++)
                    _argTypes[i] = InstantiateGenericType(g.GenericParameters[i]);

                return GenericType.NewGenericType(g.UninstantiatedType, _argTypes);
            }
            else if (t is InterGenericType)
            {
                var g = t as InterGenericType;
                CodeType[] _argTypes = new CodeType[g.GenericParameters.Length];

                for (int i = 0; i < _argTypes.Length; i++)
                    _argTypes[i] = InstantiateGenericType(g.GenericParameters[i]);

                return GenericType.NewGenericType(new InterUserType(g.GetInterType()), _argTypes);
            }
            else
                return t;
        }

        public override bool IsGeneric => true;

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

        public override IEnumerable<IFieldWrapper> GetFields(IntermediateBuilder context)
        {
            foreach (var f in _intertype.Fields)
                yield return new GenericInterFieldWrapper(f, this);
        }

        public override IEnumerable<IPropertyWrapper> GetProperties(IntermediateBuilder context)
        {
            foreach (var f in _intertype.Properties)
                yield return new GenericInterPropertyWrapper(f, context, this);
        }


    }

    class GenericParameterType : CodeType
    {
        private CodeType _type;
        public readonly int Index;

        public override string ArgumentName => "!" + Index;

        public GenericParameterType(CodeType baseType, int index) : base("")
        {
            _type = baseType;
            Index = index;

            Name = baseType == Void ? ArgumentName : baseType.Name;
            ShortName = baseType == Void ? ArgumentName : baseType.ShortName;
            StoredType = baseType;
        }

        public GenericParameterType Instatiate(CodeType[] types)
            => new GenericParameterType(types[Index], Index);

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
