using System.Linq;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class VectorTests
    {
        [Fact]
        public void MakeVector_NoFill()
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram("(make-vector 10)");

            var vector = result as Vector;
            
            Assert.NotNull(vector);
            Assert.Equal(10, vector.Count);
        }

        [Fact]
        public void MakeVector_WithFill()
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram("(make-vector 3 42.)");

            var vector = result as Vector;

            Assert.NotNull(vector);
            Assert.Equal(3, vector.Count);
            Assert.Equal(42d, vector[0]);
            Assert.Equal(42d, vector[1]);
            Assert.Equal(42d, vector[2]);
        }

        [InlineData("(vector)")]
        [InlineData("[]")]
        [InlineData("#()")]
        [Theory]
        public void Vector_Empty(string input)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            var vector = result as Vector;

            Assert.NotNull(vector);
            Assert.Empty(vector);
        }

        [InlineData("(vector 1 2 3)")]
        [InlineData("[1 2 3]")]
        [InlineData("#(1 2 3)")]
        [Theory]
        public void Vector_BasicNumbers(string input)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            var vector = result as Vector;

            Assert.NotNull(vector);
            Assert.Equal(3, vector.Count);
            Assert.Equal(1, vector[0]);
            Assert.Equal(2, vector[1]);
            Assert.Equal(3, vector[2]);
        }

        [InlineData("(vector-length (vector))", 0)]
        [InlineData("(vector-length [])", 0)]
        [InlineData("(vector-length (vector 1))", 1)]
        [InlineData("(vector-length [1 2 3])", 3)]
        [Theory]
        public void VectorLengthTests(string input, int expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [InlineData("(vector-ref (vector 1) 0)", 1)]
        [InlineData("(vector-ref [1 2 3] 1)", 2)]
        [Theory]
        public void VectorRefTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void VectorSet_BasicTest()
        {
            var runtime = new LillispRuntime();

            // R7RS 6.8
            var result = runtime.EvaluateProgram("(let ((vec (vector 0 '(2 2 2 2) \"Anna\"))) (vector-set! vec 1 '(\"Sue\" \"Sue\")) vec)");

            var vector = result as Vector;

            Assert.NotNull(vector);
            Assert.Equal(3, vector.Count);
            Assert.Equal(0, vector[0]);

            var list = (vector[1] as Pair)?.ToList();

            Assert.NotNull(list);
            Assert.Equal("Sue", list[0]);
            Assert.Equal("Sue", list[1]);

            Assert.Equal("Anna", vector[2]);
        }

        // R7RS 6.8
        [InlineData("(begin (def a [1 8 2 8]) (def b (vector-copy a)) (vector-set! b 0 3) (vector-ref b 0))", 3)]
        [InlineData("(begin (def a [1 8 2 8]) (def b (vector-copy a)) (vector-set! b 0 3) (vector-ref b 1))", 8)]
        [InlineData("(begin (def a [1 8 2 8]) (def b (vector-copy a)) (vector-set! b 0 3) (vector-length b))", 4)]
        [InlineData("(begin (def a [1 8 2 8]) (def b (vector-copy a)) (vector-set! b 0 3) (def c (vector-copy b 1 3)) (vector-ref c 0))", 8)]
        [InlineData("(begin (def a [1 8 2 8]) (def b (vector-copy a)) (vector-set! b 0 3) (def c (vector-copy b 1 3)) (vector-ref c 1))", 2)]
        [InlineData("(begin (def a [1 8 2 8]) (def b (vector-copy a)) (vector-set! b 0 3) (def c (vector-copy b 1 3)) (vector-length c))", 2)]
        [Theory]
        public void VectorCopyTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        // R7RS 6.8, slightly modified
        [Fact]
        public void VectorCopyToTest_R7RS_6_8()
        {
            var runtime = new LillispRuntime();

            string prog = "(begin (define a [1 2 3 4 5]) (define b [10 20 30 40 50]) (vector-copy! b 1 a 0 2) b)";

            var result = runtime.EvaluateProgram(prog);

            var vector = result as Vector;

            Assert.NotNull(vector);
            Assert.Equal(5, vector.Count);
            Assert.Equal(10, vector[0]);
            Assert.Equal(1, vector[1]);
            Assert.Equal(2, vector[2]);
            Assert.Equal(40, vector[3]); 
            Assert.Equal(50, vector[4]);
        }

        [InlineData("(begin (def x (vector-append [0 1 2] [3 4 5])) (vector-length x))", 6)]
        [InlineData("(begin (def x (vector-append [0 1 2] [3 4 5])) (vector-ref x 0))", 0)]
        [InlineData("(begin (def x (vector-append [0 1 2] [3 4 5])) (vector-ref x 3))", 3)]
        [Theory]
        public void VectorAppendTests(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);
        }

        // R7RS 6.8, slightly modified
        [Fact]
        public void VectorFillTest_R7RS_6_8()
        {
            var runtime = new LillispRuntime();

            string prog = "(begin (define a [1 2 3 4 5]) (vector-fill! a \"smash\" 2 4) a)";

            var result = runtime.EvaluateProgram(prog);

            var vector = result as Vector;

            Assert.NotNull(vector);
            Assert.Equal(5, vector.Count);
            Assert.Equal(1, vector[0]);
            Assert.Equal(2, vector[1]);
            Assert.Equal("smash", vector[2]);
            Assert.Equal("smash", vector[3]);
            Assert.Equal(5, vector[4]);
        }
    }
}
