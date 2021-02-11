using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class GrammarAction
    {

        private string _dec;
        private Production _prod;
        private Action<Stack<ParserStackEntry>>[] _actions;

        private static Dictionary<string, Func<Stack<ParserStackEntry>, List<GrammarActionArgument>, object>> syntaxFunctions = new Dictionary<string, Func<Stack<ParserStackEntry>, List<GrammarActionArgument>, object>>();

        public GrammarAction(string dec, Production prod)
        {
            _dec = dec;
            _prod = prod;
            Parse(dec, prod);
        }

        
        public static void Init()
        {
            var syntaxFuncs = Assembly.GetExecutingAssembly().GetTypes()
                                .SelectMany(t => t.GetMethods())
                                .Where(m => m.IsStatic && m.GetCustomAttribute(typeof(SyntaxFunctionAttribute)) != null)
                                .ToArray();

            foreach (var f in syntaxFuncs)
            {
                string name = (f.GetCustomAttribute(typeof(SyntaxFunctionAttribute)) as SyntaxFunctionAttribute).Name;
                syntaxFunctions.Add(name + f.GetParameters().Length, (s, a) => InvokeSyntaxFunction(f, s, a));
            }
        }

        private static object InvokeSyntaxFunction(MethodInfo info, Stack<ParserStackEntry> stack, List<GrammarActionArgument> args)
        {
            var paramList = args.Select(a => a.ResolveValue(stack)).ToArray();
            return info.Invoke(null, paramList);
        }

        private void Parse(string dec, Production prod)
        {
            dec = dec.Replace(" ", "").Replace("\t", "").Trim();
            int index = 0;

            List<Action<Stack<ParserStackEntry>>> actions = new List<Action<Stack<ParserStackEntry>>>();

            while (index < dec.Length)
            {
                var assign = (0, "");

                Func<Stack<ParserStackEntry>, List<GrammarActionArgument>, object> currentAction = null;

                Stack<(string, List<GrammarActionArgument>)> funcStack = new Stack<(string, List<GrammarActionArgument>)>();
                string currentFunc = "";
                List<GrammarActionArgument> args = new List<GrammarActionArgument>();

                while (index < dec.Length && dec[index] != ';')
                {
                    char c = dec[index++];

                    if (c == '$')
                    {
                        int offset = 0;
                        string key = "val";
                        if (dec[index] != '$')
                        {
                            string s = dec.ReadWhile(ref index, c => "-0123456789".Contains(c));
                            offset = int.Parse(s);
                            if (offset < 0) offset = prod.Rhs.Length - offset;
                            else if(offset > 0) offset = prod.Rhs.Length - (offset - 1);
                        }
                        else index++;

                        if (dec.MatchNext('.', ref index))
                            key = dec.ReadUntil(ref index, c => "=.),;".Contains(c));

                        if (dec.MatchNext('=', ref index))
                        {
                            assign = (offset, key);
                            continue;
                        }

                        args.Add(new GrammarActionArgument(offset, key));

                    }
                    else if ("-0123456789".Contains(c))
                    {
                        int val = int.Parse(c + dec.ReadWhile(ref index, c => "-0123456789".Contains(c)));
                        args.Add(new GrammarActionArgument(val));
                    }
                    else if (c == '"')
                    {
                        string s = dec.ReadUntil(ref index, c => c == '"');
                        index++;
                        args.Add(new GrammarActionArgument(s));
                    }
                    else if (!" ),;\t".Contains(c))
                    {
                        string func = c + dec.ReadUntil(ref index, c => c == '(');
                        index++;

                        if (currentFunc != "") funcStack.Push((currentFunc, args));
                        args = new List<GrammarActionArgument>();
                        currentFunc = func;
                        continue;
                    }

                    if (dec.MatchNext(',', ref index))
                        continue;

                    if (c == ')' || dec.MatchNext(')', ref index))
                    {
                        currentAction = (s, a) => syntaxFunctions[currentFunc+args.Count].Invoke(s, a);

                        if (funcStack.Count > 0)
                        {
                            var oldArgs = args;
                            var newFunc = funcStack.Pop();
                            args = newFunc.Item2;
                            string str = new string(currentFunc);
                            args.Add(new GrammarActionArgument(s => syntaxFunctions[str + "" + oldArgs.Count].Invoke(s, oldArgs)));
                            currentFunc = newFunc.Item1; 
                        }
                    }
                }

                if (assign.Item2 != "")
                {
                    if (currentFunc != "")
                    {
                        var oldArgs = args;
                        args = new List<GrammarActionArgument> { new GrammarActionArgument(s => syntaxFunctions[currentFunc+oldArgs.Count].Invoke(s, oldArgs)) };
                    }

                    currentAction = (s, a) => Assign(s, a, assign.Item1, assign.Item2);
                }

                actions.Add((s) => currentAction(s, args));
                index++;
            }

            _actions = actions.ToArray();
        }

        private object Assign(Stack<ParserStackEntry> stack, List<GrammarActionArgument> args, int offset, string key)
        {
            object o = args[0].ResolveValue(stack);
            stack.ToArray()[offset].UpdateAttribute(key, o);
            return null;
        }


        public void Invoke(Stack<ParserStackEntry> parseStack)
        {
            foreach(var _action in _actions)
                _action.Invoke(parseStack);
        }


        struct GrammarActionArgument
        {
            private object _val;
            private string _key;

            GrammarActionArgumentType Type;

            public GrammarActionArgument(int val, string key)
            {
                _val = val;
                _key = key;
                Type = GrammarActionArgumentType.Stack;
            }

            public GrammarActionArgument(object val)
            {
                _val = val;
                _key = "";
                Type = GrammarActionArgumentType.Literal;
            }

            public GrammarActionArgument(Func<Stack<ParserStackEntry>, object> func)
            {
                _val = func;
                _key = "";
                Type = GrammarActionArgumentType.Function;
            }

            public int Offset => (int)_val;
            public string Key => _key;
            
            public object Value => _val;
            public Func<Stack<ParserStackEntry>, object> Function => _val as Func<Stack<ParserStackEntry>, object>;

            public object ResolveValue(Stack<ParserStackEntry> parseStack)
            {
                return Type switch
                {
                    GrammarActionArgumentType.Literal => _val,
                    GrammarActionArgumentType.Function => (_val as Func<Stack<ParserStackEntry>, object>).Invoke(parseStack),
                    GrammarActionArgumentType.Stack => parseStack.ToArray()[Offset].GetAttribute(Key),
                    _ => 1,
                };
            }

        }

        enum GrammarActionArgumentType
        {
            Stack, Literal, Function
        }
    }
}
