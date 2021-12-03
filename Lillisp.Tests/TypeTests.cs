using System;
using Xunit;

namespace Lillisp.Tests
{
    public class TypeTests
    {
        [InlineData("(typeof \"foo\")", typeof(string))]
        [InlineData("(typeof 1)", typeof(int))]
        [Theory]
        public void TypeofTests(string input, Type expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(boolean? #t)", true)]
        [InlineData("(boolean? #f)", true)]
        [InlineData("(boolean? 0)", false)]
        [InlineData("(boolean? #\\a)", false)]
        [InlineData("(boolean? \"cat\")", false)]
        [InlineData("(boolean? (lambda (x) x))", false)]
        [InlineData("(boolean? '(1 2 3))", false)]
        [InlineData("(boolean? [1 2 3])", false)]
        [InlineData("(boolean? '())", false)]
        [InlineData("(boolean? 'car)", false)]
        [InlineData("(boolean? #u8(1 2 3))", false)]
        [InlineData("(boolean? (eof-object))", false)]
        [InlineData("(boolean? (current-output-port))", false)]
        [Theory]
        public void BooleanCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(char? #t)", false)]
        [InlineData("(char? #f)", false)]
        [InlineData("(char? 0)", false)]
        [InlineData("(char? #\\a)", true)]
        [InlineData("(char? \"cat\")", false)]
        [InlineData("(char? (lambda (x) x))", false)]
        [InlineData("(char? '(1 2 3))", false)]
        [InlineData("(char? [1 2 3])", false)]
        [InlineData("(char? '())", false)]
        [InlineData("(char? 'car)", false)]
        [InlineData("(char? #u8(1 2 3))", false)]
        [InlineData("(char? (eof-object))", false)]
        [InlineData("(char? (current-output-port))", false)]
        [Theory]
        public void CharCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(null? #t)", false)]
        [InlineData("(null? #f)", false)]
        [InlineData("(null? 0)", false)]
        [InlineData("(null? #\\a)", false)]
        [InlineData("(null? \"cat\")", false)]
        [InlineData("(null? (lambda (x) x))", false)]
        [InlineData("(null? '(1 2 3))", false)]
        [InlineData("(null? [1 2 3])", false)]
        [InlineData("(null? '())", true)]
        [InlineData("(null? 'car)", false)]
        [InlineData("(null? #u8(1 2 3))", false)]
        [InlineData("(null? (eof-object))", false)]
        [InlineData("(null? (current-output-port))", false)]
        [Theory]
        public void NullCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(number? #t)", false)]
        [InlineData("(number? #f)", false)]
        [InlineData("(number? 0)", true)]
        [InlineData("(number? #\\a)", false)]
        [InlineData("(number? \"cat\")", false)]
        [InlineData("(number? (lambda (x) x))", false)]
        [InlineData("(number? '(1 2 3))", false)]
        [InlineData("(number? [1 2 3])", false)]
        [InlineData("(number? '())", false)]
        [InlineData("(number? 'car)", false)]
        [InlineData("(number? #u8(1 2 3))", false)]
        [InlineData("(number? (eof-object))", false)]
        [InlineData("(number? (current-output-port))", false)]
        [Theory]
        public void NumberCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(string? #t)", false)]
        [InlineData("(string? #f)", false)]
        [InlineData("(string? 0)", false)]
        [InlineData("(string? #\\a)", false)]
        [InlineData("(string? \"cat\")", true)]
        [InlineData("(string? (lambda (x) x))", false)]
        [InlineData("(string? '(1 2 3))", false)]
        [InlineData("(string? [1 2 3])", false)]
        [InlineData("(string? '())", false)]
        [InlineData("(string? 'car)", false)]
        [InlineData("(string? #u8(1 2 3))", false)]
        [InlineData("(string? (eof-object))", false)]
        [InlineData("(string? (current-output-port))", false)]
        [Theory]
        public void StringCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(pair? #t)", false)]
        [InlineData("(pair? #f)", false)]
        [InlineData("(pair? 0)", false)]
        [InlineData("(pair? #\\a)", false)]
        [InlineData("(pair? \"cat\")", false)]
        [InlineData("(pair? (lambda (x) x))", false)]
        [InlineData("(pair? '(1 2 3))", true)]
        [InlineData("(pair? [1 2 3])", false)]
        [InlineData("(pair? '())", false)]
        [InlineData("(pair? 'car)", false)]
        [InlineData("(pair? #u8(1 2 3))", false)]
        [InlineData("(pair? (eof-object))", false)]
        [InlineData("(pair? (current-output-port))", false)]
        [Theory]
        public void PairCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(procedure? #t)", false)]
        [InlineData("(procedure? #f)", false)]
        [InlineData("(procedure? 0)", false)]
        [InlineData("(procedure? #\\a)", false)]
        [InlineData("(procedure? \"cat\")", false)]
        [InlineData("(procedure? (lambda (x) x))", true)]
        [InlineData("(procedure? '(1 2 3))", false)]
        [InlineData("(procedure? [1 2 3])", false)]
        [InlineData("(procedure? '())", false)]
        [InlineData("(procedure? 'car)", false)]
        [InlineData("(procedure? #u8(1 2 3))", false)]
        [InlineData("(procedure? (eof-object))", false)]
        [InlineData("(procedure? (current-output-port))", false)]
        [Theory]
        public void ProcedureCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(symbol? #t)", false)]
        [InlineData("(symbol? #f)", false)]
        [InlineData("(symbol? 0)", false)]
        [InlineData("(symbol? #\\a)", false)]
        [InlineData("(symbol? \"cat\")", false)]
        [InlineData("(symbol? (lambda (x) x))", false)]
        [InlineData("(symbol? '(1 2 3))", false)]
        [InlineData("(symbol? [1 2 3])", false)]
        [InlineData("(symbol? '())", false)]
        [InlineData("(symbol? 'car)", true)]
        [InlineData("(symbol? #u8(1 2 3))", false)]
        [InlineData("(symbol? (eof-object))", false)]
        [InlineData("(symbol? (current-output-port))", false)]
        [Theory]
        public void SymbolCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(vector? #t)", false)]
        [InlineData("(vector? #f)", false)]
        [InlineData("(vector? 0)", false)]
        [InlineData("(vector? #\\a)", false)]
        [InlineData("(vector? \"cat\")", false)]
        [InlineData("(vector? (lambda (x) x))", false)]
        [InlineData("(vector? '(1 2 3))", false)]
        [InlineData("(vector? [1 2 3])", true)]
        [InlineData("(vector? '())", false)]
        [InlineData("(vector? 'car)", false)]
        [InlineData("(vector? #u8(1 2 3))", false)]
        [InlineData("(vector? (eof-object))", false)]
        [InlineData("(vector? (current-output-port))", false)]
        [Theory]
        public void VectorCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(bytevector? #t)", false)]
        [InlineData("(bytevector? #f)", false)]
        [InlineData("(bytevector? 0)", false)]
        [InlineData("(bytevector? #\\a)", false)]
        [InlineData("(bytevector? \"cat\")", false)]
        [InlineData("(bytevector? (lambda (x) x))", false)]
        [InlineData("(bytevector? '(1 2 3))", false)]
        [InlineData("(bytevector? [1 2 3])", false)]
        [InlineData("(bytevector? '())", false)]
        [InlineData("(bytevector? 'car)", false)]
        [InlineData("(bytevector? #u8(1 2 3))", true)]
        [InlineData("(bytevector? (eof-object))", false)]
        [InlineData("(bytevector? (current-output-port))", false)]
        [Theory]
        public void BytevectorCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(eof-object? #t)", false)]
        [InlineData("(eof-object? #f)", false)]
        [InlineData("(eof-object? 0)", false)]
        [InlineData("(eof-object? #\\a)", false)]
        [InlineData("(eof-object? \"cat\")", false)]
        [InlineData("(eof-object? (lambda (x) x))", false)]
        [InlineData("(eof-object? '(1 2 3))", false)]
        [InlineData("(eof-object? [1 2 3])", false)]
        [InlineData("(eof-object? '())", false)]
        [InlineData("(eof-object? 'car)", false)]
        [InlineData("(eof-object? #u8(1 2 3))", false)]
        [InlineData("(eof-object? (eof-object))", true)]
        [InlineData("(eof-object? (current-output-port))", false)]
        [Theory]
        public void EofObjectCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(port? #t)", false)]
        [InlineData("(port? #f)", false)]
        [InlineData("(port? 0)", false)]
        [InlineData("(port? #\\a)", false)]
        [InlineData("(port? \"cat\")", false)]
        [InlineData("(port? (lambda (x) x))", false)]
        [InlineData("(port? '(1 2 3))", false)]
        [InlineData("(port? [1 2 3])", false)]
        [InlineData("(port? '())", false)]
        [InlineData("(port? 'car)", false)]
        [InlineData("(port? #u8(1 2 3))", false)]
        [InlineData("(port? (eof-object))", false)]
        [InlineData("(port? (current-output-port))", true)]
        [Theory]
        public void PortCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(promise? (delay 3))", true)]
        [InlineData("(promise? (force (delay 3)))", false)]
        [Theory]
        public void PromiseCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(char->integer #\\=)", 61)]
        [InlineData("(integer->char 61)", '=')]
        [InlineData("(string->list \"foo\")", new[] { 'f', 'o', 'o' })]
        [InlineData("(list->string (list #\\a #\\b #\\c))", "abc")]
        [Theory]
        public void TypeConversionTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        // R7RS 6.2.6
        // NOTE on commented-out tests: waiting on this to be fixed: https://github.com/xunit/visualstudio.xunit/issues/266
        [InlineData("(complex? 3+4i)", true)]
        [InlineData("(complex? 3)", true)]
        [InlineData("(real? 3)", true)]
        //[InlineData("(real? -2.5+0i)", true, Skip = "Would require either an exact complex type, or fail the test below by considering an imaginary part of 0 real")]
        [InlineData("(real? -2.5+0.0i)", false)]
        //[InlineData("(real? #e1e10)", true, Skip = "Exp notation not yet supported")]
        [InlineData("(real? +inf.0)", true)]
        [InlineData("(real? +nan.0)", true)]
        [InlineData("(rational? -inf.0)", false)]
        //[InlineData("(rational? 3.5)", true, Skip = "Rationals (and therefore, their conversion to/from decimal) not yet supported")]
        [InlineData("(rational? 6/10)", true, Skip = "Rationals not yet supported")]
        [InlineData("(rational? 6/3)", true, Skip = "Rationals not yet supported")]
        //[InlineData("(integer? 3+0i)", true, Skip = "Would require an exact complex type, or failing the '(real? -2.5+0.0i)' test above")]
        //[InlineData("(integer? 3.0)", true, Skip = "Exact real literals not yet supported")]
        //[InlineData("(integer? 8/4)", true, Skip = "Rationals not yet supported")]
        [Theory]
        public void NumberTypeCheckTests(string input, bool expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(utf8->string #u8(65))", "A")]
        [InlineData("(bytevector-length (string->utf8 \"λ\"))", 2)]
        [InlineData("(bytevector-u8-ref (string->utf8 \"λ\") 0)", (byte)0xCE)]
        [InlineData("(bytevector-u8-ref (string->utf8 \"λ\") 1)", (byte)0xBB)]
        [Theory]
        public void Utf8StringConversionTetss(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}