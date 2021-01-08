

letter = [a-z]
number = [0-9]
char = letter|number
binop = [+\-*/]

%%

if/ab\wb				{Punctuation}
(letter|_)char* {Identifier}
number+			{NumLiteral} //Hey
\w				{Whitespace}
number+ binop number+ {Expression}