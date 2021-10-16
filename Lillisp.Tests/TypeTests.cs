﻿using System;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class TypeTests
    {
        [InlineData("(typeof \"foo\")", typeof(string))]
        [InlineData("(typeof 1)", typeof(double))]
        [Theory]
        public void TypeofTests(string input, Type expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(typeof (cast 1 Int32))", typeof(int))]
        [InlineData("(typeof (cast \"1\" Double)", typeof(double))]
        [Theory]
        public void CastTests(string input, Type expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void BooleanCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void CharCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void NullCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void NumberCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void StringCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void PairCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void ProcedureCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void SymbolCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
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
        [Theory]
        public void VectorCheckTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(char->integer #\\=)", 61)]
        [InlineData("(integer->char 61)", '=')]
        [InlineData("(string->list \"foo\")", new[] { 'f', 'o', 'o' })]
        [InlineData("(list->string (list #\\a #\\b #\\c))", "abc")]
        [Theory]
        public void TypeConversionTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
