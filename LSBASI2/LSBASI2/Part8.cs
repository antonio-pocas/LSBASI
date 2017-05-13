using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI2.Part8
{
    public class Lexer
    {
        private string currentChar => position < input.Length ? input[position].ToString() : null;
        private readonly string input;
        private int position;

        public Lexer(string input)
        {
            this.input = input;
        }

        public Token GetNextToken()
        {
            SkipWhitespace();

            if (currentChar != null)
            {
                int tempValue;
                if (int.TryParse(currentChar, out tempValue))
                {
                    return Integer();
                }

                if (currentChar == "/")
                {
                    position++;
                    return new Token(TokenType.Divide, string.Empty);
                }

                if (currentChar == "*")
                {
                    position++;
                    return new Token(TokenType.Multiply, string.Empty);
                }

                if (currentChar == "+")
                {
                    position++;
                    return new Token(TokenType.Add, string.Empty);
                }

                if (currentChar == "-")
                {
                    position++;
                    return new Token(TokenType.Subtract, string.Empty);
                }

                if (currentChar == "(")
                {
                    position++;
                    return new Token(TokenType.LeftParen, string.Empty);
                }

                if (currentChar == ")")
                {
                    position++;
                    return new Token(TokenType.RightParen, string.Empty);
                }

                throw new InvalidOperationException("Lexer error");
            }

            return Token.CreateEOFToken();
        }

        private Token Integer()
        {
            var sb = new StringBuilder();
            int tempValue;
            do
            {
                sb.Append(currentChar);
                position++;
            } while (int.TryParse(currentChar, out tempValue));

            return new Token(TokenType.Integer, sb.ToString());
        }

        private void SkipWhitespace()
        {
            while (currentChar == " ")
            {
                position++;
            }
        }
    }

    /// <summary>
    /// Grammar:
    /// expr:      term ((ADD|SUB) term)*
    /// term:      factor ((MUL|DIV) factor)*
    /// factor:    (+|-)factor | INTEGER | LPAREN expr RPAREN
    /// </summary>
    public class Parser
    {
        private Token currentToken;
        private readonly Lexer lexer;

        private readonly HashSet<TokenType> termOperationTokens = new HashSet<TokenType>() { TokenType.Multiply, TokenType.Divide };
        private readonly HashSet<TokenType> exprOperationTokens = new HashSet<TokenType>() { TokenType.Add, TokenType.Subtract };
        private readonly HashSet<TokenType> factorOperationTokens = new HashSet<TokenType>() { TokenType.Add, TokenType.Subtract };

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            currentToken = lexer.GetNextToken();
        }

        private void Eat(TokenType type)
        {
            if (currentToken.Type == type)
            {
                currentToken = lexer.GetNextToken();
            }
            else
            {
                throw new InvalidOperationException("Parser error");
            }
        }

        public AstNode expr()
        {
            var result = term();
            while (exprOperationTokens.Contains(currentToken.Type))
            {
                if (currentToken.Type == TokenType.Add)
                {
                    var token = currentToken;
                    Eat(TokenType.Add);
                    result = new BinaryOperationNode(token, result, term());
                }
                else if (currentToken.Type == TokenType.Subtract)
                {
                    var token = currentToken;
                    Eat(TokenType.Subtract);
                    result = new BinaryOperationNode(token, result, term());
                }
            }

            return result;
        }

        private AstNode term()
        {
            var result = factor();
            while (termOperationTokens.Contains(currentToken.Type))
            {
                if (currentToken.Type == TokenType.Multiply)
                {
                    var token = currentToken;
                    Eat(TokenType.Multiply);
                    result = new BinaryOperationNode(token, result, factor());
                }
                else if (currentToken.Type == TokenType.Divide)
                {
                    var token = currentToken;
                    Eat(TokenType.Divide);
                    result = new BinaryOperationNode(token, result, factor());
                }
            }

            return result;
        }

        private AstNode factor()
        {
            if (factorOperationTokens.Contains(currentToken.Type))
            {
                var token = currentToken;
                Eat(currentToken.Type);
                return new UnaryOperationNode(token, factor());
            }
            if (currentToken.Type == TokenType.Integer)
            {
                var value = new NumberNode(currentToken);
                Eat(TokenType.Integer);
                return value;
            }

            Eat(TokenType.LeftParen);
            var ret = expr();
            Eat(TokenType.RightParen);
            return ret;
        }
    }

    public class Interpreter : IVisitor<int>
    {
        private Parser parser;
        private readonly AstNode rootNode;

        public Interpreter(Parser parser)
        {
            this.parser = parser;
            this.rootNode = parser.expr();
        }

        public int Evaluate()
        {
            return this.rootNode.Accept(this);
        }

        public int Visit(UnaryOperationNode node)
        {
            switch (node.Type)
            {
                case UnaryOperationType.Plus:
                    return +node.Child.Accept(this);

                case UnaryOperationType.Minus:
                    return -node.Child.Accept(this);
            }

            throw new Exception();
        }

        int IVisitor<int>.Visit(BinaryOperationNode node)
        {
            switch (node.Type)
            {
                case BinaryOperationType.Add:
                    return node.Left.Accept(this) + node.Right.Accept(this);

                case BinaryOperationType.Subtract:
                    return node.Left.Accept(this) - node.Right.Accept(this);

                case BinaryOperationType.Multiply:
                    return node.Left.Accept(this) * node.Right.Accept(this);

                case BinaryOperationType.Divide:
                    return node.Left.Accept(this) / node.Right.Accept(this);
            }

            throw new Exception();
        }

        int IVisitor<int>.Visit(NumberNode node)
        {
            return node.Value;
        }
    }
}