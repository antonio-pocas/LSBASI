using System;
using LSBASI2.Part6Exercise;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Part6ExerciseTests
    {
        [TestMethod]
        public void Part6_Exercise_TestValidExpressions()
        {
            TestExpression("(((2*3))) + 4 + 10 * 100 - (2 * 20)", 970);
            TestExpression("1 + 1 + 1 + 1 + 2 * 2 + 4 * (5-2) / 2", 14);
            TestExpression("(((((1)))))", 1);
            TestExpression("2 * 2", 4);
            TestExpression("2 * 2 + 2", 6);
            TestExpression("2 + 2 * 2", 6);
            TestExpression("2 + 2 * 2 - 10 / 5", 4);
        }

        private void TestExpression(string expression, int expected)
        {
            var target = GetTarget(expression);
            var result = target.expr();
            Assert.AreEqual(expected, result);
        }

        private Parser GetTarget(string input)
        {
            var lexer = new Lexer(input);
            return new Parser(lexer);
        }
    }
}