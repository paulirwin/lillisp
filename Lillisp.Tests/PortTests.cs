﻿using Lillisp.Core;

namespace Lillisp.Tests;

public class PortTests
{
    [Fact]
    public void ParameterizedOpenOutputStringPortTest()
    {
        const string program = "(parameterize ((current-output-port (open-output-string))) (display \"piece\") (display \" by piece \") (display \"by piece.\") (newline) (get-output-string (current-output-port)))";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program) as string;

        Assert.NotNull(result);

        if (Environment.NewLine == "\r\n")
        {
            Assert.Equal("piece by piece by piece.\r\n", result);
        }
        else
        {
            Assert.Equal("piece by piece by piece.\n", result);
        }
    }

    [Fact]
    public void ParameterizedOpenInputStringPortTest()
    {
        const string program = "(parameterize ((current-input-port (open-input-string \"ABC\"))) (list (read-char) (read-char) (read-char) (eof-object? (read-char))))";

        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(program) as IEnumerable<object>;

        Assert.NotNull(result);

        var list = result!.ToList();

        Assert.Equal('A', list[0]);
        Assert.Equal('B', list[1]);
        Assert.Equal('C', list[2]);
        Assert.Equal(true, list[3]);
    }

    [InlineData("(parameterize ((current-input-port (open-input-string \"ABC\\nDEF\"))) (list (read-line) (read-line)))", new object[] { "ABC", "DEF" })]
    [Theory]
    public void ReadLineTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(parameterize ((current-input-port (open-input-string \"'(1 2 3)\"))) (eval (read)))", new object[] { 1, 2, 3 })]
    [Theory]
    public void ReadTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(parameterize ((current-output-port (open-output-string))) (write '(1 2 3 \"abc\")) (get-output-string (current-output-port)))", "(1 2 3 \"abc\")")]
    [InlineData("(parameterize ((current-output-port (open-output-string))) (write-simple '(1 2 3 \"abc\")) (get-output-string (current-output-port)))", "(1 2 3 \"abc\")")]
    [Theory]
    public void WriteTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(parameterize ((current-input-port (open-input-string \"abc\"))) (char-ready?)))", true)]
    [InlineData("(char-ready?)", false)]
    [Theory]
    public void CharReadyTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(parameterize ((current-input-port (open-input-bytevector (string->utf8 \"abc\")))) (peek-u8)))", (byte)97)]
    [Theory]
    public void PeekU8Tests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(parameterize ((current-input-port (open-input-bytevector (string->utf8 \"abc\")))) (u8-ready?)))", true)]
    [InlineData("(parameterize ((current-input-port (Console/OpenStandardInput))) (u8-ready?)))", false)]
    [Theory]
    public void U8ReadyTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }

    [InlineData("(let ((p (open-input-string \"abc\"))) (read-line p))", "abc")]
    [Theory]
    public void CallWithPortTests(string input, object expected)
    {
        TestHelper.DefaultTest(input, expected);
    }
}