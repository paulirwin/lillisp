grammar Lillisp;

prog: expr* EOF;

expr: list | atom | macro;

list: LPAREN expr* RPAREN;

atom: (NUMBER | STRING | SYMBOL);

macro: quote;

quote: QUOTE expr;

STRING : '"' ( ~'"' | '\\' '"' )* '"' ;

SYMBOL: OPERATOR | IDENTIFIER;

OPERATOR: SYMBOL_CHAR+;

IDENTIFIER: (LETTER | DOT | HASH) (
		LETTER
		| NUMBER
		| SYMBOL_CHAR
	)*;

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
	| '='
	| '!'
	| '&'
	| '|'
	| '['
	| ']'
	| '$'
	| '.'
	| ':'
	| '?'
	| '@'
	| '~'
	| '_';
LPAREN: '(';
RPAREN: ')';
NEGATE: '-';
UNDERSCORE: '_';
QUOTE: '\'';
DQUOTE: '\"';
DOT: '.';
HASH: '#';

WHITESPACE: [ \r\n\t]+ -> channel(HIDDEN);