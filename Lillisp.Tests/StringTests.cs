using System.Text;
using Lillisp.Core;
using Xunit;
// ReSharper disable StringLiteralTypo

namespace Lillisp.Tests
{
    public class StringTests
    {
        [InlineData("\"\"", "")]
        [InlineData("\"foo\"", "foo")]
        [InlineData("\"\\\"foo\\\"\"", "\"foo\"")]
        [InlineData("\"foo\\tbar\"", "foo\tbar")]
        [InlineData("\"\\\\\"", "\\")]
        [Theory]
        public void StringLiteralTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(str 100)", "100")]
        [InlineData("(str null)", null)]
        [InlineData("(str nil)", "()")]
        [InlineData("(str 'car)", "car")]
        [InlineData("(str '(1 2 3))", "(1 2 3)")]
        [InlineData("(str (new Uri \"https://www.google.com\"))", "https://www.google.com/")]
        [Theory]
        public void StrTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(make-string 4)", "    ")]
        [InlineData("(make-string 4 #\\*)", "****")]
        [Theory]
        public void MakeStringTests(string input, string expectedAfterToString)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            var sb = result as StringBuilder;

            Assert.NotNull(sb);
            Assert.Equal(expectedAfterToString, sb.ToString());
        }

        [InlineData("(string #\\a)", "a")]
        [InlineData("(string #\\a #\\b #\\c)", "abc")]
        [Theory]
        public void StringFromCharsTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(string-length \"abc\")", 3)]
        [InlineData("(string-length \"\")", 0)]
        [InlineData("(string-length (make-string 4))", 4)]
        [Theory]
        public void StringLengthTests(string input, int expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(string-ref \"abc\" 1)", 'b')]
        [InlineData("(string-ref (make-string 4 #\\*) 2)", '*')]
        [Theory]
        public void StringRefTests(string input, char expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(begin (def f (make-string 3 #\\*)) (string-set! f 0 #\\?) (str f))", "?**")]
        [Theory]
        public void StringSetTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(string=? \"a\" \"\u0061\")", true)]
        [InlineData("(string=? \"a\" \"\u0062\")", false)]
        [InlineData("(string<? \"a\" \"b\" \"c\")", true)]
        [InlineData("(string<? \"a\" \"Z\" \"0\")", false)]
        [InlineData("(string<=? \"a\" \"b\" \"b\")", true)]
        [InlineData("(string<=? \"A\" \"Z\" \"z\")", false)]
        [InlineData("(string>? \"c\" \"b\" \"a\")", true)]
        [InlineData("(string>? \"0\" \"Z\" \"a\")", false)]
        [InlineData("(string>=? \"b\" \"b\" \"a\")", true)]
        [InlineData("(string>=? \"z\" \"Z\" \"A\")", false)]
        [InlineData("(string-ci=? \"a\" \"A\")", true)]
        [InlineData("(string-ci=? \"a\" \"B\")", false)]
        [InlineData("(string-ci<? \"a\" \"B\" \"c\")", true)]
        [InlineData("(string-ci<? \"a\" \"A\" \"0\")", false)]
        [InlineData("(string-ci<=? \"a\" \"B\" \"b\")", true)]
        [InlineData("(string-ci<=? \"Z\" \"a\" \"a\")", false)]
        [InlineData("(string-ci>? \"c\" \"B\" \"a\")", true)]
        [InlineData("(string-ci>? \"0\" \"z\" \"a\")", false)]
        [InlineData("(string-ci>=? \"b\" \"B\" \"a\")", true)]
        [InlineData("(string-ci>=? \"a\" \"B\" \"b\")", false)]
        [Theory]
        public void StringEquivalenceTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(string-upcase \"aaa\")", "AAA")]
        [InlineData("(string-upcase \"AAA\")", "AAA")]
        [InlineData("(string-upcase \"999\")", "999")]
        [InlineData("(string-downcase \"AAA\")", "aaa")]
        [InlineData("(string-downcase \"aaa\")", "aaa")]
        [InlineData("(string-downcase \"999\")", "999")]
        [InlineData("(string-foldcase \"aaa\")", "aaa")]
        [InlineData("(string-foldcase \"AAA\")", "aaa")]
        [InlineData("(string-foldcase \"999\")", "999")]
        [Theory]
        public void CaseChangingTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
        
        [InlineData("(substring \"abcde\" 0 3)", "abc")]
        [InlineData("(substring \"abcde\" 1 3)", "bc")]
        [InlineData("(substring \"abcde\" 1 4)", "bcd")]
        [Theory]
        public void SubstringTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(string-append \"foo\" \"bar\")", "foobar")]
        [InlineData("(string-append \"foo\" \"bar\" \"baz\")", "foobarbaz")]
        [Theory]
        public void StringAppendTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(string-copy (make-string 4 #\\*))", "****")]
        [InlineData("(string-copy \"abcde\")", "abcde")]
        [InlineData("(string-copy \"abcde\" 1)", "bcde")]
        [InlineData("(string-copy \"abcde\" 0 3)", "abc")]
        [InlineData("(string-copy \"abcde\" 1 3)", "bc")]
        [InlineData("(string-copy \"abcde\" 1 4)", "bcd")]
        [Theory]
        public void StringCopyTests(string input, string expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
