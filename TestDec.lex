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

int {Letter}
float {Letter}
number {Digit}
[=\*+-] {Operator}
$ {Punctuation}

#End

#Grammar

left + -
left * /
right UMINUS

%%

Dec: Type Id;
Type: int {int} | float {float};
Id: 1 {print};

//Stmnts:	Left = Right | Right; 
//Left:	* Right | d;
//Right:	Left;

#End