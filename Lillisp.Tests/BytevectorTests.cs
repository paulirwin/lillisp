using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class BytevectorTests
    {
        [InlineData("(length #u8(0 1 2 3))", 4)]
        [InlineData("(get #u8(3 2 1 0) 0)", (byte)3)]
        [InlineData("(length #u8())", 0)]
        [Theory]
        public void BytevectorLiteralTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(length (make-bytevector 10))", 10)]
        [InlineData("(get (make-bytevector 10) 1)", (byte)0)]
        [InlineData("(length (make-bytevector 10 42))", 10)]
        [InlineData("(get (make-bytevector 10 42) 1)", (byte)42)]
        [Theory]
        public void MakeBytevectorTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(length (bytevector 0 1 2 3))", 4)]
        [InlineData("(get (bytevector 3 2 1 0) 0)", (byte)3)]
        [InlineData("(length (bytevector))", 0)]
        [Theory]
        public void BytevectorCreationTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }


        [InlineData("(bytevector-length (bytevector 0 1 2 3))", 4)]
        [InlineData("(bytevector-length #u8(3 2 1 0))", 4)]
        [InlineData("(bytevector-length (bytevector))", 0)]
        [InlineData("(bytevector-length #u8())", 0)]
        [Theory]
        public void BytevectorLengthTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(bytevector-u8-ref '#u8(1 1 2 3 5 8 13 21) 5)", (byte)8)]
        [InlineData("(bytevector-u8-ref #u8(3 2 1 0) 0)", (byte)3)]
        [InlineData("(bytevector-u8-ref (bytevector 2 4 6 8) 3)", (byte)8)]
        [Theory]
        public void BytevectorRefTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [Fact]
        public void BytevectorSet_BasicTest()
        {
            var runtime = new LillispRuntime();

            // R7RS 6.9
            var result = runtime.EvaluateProgram("(let ((bv (bytevector 1 2 3 4))) (bytevector-u8-set! bv 1 3) bv)");

            var vector = result as Bytevector;

            Assert.NotNull(vector);
            Assert.Equal(4, vector.Count);
            Assert.Equal((byte)1, vector[0]);
            Assert.Equal((byte)3, vector[1]);
            Assert.Equal((byte)3, vector[2]);
            Assert.Equal((byte)4, vector[3]);
        }

        // R7RS 6.9
        [InlineData("(begin (define a #u8(1 2 3 4 5)) (length (bytevector-copy a 2 4)))", 2)]
        [InlineData("(begin (define a #u8(1 2 3 4 5)) (get (bytevector-copy a 2 4) 0))", (byte)3)]
        [InlineData("(begin (define a #u8(1 2 3 4 5)) (get (bytevector-copy a 2 4) 1))", (byte)4)]
        [Theory]
        public void BytevectorCopyTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [Fact]
        public void BytevectorCopyToTests()
        { 
            var runtime = new LillispRuntime();

            // R7RS 6.9
            var result = runtime.EvaluateProgram("(begin (define a (bytevector 1 2 3 4 5)) (define b (bytevector 10 20 30 40 50)) (bytevector-copy! b 1 a 0 2) b)");

            var vector = result as Bytevector;

            Assert.NotNull(vector);
            Assert.Equal(5, vector.Count);
            Assert.Equal((byte)10, vector[0]);
            Assert.Equal((byte)1, vector[1]);
            Assert.Equal((byte)2, vector[2]);
            Assert.Equal((byte)40, vector[3]);
            Assert.Equal((byte)50, vector[4]);
        }

        [InlineData("(begin (def x (bytevector-append #u8(0 1 2) #u8(3 4 5))) (bytevector-length x))", 6)]
        [InlineData("(begin (def x (bytevector-append #u8(0 1 2) #u8(3 4 5))) (bytevector-u8-ref x 0))", (byte)0)]
        [InlineData("(begin (def x (bytevector-append #u8(0 1 2) #u8(3 4 5))) (bytevector-u8-ref x 3))", (byte)3)]
        [Theory]
        public void BytevectorAppendTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
