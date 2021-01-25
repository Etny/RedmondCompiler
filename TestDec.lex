#Tokens

Letter
Digit
Operator
Punctuation

#End

#Lex

letter = [a-z]
number = [0-9]

%%

letter {Letter}
number {Digit}
[=\*+-] {Operator}
$ {Punctuation}

#End

#Grammar

left + -
left * /
right UMINUS

%%

Stmnts: Expr;
Expr: Expr + Expr | Expr * Expr | ( Expr ) | - Expr %prec:UMINUS | 1;

//Stmnts:	Left = Right | Right;
//Left:	* Right | d;
//Right:	Left;

#End