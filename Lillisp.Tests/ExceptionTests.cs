using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public class ExceptionTests
    {
        [Fact]
        public void Raise_BasicTest()
        {
            var runtime = new LillispRuntime();
            
            var ex = Assert.Throws<RaisedException>(() => runtime.EvaluateProgram("(raise 'an-error)"));

            Assert.IsType<Symbol>(ex.Expression);
            Assert.Equal("an-error", ex.Expression?.ToString());
        }

        [Fact]
        public void Error_BasicTest()
        {
            var runtime = new LillispRuntime();

            var ex = Assert.Throws<ErrorException>(() => runtime.EvaluateProgram("(error \"something went wrong\")"));

            Assert.Equal("something went wrong", ex.Message);
            Assert.Equal(0, ex.Irritants.Count);
        }

        [Fact]
        public void Error_WithIrritants()
        {
            var runtime = new LillispRuntime();

            var ex = Assert.Throws<ErrorException>(() => runtime.EvaluateProgram("(error \"something went wrong\" 1 \"foo\")"));

            Assert.Equal("something went wrong", ex.Message);
            Assert.Equal(2, ex.Irritants.Count);
            Assert.Equal(1d, ex.Irritants[0]);
            Assert.Equal("foo", ex.Irritants[1]);
        }

        
        [Fact]
        public void WithExceptionHandler_BasicRaiseTest()
        {
            var runtime = new LillispRuntime();

            var prog = "(let ((x 0))" +
                       "(with-exception-handler " +
                       "    (lambda (ex) (begin (set! x 1) ()))" +
                       "    (lambda () (+ 1 (raise 'an-error))))" +
                       "x)";

            var result = runtime.EvaluateProgram(prog);

            Assert.Equal(1d, result);
        }

        [Fact]
        public void WithExceptionHandler_BasicRaiseContinuableTest()
        {
            var runtime = new LillispRuntime();

            var prog = "(def x 0)" +
                       "(list " +
                       "    (with-exception-handler " +
                       "        (lambda (con) " +
                       "            (begin " +
                       "                (if (string? con) (set! x 1) (println \"a warning has been issued\"))" +
                       "                42))" +
                       "        (lambda () (+ (raise-continuable \"should be a number\") 23)))" +
                       "    x)" +
                       ")";

            var result = (runtime.EvaluateProgram(prog) as Pair)?.ToArray();

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.Equal(65d, result[0]);
            Assert.Equal(1d, result[1]);
        }

        [Fact]
        public void WithExceptionHandler_BasicErrorTest()
        {
            var runtime = new LillispRuntime();

            var prog = "(let ((x 0))" +
                       "(with-exception-handler " +
                       "    (lambda (ex) (begin (set! x 1) ()))" +
                       "    (lambda () (+ 1 (error \"whoops\" 1 2 3))))" +
                       "x)";

            var result = runtime.EvaluateProgram(prog);

            Assert.Equal(1d, result);
        }

        [Fact]
        public void ErrorObject_BasicErrorTest()
        {
            var runtime = new LillispRuntime();

            var prog = "(let ((x 0))" +
                       "(with-exception-handler " +
                       "    (lambda (ex) (begin (when (error-object? ex) (set! x 1)) ()))" +
                       "    (lambda () (+ 1 (error \"whoops\" 1 2 3))))" +
                       "x)";

            var result = runtime.EvaluateProgram(prog);

            Assert.Equal(1d, result);
        }

        [Fact]
        public void ErrorObjectMessage_BasicErrorTest()
        {
            var runtime = new LillispRuntime();

            var prog = "(let ((x \"\"))" +
                       "(with-exception-handler " +
                       "    (lambda (ex) (begin (set! x (error-object-message ex)) ()))" +
                       "    (lambda () (+ 1 (error \"whoops\" 1 2 3))))" +
                       "x)";

            var result = runtime.EvaluateProgram(prog);

            Assert.Equal("whoops", result);
        }

        [Fact]
        public void ErrorObjectIrritants_BasicErrorTest()
        {
            var runtime = new LillispRuntime();

            var prog = "(let ((x (list)))" +
                       "(with-exception-handler " +
                       "    (lambda (ex) (begin (set! x (error-object-irritants ex)) ()))" +
                       "    (lambda () (+ 1 (error \"whoops\" 1 \"foo\" 3))))" +
                       "x)";

            var result = runtime.EvaluateProgram(prog) as IList<object>;

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(1d, result[0]);
            Assert.Equal("foo", result[1]);
            Assert.Equal(3d, result[2]);
        }
    }
}
