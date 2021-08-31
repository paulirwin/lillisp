grammar Lillisp;

prog: expr* EOF ;

expr: LPAREN (atom | expr)* RPAREN ;

atom: NUMBER | IDENTIFIER | OPERATOR ;

OPERATOR: SYMBOL* ;

IDENTIFIER: LETTER (LETTER | NUMBER | UNDERSCORE)* ;

NUMBER: INTEGER | FLOAT | RATIO ;

INTEGER: (DIGIT)+ ;
FLOAT: (DIGIT | '.')+ ;
RATIO: INTEGER '/' INTEGER;

LETTER: LOWER | UPPER ;
DIGIT: '0'..'9' ;
LOWER: 'a'..'z' ;
UPPER: 'A'..'Z' ;
SYMBOL: '+' | '-' | '*' | '/' | '%' | '^' | '<' | '>' | '=' ;
LPAREN: '(' ;
RPAREN: ')' ;
UNDERSCORE: '_' ;

WHITESPACE: [ \r\n\t]+ -> channel(HIDDEN) ;