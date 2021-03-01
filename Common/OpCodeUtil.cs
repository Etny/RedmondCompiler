using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Common
{
    static class OpCodeUtil
    {


        public static OpCode GetOpcode(string name)
        {
            foreach (var f in typeof(OpCodes).GetFields())
            {
                if (f.Name == name)
                    return (OpCode)f.GetValue(null);
            }
            return OpCodes.Nop;
        }

        public static bool HasVariableStackBehaviour(this OpCode op)
            => op.StackBehaviourPush == StackBehaviour.Varpush || op.StackBehaviourPop == StackBehaviour.Varpop;

        public static int NetStackCount(this OpCode op)
            => PushCount(op) - PopCount(op);

        public static int PushCount(this OpCode op)
        {
            switch (op.StackBehaviourPush)
            {
                default:
                case StackBehaviour.Push0:
                    return 0;

                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                case StackBehaviour.Push1:
                    return 1;

                case StackBehaviour.Push1_push1:
                    return 2;
            }
        }

        public static int PopCount(this OpCode op)
        {
            switch (op.StackBehaviourPop)
            {
                default:
                case StackBehaviour.Pop0:
                    return 0;

                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                    return 1;

                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    return 2;

                case StackBehaviour.Popref_popi_pop1:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                case StackBehaviour.Popi_popi_popi:
                    return 3;
            }
        }

    }
}
