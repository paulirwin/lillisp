grammar Lillisp;

prog: form * EOF;

form: atom | list | bytevector | vector | meta;

atom: (number | symbol | STRING | CHARACTER);

list: '(' form* ')';

bytevector: '#u8(' integer* ')';

vector: '[' form* ']' | '#(' form* ')';

meta: quote | quasiquote | unquote_splicing | unquote | comment_datum;

integer: INTEGER;

quote: '\'' form;

quasiquote: '`' form;

unquote_splicing: ',@' form;

unquote: ',' form;

comment_datum: '#;' form;

number: prefixed_number | INTEGER | FLOAT | COMPLEX | RATIO | POS_INFINITY | NEG_INFINITY | NAN;

prefixed_number: hex_prefixed | decimal_prefixed | octal_prefixed | binary_prefixed;

hex_prefixed: HEX_PREFIXED_NUMBER;

octal_prefixed: OCTAL_PREFIXED_NUMBER;

binary_prefixed: BINARY_PREFIXED_NUMBER;

decimal_prefixed: DECIMAL_PREFIX (INTEGER | FLOAT);

symbol: DOT_LITERAL | BOOLEAN | IDENTIFIER | ESCAPED_IDENTIFIER;

POS_INFINITY: '+inf.0';
NEG_INFINITY: '-inf.0';
NAN: '+nan.0' | '-nan.0';

DOT_LITERAL: '.';

HEX_PREFIXED_NUMBER: HEX_PREFIX (HEX_DIGIT | UNDERSCORE)+;
OCTAL_PREFIXED_NUMBER: OCTAL_PREFIX (OCTAL_DIGIT | UNDERSCORE)+;
BINARY_PREFIXED_NUMBER: BINARY_PREFIX (BINARY_DIGIT | UNDERSCORE)+;

INTEGER: NEGATE? (DIGIT | UNDERSCORE)+;
COMPLEX: ((NEGATE? (DIGIT | UNDERSCORE | '.')+) | POS_INFINITY | NEG_INFINITY | NAN) ('+' | '-') ((DIGIT | UNDERSCORE | '.')+ | 'inf.0' | 'nan.0') 'i';
FLOAT: NEGATE? (DIGIT | UNDERSCORE | '.')+ ('e' '-'? (DIGIT | UNDERSCORE | '.')+)?;
RATIO: NEGATE? (DIGIT | UNDERSCORE)+ '/' (DIGIT | UNDERSCORE)+;

IDENTIFIER: (LETTER | SYMBOL_CHAR) (LETTER | DIGIT | SYMBOL_CHAR)*;

CHARACTER: '#\\' ((LETTER | DIGIT | SYMBOL_CHAR)* | '(' | ')');

STRING : '"' ( ~'"' | '\\' '"' )* '"' ;

HEX_PREFIX: '#x';
OCTAL_PREFIX: '#o';
BINARY_PREFIX: '#b';
DECIMAL_PREFIX: '#de' | '#di' | '#ed' | '#id' | '#d' | '#e' | '#i';

ESCAPED_IDENTIFIER: '|' ( ~'|' | '\\' '|' )* '|';

BOOLEAN: '#t' | '#f';

fragment HEX_DIGIT: '0'..'9' | 'a'..'f' | 'A'..'F';
fragment DIGIT: '0'..'9';
fragment OCTAL_DIGIT: '0'..'7';
fragment BINARY_DIGIT: '0' | '1';

fragment LETTER: LOWER | UPPER;
fragment LOWER: 'a'..'z';
fragment UPPER: 'A'..'Z';

fragment SYMBOL_CHAR:
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
fragment LPAREN: '(';
fragment RPAREN: ')';
fragment LBRACKET: '[';
fragment RBRACKET: ']';
fragment NEGATE: '-';
fragment UNDERSCORE: '_';
fragment QUOTE: '\'';
fragment DQUOTE: '"';
fragment COMMA: ',';
fragment BACKTICK: '`';
fragment ATSIGN: '@';
fragment DOT: '.';
fragment HASH: '#';

BLOCK_COMMENT: '#|' .*? '|#' -> skip;

LINE_COMMENT: ';' ~[\r\n]* -> skip;

WHITESPACE: [ \r\n\t]+ -> channel(HIDDEN);