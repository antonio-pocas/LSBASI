using System;
using LSBASI2.Part8;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Part8Tests
    {
        [TestMethod]
        public void Part8_TestValidExpressions()
        {
            TestExpression("-3", -3);
            TestExpression("+3", 3);
            TestExpression("5 - - -+-3", 8);
            TestExpression("5 - - -+-(3 + 4) - +2", 10);
        }

        private void TestExpression(string expression, int expected)
        {
            var target = GetTarget(expression);
            var result = target.Evaluate();
            Assert.AreEqual(expected, result);
        }

        private Interpreter GetTarget(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            return new Interpreter(parser);
        }
    }
}