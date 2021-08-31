grammar Lillisp;

prog: expr* EOF;

expr: list | atom | macro;

list: LPAREN expr* RPAREN;

atom: (NUMBER | SYMBOL);

macro: quote;

quote: QUOTE expr;

SYMBOL: OPERATOR | IDENTIFIER;

OPERATOR: SYMBOL_CHAR+;

IDENTIFIER: LETTER (LETTER | NUMBER | UNDERSCORE)*;

NUMBER: INTEGER | FLOAT | RATIO;

INTEGER: NEGATE? (DIGIT)+;
FLOAT: NEGATE? (DIGIT | '.')+;
RATIO: INTEGER '/' INTEGER;

LETTER: LOWER | UPPER;
DIGIT: '0' ..'9';
LOWER: 'a' ..'z';
UPPER: 'A' ..'Z';
SYMBOL_CHAR:
	'+'
	| '-'
	| '*'
	| '/'
	| '%'
	| '^'
	| '<'
	| '>'
	| '=';
LPAREN: '(';
RPAREN: ')';
NEGATE: '-';
UNDERSCORE: '_';
QUOTE: '\'';

WHITESPACE: [ \r\n\t]+ -> channel(HIDDEN);