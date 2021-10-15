using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class CharacterTests
    {
        [InlineData("#\\alarm", '\u0007')]
        [InlineData("#\\backspace", '\u0008')]
        [InlineData("#\\delete", '\u007f')]
        [InlineData("#\\escape", '\u001b')]
        [InlineData("#\\newline", '\n')]
        [InlineData("#\\null", '\0')]
        [InlineData("#\\return", '\r')]
        [InlineData("#\\space", ' ')]
        [InlineData("#\\tab", '\t')]
        [InlineData("#\\a", 'a')]
        [InlineData("#\\A", 'A')]
        [InlineData("#\\(", '(')]
        [InlineData("#\\", ' ')]
        [InlineData("#\\x03BB", '\u03bb')]
        [Theory]
        public void CharacterLiteralTests(string input, char expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        // NOTE: Reminder that in ASCII/UTF-8, upper-case latin chars come before lower-case
        [InlineData("(char=? #\\a #\\x0061)", true)]
        [InlineData("(char=? #\\a #\\x0062)", false)]
        [InlineData("(char<? #\\a #\\b #\\c)", true)]
        [InlineData("(char<? #\\a #\\Z #\\0)", false)]
        [InlineData("(char<=? #\\a #\\b #\\b)", true)]
        [InlineData("(char<=? #\\a #\\Z #\\Z)", false)]
        [InlineData("(char>? #\\c #\\b #\\a)", true)]
        [InlineData("(char>? #\\0 #\\Z #\\a)", false)]
        [InlineData("(char>=? #\\b #\\b #\\a)", true)]
        [InlineData("(char>=? #\\Z #\\Z #\\a)", false)]
        [InlineData("(char-ci=? #\\a #\\A)", true)]
        [InlineData("(char-ci=? #\\a #\\B)", false)]
        [InlineData("(char-ci<? #\\a #\\B #\\c)", true)]
        [InlineData("(char-ci<? #\\a #\\A #\\0)", false)]
        [InlineData("(char-ci<=? #\\a #\\B #\\b)", true)]
        [InlineData("(char-ci<=? #\\Z #\\a #\\a)", false)]
        [InlineData("(char-ci>? #\\c #\\B #\\a)", true)]
        [InlineData("(char-ci>? #\\0 #\\z #\\a)", false)]
        [InlineData("(char-ci>=? #\\b #\\B #\\a)", true)]
        [InlineData("(char-ci>=? #\\a #\\B #\\b)", false)]
        [Theory]
        public void CharacterEquivalenceTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(char-alphabetic? #\\a)", true)]
        [InlineData("(char-alphabetic? #\\A)", true)]
        [InlineData("(char-alphabetic? #\\0)", false)]
        [InlineData("(char-alphabetic? #\\()", false)]
        [InlineData("(char-alphabetic? #\\tab)", false)]
        [InlineData("(char-numeric? #\\a)", false)]
        [InlineData("(char-numeric? #\\A)", false)]
        [InlineData("(char-numeric? #\\0)", true)]
        [InlineData("(char-numeric? #\\()", false)]
        [InlineData("(char-numeric? #\\tab)", false)]
        [InlineData("(char-whitespace? #\\a)", false)]
        [InlineData("(char-whitespace? #\\A)", false)]
        [InlineData("(char-whitespace? #\\0)", false)]
        [InlineData("(char-whitespace? #\\()", false)]
        [InlineData("(char-whitespace? #\\tab)", true)]
        [InlineData("(char-upper-case? #\\a)", false)]
        [InlineData("(char-upper-case? #\\A)", true)]
        [InlineData("(char-upper-case? #\\0)", false)]
        [InlineData("(char-upper-case? #\\()", false)]
        [InlineData("(char-upper-case? #\\tab)", false)]
        [InlineData("(char-lower-case? #\\a)", true)]
        [InlineData("(char-lower-case? #\\A)", false)]
        [InlineData("(char-lower-case? #\\0)", false)]
        [InlineData("(char-lower-case? #\\()", false)]
        [InlineData("(char-lower-case? #\\tab)", false)]
        [Theory]
        public void CharacterCategoryTests(string input, bool expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(digit-value #\\3)", 3)]
        [InlineData("(digit-value #\\x0664)", 4)]
        [InlineData("(digit-value #\\x0AE6)", 0)]
        [InlineData("(digit-value #\\x0EA6)", false)]
        [InlineData("(digit-value #\\=)", false)]
        [Theory]
        public void DigitValueTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(char-upcase #\\a)", 'A')]
        [InlineData("(char-upcase #\\A)", 'A')]
        [InlineData("(char-upcase #\\9)", '9')]
        [InlineData("(char-downcase #\\A)", 'a')]
        [InlineData("(char-downcase #\\a)", 'a')]
        [InlineData("(char-downcase #\\9)", '9')]
        [InlineData("(char-foldcase #\\A)", 'a')]
        [InlineData("(char-foldcase #\\a)", 'a')]
        [InlineData("(char-foldcase #\\9)", '9')]
        [Theory]
        public void CaseChangingTests(string input, char expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }
    }
}
