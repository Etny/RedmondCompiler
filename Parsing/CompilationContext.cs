﻿using Redmond.Lex;
using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing
{
    class CompilationContext
    {

        public readonly TokenStream Input;
        public readonly IStringStream Output;

        public readonly Compiler Compiler;

        public CompilationContext(TokenStream input, IStringStream output)
        {
            Input = input;
            Output = output;

            Compiler = new Compiler(input, output, this);
        }

        public void Start()
            => Compiler.StartCompilation();

    }
}