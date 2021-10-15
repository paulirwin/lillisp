grammar Lillisp;

prog: form * EOF;

form: atom | list | vector | meta;

list: '(' form* ')';

vector: '[' form* ']' | '#(' form* ')';

atom: (NUMBER | STRING | SYMBOL | CHARACTER);

meta: quote | quasiquote | unquote;

quote: '\'' form;

quasiquote: '`' form;

unquote: ',' form;

CHARACTER: '#\\' (LETTER | NUMBER | SYMBOL_CHAR)+;

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
	| '$'
	| '.'
	| ':'
	| '?'
	| '@'
	| '~'
	| '_';
LPAREN: '(';
RPAREN: ')';
LBRACKET: '[';
RBRACKET: ']';
NEGATE: '-';
UNDERSCORE: '_';
QUOTE: '\'';
DQUOTE: '"';
COMMA: ',';
BACKTICK: '`';
ATSIGN: '@';
DOT: '.';
HASH: '#';

WHITESPACE: [ \r\n\t]+ -> channel(HIDDEN);