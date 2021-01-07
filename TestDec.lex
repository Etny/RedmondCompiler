

letter = [a-z]
number = [0-9]
char = letter|number

%%

(letter|_)char* {Identifier}
number+			{NumLiteral} //Hey
