﻿using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    struct InterInstOperand
    {

        public CodeValue Value { get; private set; }
        public IInterOp Op { get; private set; }

        public bool IsValue { get => Op == null; }

        public InterInstOperand(object o)
        {
            if(o is IInterOp)
            {
                Op = o as IInterOp;
                Value = null;
            }
            else
            {
                Value = o as CodeValue;
                Op = null;
            }
        }

        public CodeType Type
        {
            get
            {
                if (IsValue)
                    return Value.Type;
                else
                    return Op.GetResultType();
            }
        }

    }
}
