using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Redmond.Lex.LexCompiler
{
    class LexAction
    {

        private string _dec;

        private Action<Token>[] _actions;

        private static Dictionary<string, Func<Token, List<LexActionArgument>, object>> lexFunctions = new Dictionary<string, Func<Token, List<LexActionArgument>, object>>();

        public LexAction(string dec)
        {
            _dec = dec;
            Parse(dec);
        }


        public static void Init()
        {
            var syntaxFuncs = Assembly.GetExecutingAssembly().GetTypes()
                                .SelectMany(t => t.GetMethods())
                                .Where(m => m.IsStatic && m.GetCustomAttribute(typeof(LexFunctionAttribute)) != null)
                                .ToArray();

            foreach (var f in syntaxFuncs)
            {
                string name = (f.GetCustomAttribute(typeof(LexFunctionAttribute)) as LexFunctionAttribute).Name;
                lexFunctions.Add(name + f.GetParameters().Length, (s, a) => InvokeLexFunction(f, s, a));
            }
        }

        private static object InvokeLexFunction(MethodInfo info,  Token t, List<LexActionArgument> args)
        {
            var paramList = args.Select(a => a.ResolveValue(t)).ToArray();
            return info.Invoke(null, paramList);
        }

        private void Parse(string dec)
        {
            dec = dec.Replace(" ", "").Replace("\t", "").Trim();
            int index = 0;

            List<Action<Token>> actions = new List<Action<Token>>();

            while (index < dec.Length)
            {
                bool assign = false;

                Func<Token, List<LexActionArgument>, object> currentAction = null;

                Stack<(string, List<LexActionArgument>)> funcStack = new Stack<(string, List<LexActionArgument>)>();
                string currentFunc = "";
                List<LexActionArgument> args = new List<LexActionArgument>();

                while (index < dec.Length && dec[index] != ';')
                {
                    char c = dec[index++];

                    if (c == '$')
                    {
                        if (index < dec.Length - 2 && dec[index] == '$' && dec[index+1] == '=')
                        {
                            index += 2;
                            assign = true;
                        }else
                            args.Add(new LexActionArgument("", true));

                    }
                    else if ("-0123456789".Contains(c))
                    {
                        int val = int.Parse(c + dec.ReadWhile(ref index, c => "-0123456789".Contains(c)));
                        args.Add(new LexActionArgument(val));
                    }
                    else if (c == '"')
                    {
                        string s = dec.ReadUntil(ref index, c => c == '"');
                        index++;
                        args.Add(new LexActionArgument(s));
                    }
                    else if (!" ),;\t".Contains(c))
                    {
                        string func = c + dec.ReadUntil(ref index, c => c == '(');
                        index++;

                        if (currentFunc != "") funcStack.Push((currentFunc, args));
                        args = new List<LexActionArgument>();
                        currentFunc = func;
                        continue;
                    }

                    if (dec.MatchNext(',', ref index))
                        continue;

                    if (c == ')' || dec.MatchNext(')', ref index))
                    {
                        currentAction = (s, a) => lexFunctions[currentFunc + args.Count].Invoke(s, a);
                        if (funcStack.Count > 0)
                        {
                            var oldArgs = args;
                            var newFunc = funcStack.Pop();
                            currentFunc = newFunc.Item1;
                            args = newFunc.Item2;
                            args.Add(new LexActionArgument(s => currentAction.Invoke(s, oldArgs)));
                        }
                    }
                }

                if (assign)
                {
                    if (currentFunc != "")
                    {
                        var oldArgs = args;
                        args = new List<LexActionArgument> { new LexActionArgument(s => lexFunctions[currentFunc + oldArgs.Count].Invoke(s, oldArgs)) };
                    }

                    currentAction = (s, a) => Assign(s, a);
                }
                

                actions.Add((s) => currentAction(s, args));
                index++;
            }

            _actions = actions.ToArray();
        }

        private object Assign(Token token, List<LexActionArgument> args)
        {
            object o = args[0].ResolveValue(token);
            token.Value = o;
            return null;
        }


        public void Invoke(Token t)
        {
            foreach (var _action in _actions)
                _action.Invoke(t);
        }


        struct LexActionArgument
        {
            private object _val;
            private string _key;

            LexActionArgumentType Type;

            public LexActionArgument(string val, bool self)
            {
                _val = val;
                _key = "";
                Type = LexActionArgumentType.Self;
            }

            public LexActionArgument(object val)
            {
                _val = val;
                _key = "";
                Type = LexActionArgumentType.Literal;
            }

            public LexActionArgument(Func<Token, object> func)
            {
                _val = func;
                _key = "";
                Type = LexActionArgumentType.Function;
            }

            public int Offset => (int)_val;
            public string Key => _key;

            public object Value => _val;
            public Func<Token, object> Function => _val as Func<Token, object>;

            public object ResolveValue(Token self)
            {
                return Type switch
                {
                    LexActionArgumentType.Literal => _val,
                    LexActionArgumentType.Function => (_val as Func<Token, object>).Invoke(self),
                    LexActionArgumentType.Self => self.Text,
                    _ => 1,
                };
            }

        }

        enum LexActionArgumentType
        {
            Self, Literal, Function
        }
    }
}
