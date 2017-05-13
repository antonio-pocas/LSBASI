using System;
using LSBASI2.Part7;
using LSBASI2.Part7Exercises;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Part7Tests
    {
        [TestMethod]
        public void Part7_TestValidExpressions()
        {
            TestExpression("(((2*3))) + 4 + 10 * 100 - (2 * 20)", 970);
            TestExpression("1 + 1 + 1 + 1 + 2 * 2 + 4 * (5-2) / 2", 14);
            TestExpression("(((((1)))))", 1);
            TestExpression("2 * 2", 4);
            TestExpression("2 * 2 + 2", 6);
            TestExpression("2 + 2 * 2", 6);
            TestExpression("2 + 2 * 2 - 10 / 5", 4);
            TestExpression("52+(1+2)*4-3", 61);
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

    [TestClass]
    public class Part7RPNTranslatorTests
    {
        [TestMethod]
        public void Part7_RPNTranslator_TestValidExpressions()
        {
            TestExpression("(5 + 3) * 12 / 3", "5 3 + 12 * 3 /");
            TestExpression("2 + 3 * 5", "2 3 5 * +");
            TestExpression("5 + ((1 + 2) * 4) - 3", "5 1 2 + 4 * + 3 -");
            TestExpression("(52+1+2)*4-3", "52 1 + 2 + 4 * 3 -");
        }

        private void TestExpression(string expression, string expected)
        {
            var target = GetTarget(expression);
            var result = target.Evaluate();
            Assert.AreEqual(expected, result);
        }

        private RPNTranslator GetTarget(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            return new RPNTranslator(parser.expr());
        }
    }

    [TestClass]
    public class Part7SExpressionTranslatorTests
    {
        [TestMethod]
        public void Part7_SExpressionTranslator_TestValidExpressions()
        {
            TestExpression("1 + 2", "(+ 1 2)");
            TestExpression("(2 + 3 * 5)", "(+ 2 (* 3 5))");
            TestExpression("5 + ((1 + 2) * 4) - 3", "(- (+ 5 (* (+ 1 2) 4)) 3)");
            TestExpression("2 * 7", "(* 2 7)");
            TestExpression("7 + 5 * 2 - 3", "(- (+ 7 (* 5 2)) 3)");
        }

        private void TestExpression(string expression, string expected)
        {
            var target = GetTarget(expression);
            var result = target.Evaluate();
            Assert.AreEqual(expected, result);
        }

        private SExpressionTranslator GetTarget(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            return new SExpressionTranslator(parser.expr());
        }
    }
}