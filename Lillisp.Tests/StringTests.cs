using System.Linq;
using System.Text;
using Lillisp.Core;
using Xunit;
// ReSharper disable StringLiteralTypo

namespace Lillisp.Tests;

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
        TestHelper.DefaultTest(input, expected);
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
        TestHelper.DefaultTest(input, expected);
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
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(string-length \"abc\")", 3)]
    [InlineData("(string-length \"\")", 0)]
    [InlineData("(string-length (make-string 4))", 4)]
    [Theory]
    public void StringLengthTests(string input, int expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(string-ref \"abc\" 1)", 'b')]
    [InlineData("(string-ref (make-string 4 #\\*) 2)", '*')]
    [Theory]
    public void StringRefTests(string input, char expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(begin (def f (make-string 3 #\\*)) (string-set! f 0 #\\?) (str f))", "?**")]
    [Theory]
    public void StringSetTests(string input, string expected)
    {
        TestHelper.DefaultTest(input, expected);
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
        TestHelper.DefaultTest(input, expected);
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
        TestHelper.DefaultTest(input, expected);
    }
        
    [InlineData("(substring \"abcde\" 0 3)", "abc")]
    [InlineData("(substring \"abcde\" 1 3)", "bc")]
    [InlineData("(substring \"abcde\" 1 4)", "bcd")]
    [Theory]
    public void SubstringTests(string input, string expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(string-append \"foo\" \"bar\")", "foobar")]
    [InlineData("(string-append \"foo\" \"bar\" \"baz\")", "foobarbaz")]
    [Theory]
    public void StringAppendTests(string input, string expected)
    {
        TestHelper.DefaultTest(input, expected);
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

        var result = runtime.EvaluateProgram(input)?.ToString();

        Assert.Equal(expected, result);
    }

    [InlineData("(begin (def a \"12345\") (def b (string-copy \"abcde\")) (string-copy! b 1 a 0 2) (str b))", "a12de")]
    [Theory]
    public void StringCopyToTests(string input, string expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(begin (def a (make-string 4)) (string-fill! a #\\*) (str a))", "****")]
    [InlineData("(begin (def a (make-string 4 #\\a)) (string-fill! a #\\* 1) (str a))", "a***")]
    [InlineData("(begin (def a (make-string 4 #\\a)) (string-fill! a #\\* 1 3) (str a))", "a**a")]
    [Theory]
    public void StringFillTests(string input, string expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(string-map char-foldcase \"AbdEgH\")", "abdegh")]
    [InlineData("(string-map (lambda (c) (integer->char (+ 1 (char->integer c)))) \"HAL\")", "IBM")]
    [InlineData("(string-map (lambda (c k) ((if (eqv? k #\\u) char-upcase char-downcase) c)) \"studlycaps xxx\" \"ululululul\")", "StUdLyCaPs")]
    [Theory]
    public void StringMapTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [Fact]
    public void StringForEachTest()
    {
        const string program = @"
(let ((v '()))
    (string-for-each
        (lambda (c) (set! v (cons (char->integer c) v)))
        ""abcde"")
v)
";
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program) as Pair;

        Assert.NotNull(result);

        var resultList = result.ToList();

        Assert.Equal(5, resultList.Count);
        Assert.Equal(101, resultList[0]);
        Assert.Equal(100, resultList[1]);
        Assert.Equal(99, resultList[2]);
        Assert.Equal(98, resultList[3]);
        Assert.Equal(97, resultList[4]);
    }
}