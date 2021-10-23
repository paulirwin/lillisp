using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Lillisp.Tests
{
    public class InteropTests
    {
        [InlineData("(begin (use 'System.Text) (typeof (new StringBuilder)))", typeof(StringBuilder))]
        [InlineData("(typeof (new Uri \"https://www.google.com\"))", typeof(Uri))]
        [InlineData("(begin (def x (new DateTime)) (typeof x))", typeof(DateTime))]
        [InlineData("(use 'System.Collections.Generic) (def x (new (List String))) (.Add x \"foo\") (get x 0)", "foo")]
        [Theory]
        public void NewAndUseTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("Math/PI", Math.PI)]
        [InlineData("(typeof (Guid/NewGuid))", typeof(Guid))]
        [InlineData("(begin (def x String/Empty) x)", "")]
        [InlineData("(String/IsNullOrEmpty null)", true)]
        [InlineData("(String/IsNullOrEmpty \"foo\")", false)]
        [InlineData("(Int32/Parse \"123\")", 123)]
        [Theory]
        public void StaticMemberTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
        
        [InlineData("(begin (def x (new Uri \"https://www.google.com\")) (.Scheme x))", "https")]
        [InlineData("(begin (use 'System.Text) (def x (new StringBuilder)) (.Append x \"foo\") (.Append x \"bar\") (str x))", "foobar")]
        [Theory]
        public void InstanceMemberTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }

        [InlineData("(System.Collections.Generic.List String)", typeof(List<string>))]
        [InlineData("(use 'System.Collections.Generic) (List String)", typeof(List<string>))]
        [InlineData("(use 'System.Collections.Generic) (Dictionary String Int32)", typeof(Dictionary<string, int>))]
        [InlineData("(use 'System.Collections.Generic) (Dictionary String (List Guid))", typeof(Dictionary<string, List<Guid>>))]
        [Theory]
        public void GenericTypeTests(string input, object expected)
        {
            TestHelper.DefaultTest(input, expected);
        }
    }
}
