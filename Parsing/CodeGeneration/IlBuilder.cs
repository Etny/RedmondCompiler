using Redmond.Output;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using Redmond.Common;

namespace Redmond.Parsing.CodeGeneration
{
    class IlBuilder
    {
        public OutputStream Output;

        private int _currentStack = 0, _maxStack = 0;

        public IlBuilder(OutputStream output)
        {
            Output = output;
        }


        public void PushValue(CodeValue val)
        {
            val.Push(this);
        }

        public void ShrinkStack(int amount)
            => _currentStack -= amount;

        public void ExpandStack(int amount)
        {
            _currentStack += amount;
            if (_currentStack > _maxStack)
                _maxStack = _currentStack;
        }

        public int GetMaxStack()
        {
            int temp = _maxStack;
            _maxStack = 0;
            _currentStack = 0;
            return temp;
        }

        public void PopValue(CodeSymbol sym = null)
        {
            if (sym == null)
                EmitOpCode(OpCodes.Pop);
            else
                sym.Store(this);
        }

        public void EmitOpCode(OpCode opCode, params object[] operands)
        {
            EmitString(opCode.ToString());

            foreach (var s in operands)
                EmitString(" " + s);

            EmitLine("");

            if (!opCode.HasVariableStackBehaviour())
                _currentStack += opCode.NetStackCount();

            if (_currentStack > _maxStack)
                _maxStack = _currentStack;
        }

        public void EmitString(string s)
            => Output.WriteString(s);

        public void EmitLine(string s = "")
            => Output.WriteLine(s);

    }
}
