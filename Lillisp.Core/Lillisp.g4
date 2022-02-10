grammar Lillisp;

prog: form * EOF;

form: regex | atom | list | bytevector | vector | meta;

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

regex: REGEX_PATTERN IDENTIFIER?;

symbol: DOT_LITERAL | ELLIPSIS_LITERAL | UNDERSCORE_LITERAL | BOOLEAN | IDENTIFIER | ESCAPED_IDENTIFIER;

POS_INFINITY: '+inf.0';
NEG_INFINITY: '-inf.0';
NAN: '+nan.0' | '-nan.0';
BOOLEAN: '#t' | '#f';

DOT_LITERAL: '.';
ELLIPSIS_LITERAL: '...';
UNDERSCORE_LITERAL: '_';

HEX_PREFIXED_NUMBER: HEX_PREFIX (HEX_DIGIT | UNDERSCORE)+;
OCTAL_PREFIXED_NUMBER: OCTAL_PREFIX (OCTAL_DIGIT | UNDERSCORE)+;
BINARY_PREFIXED_NUMBER: BINARY_PREFIX (BINARY_DIGIT | UNDERSCORE)+;

INTEGER: NEGATE? (DIGIT | UNDERSCORE)+;
COMPLEX: ((NEGATE? (DIGIT | UNDERSCORE | '.')+) | POS_INFINITY | NEG_INFINITY | NAN) ('+' | '-') ((DIGIT | UNDERSCORE | '.')+ | 'inf.0' | 'nan.0') 'i';
FLOAT: NEGATE? (DIGIT | UNDERSCORE | '.')+ ('e' '-'? (DIGIT | UNDERSCORE | '.')+)?;
RATIO: NEGATE? (DIGIT | UNDERSCORE)+ '/' (DIGIT | UNDERSCORE)+;

CHARACTER: '#\\' ((LETTER | DIGIT | SYMBOL_CHAR)* | '(' | ')');

REGEX_PATTERN: '/' ( ~('/' | ' ') | '\\' '/' | '\\' ' ' )* '/' REGEX_FLAGS;

ESCAPED_IDENTIFIER: '|' ( ~'|' | '\\' '|' )* '|';

IDENTIFIER: IDENTIFIER_START IDENTIFIER_PART*;

STRING : '"' ( ~'"' | '\\' '"' )* '"' ;

fragment REGEX_FLAGS: IDENTIFIER_PART*;

fragment IDENTIFIER_START: (LETTER | SYMBOL_CHAR);
fragment IDENTIFIER_PART: (LETTER | DIGIT | SYMBOL_CHAR);

fragment HEX_PREFIX: '#x';
fragment OCTAL_PREFIX: '#o';
fragment BINARY_PREFIX: '#b';
fragment DECIMAL_PREFIX: '#de' | '#di' | '#ed' | '#id' | '#d' | '#e' | '#i';

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