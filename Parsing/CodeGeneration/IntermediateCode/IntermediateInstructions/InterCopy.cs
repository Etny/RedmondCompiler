using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterCopy : InterInst
    {

        private CodeSymbol _target = null;
        private CodeValue _source;
        private LateStaticReferenceResolver _resolver;

        private bool emitConv = false;

        public InterCopy(CodeSymbol target, CodeValue source)
        {
            _target = target;
            _source = source;                
        }

        public InterCopy(LateStaticReferenceResolver resolver, CodeValue source)
        {
            _resolver = resolver;
            _source = source;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _source.Bind(context);

            if (_target == null)
            {
                _resolver.Bind(context);
                Debug.Assert(_resolver.IsFieldOrProperty);
                _target = _resolver.GetReferencedFieldOrProperty();
            }
            else
                _target.Bind(context);
        }

        //TODO: Add back pre-emit type conversion
        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            if (_source.Type != _target.Type | emitConv) 
                _source = new ConvertedValue(_source, _target.Type);

            if (!Owner.IsStatic && FieldOrPropertySymbol.IsFieldOrProperty(_target))
            {
                var forp = FieldOrPropertySymbol.ToFieldOrPropertySymbol(_target);
                forp.SetOwner(Owner.ThisPointer);
                _target = forp;
            }

            _target.Store(builder, _source);
            //builder.EmitOpCode(_target.Location.GetStoreOpcode(), "For ID " + _target.ID);
        }
    }
}
