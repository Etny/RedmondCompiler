#Lex

letter = [a-z]
digit = [0-9]
number = digit+
char = letter|digit
binop = [+\-*/]

%%

if/ab\wb				{Punctuation}
(letter|_)char* {Identifier}
number			{NumLiteral} //Hey
\w+				{Whitespace}
number binop number {Expression}

#End