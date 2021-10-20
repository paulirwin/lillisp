using System.Numerics;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class ComplexTests
    {
        [InlineData("(make-rectangular 42 7)", 42d, 7d)]
        [InlineData("(make-rectangular 23.5 0)", 23.5d, 0d)]
        [InlineData("(make-rectangular 42 -7)", 42d, -7d)]
        [InlineData("(make-rectangular -23.5 0)", -23.5d, 0d)]
        [InlineData("(make-rectangular 0 -23.5)", 0d, -23.5d)]
        [InlineData("(make-rectangular 0.0 0)", 0d, 0d)]
        [InlineData("(make-rectangular -42 7)", -42d, 7d)]
        [InlineData("(make-rectangular -42 -7)", -42d, -7d)]
        [Theory]
        public void MakeRectangularTests(string input, double real, double imaginary)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.IsType<Complex>(result);

            var complex = (Complex)result;

            Assert.Equal(real, complex.Real, 6);
            Assert.Equal(imaginary, complex.Imaginary, 6);
        }

        [InlineData("(make-polar 5.39 0.38)", 5.39d, 0.38d)]
        [InlineData("(make-polar 23.5 0)", 23.5d, 0d)]
        [InlineData("(make-polar 0 0.5)", 0d, 0d)]
        [InlineData("(make-polar 0.0 0)", 0d, 0d)]
        [Theory]
        public void MakePolarTests(string input, double magnitude, double phase)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.IsType<Complex>(result);

            var complex = (Complex)result;

            Assert.Equal(phase, complex.Phase, 6);
            Assert.Equal(magnitude, complex.Magnitude, 6);
        }

        [InlineData("(real-part 42+7i)", 42d)]
        [InlineData("(real-part 23.5+0i)", 23.5d)]
        [InlineData("(real-part 42-7i)", 42d)]
        [InlineData("(real-part -23.5+0i)", -23.5d)]
        [InlineData("(real-part 0-23.5i)", 0d)]
        [InlineData("(real-part 0.0+0i)", 0d)]
        [InlineData("(real-part -42+7i)", -42d)]
        [InlineData("(real-part -42-7i)", -42d)]
        [Theory]
        public void RealPartTests(string input, double real)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.IsType<double>(result);
            Assert.Equal(real, (double)result, 6);
        }

        [InlineData("(imag-part 42+7i)", 7d)]
        [InlineData("(imag-part 23.5+0i)", 0d)]
        [InlineData("(imag-part 42-7i)", -7d)]
        [InlineData("(imag-part -23.5+0i)", 0d)]
        [InlineData("(imag-part 0-23.5i)", -23.5d)]
        [InlineData("(imag-part 0.0+0i)", 0d)]
        [InlineData("(imag-part -42+7i)", 7d)]
        [InlineData("(imag-part -42-7i)", -7d)]
        [Theory]
        public void ImagPartTests(string input, double imaginary)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.IsType<double>(result);
            Assert.Equal(imaginary, (double)result, 6);
        }

        [InlineData("(angle 0+0i)", 0d)]
        [InlineData("(angle (make-polar 5 0.38))", 0.38d)]
        [InlineData("(angle (make-polar 5 0))", 0d)]
        [Theory]
        public void AngleTests(string input, double angle)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.IsType<double>(result);
            Assert.Equal(angle, (double)result, 6);
        }

        [InlineData("(magnitude 0+0i)", 0d)]
        [InlineData("(magnitude (make-polar 5 0.38))", 5d)]
        [InlineData("(magnitude (make-polar 5 0))", 5d)]
        [Theory]
        public void MagnitudeTests(string input, double magnitude)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.IsType<double>(result);
            Assert.Equal(magnitude, (double)result, 6);
        }
    }
}
