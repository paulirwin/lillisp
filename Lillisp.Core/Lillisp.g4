grammar Lillisp;

prog: form * EOF;

form: atom | list | vector | meta;

atom: (number | STRING | SYMBOL | CHARACTER);

list: '(' form* ')';

vector: '[' form* ']' | '#(' form* ')';

meta: quote | quasiquote | unquote;

quote: '\'' form;

quasiquote: '`' form;

unquote: ',' form;

CHARACTER: '#\\' ((LETTER | DIGIT | SYMBOL_CHAR)* | '(' | ')');

STRING : '"' ( ~'"' | '\\' '"' )* '"' ;

SYMBOL: OPERATOR | IDENTIFIER;

OPERATOR: SYMBOL_CHAR+;

IDENTIFIER: (LETTER | DOT | HASH) (
		LETTER
		| DIGIT
		| SYMBOL_CHAR
	)*;

number: INTEGER | COMPLEX | FLOAT | RATIO;

INTEGER: NEGATE? (DIGIT)+;
FLOAT: NEGATE? (DIGIT | '.')+;
RATIO: INTEGER '/' INTEGER;
COMPLEX: NEGATE? (DIGIT | '.')+ ('+' | '-') (DIGIT | '.')+ 'i';

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