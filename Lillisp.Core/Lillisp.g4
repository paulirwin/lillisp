grammar Lillisp;

prog: form * EOF;

form: atom | list | bytevector | vector | meta;

atom: (number | STRING | symbol | CHARACTER);

list: '(' form* ')';

bytevector: '#u8(' integer* ')';

vector: '[' form* ']' | '#(' form* ')';

meta: quote | quasiquote | unquote | comment_datum;

integer: INTEGER;

quote: '\'' form;

quasiquote: '`' form;

unquote: ',' form;

comment_datum: '#;' form;

symbol: OPERATOR | IDENTIFIER | ESCAPED_IDENTIFIER;

CHARACTER: '#\\' ((LETTER | DIGIT | SYMBOL_CHAR)* | '(' | ')');

STRING : '"' ( ~'"' | '\\' '"' )* '"' ;

OPERATOR: SYMBOL_CHAR+;

ESCAPED_IDENTIFIER: '|' ( ~'|' | '\\' '|' )* '|';

IDENTIFIER: (LETTER | DOT | HASH) (
		LETTER
		| DIGIT
		| SYMBOL_CHAR
	)*;

number: INTEGER | COMPLEX | FLOAT | RATIO | POS_INFINITY | NEG_INFINITY | NAN;

INTEGER: NEGATE? (DIGIT)+;
FLOAT: NEGATE? (DIGIT | '.')+ ('e' '-'? (DIGIT | '.')+)?;
RATIO: NEGATE? DIGIT+ '/' DIGIT+;
COMPLEX: ((NEGATE? (DIGIT | '.')+) | POS_INFINITY | NEG_INFINITY | NAN) ('+' | '-') ((DIGIT | '.')+ | 'inf.0' | 'nan.0') 'i';
POS_INFINITY: '+inf.0';
NEG_INFINITY: '-inf.0';
NAN: '+nan.0' | '-nan.0';

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

BLOCK_COMMENT: '#|' .*? '|#' -> skip;

LINE_COMMENT: ';' ~[\r\n]* -> skip;

WHITESPACE: [ \r\n\t]+ -> channel(HIDDEN);