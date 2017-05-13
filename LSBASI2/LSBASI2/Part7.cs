using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSBASI2.Part7Exercises;

namespace LSBASI2.Part7
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
    /// factor:    INTEGER | LPAREN expr RPAREN
    /// </summary>
    public class Parser
    {
        private Token currentToken;
        private readonly Lexer lexer;

        private readonly HashSet<TokenType> termOperationTokens = new HashSet<TokenType>() { TokenType.Multiply, TokenType.Divide };
        private readonly HashSet<TokenType> exprOperationTokens = new HashSet<TokenType>() { TokenType.Add, TokenType.Subtract };

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

        int IVisitor<int>.Visit(BinaryOperationNode node)
        {
            switch (node.Type)
            {
                case OperationType.Add:
                    return node.Left.Accept(this) + node.Right.Accept(this);

                case OperationType.Subtract:
                    return node.Left.Accept(this) - node.Right.Accept(this);

                case OperationType.Multiply:
                    return node.Left.Accept(this) * node.Right.Accept(this);

                case OperationType.Divide:
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

namespace LSBASI2
{
    public abstract class AstNode
    {
        public Token token;

        public AstNode(Token token)
        {
            this.token = token;
        }

        public abstract T Accept<T>(IVisitor<T> visitor);
    }

    public class BinaryOperationNode : AstNode
    {
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }
        public OperationType Type { get; set; }

        public BinaryOperationNode(Token token, AstNode left, AstNode right) : base(token)
        {
            this.Left = left;
            this.Right = right;
            switch (token.Type)
            {
                case TokenType.Add:
                    this.Type = OperationType.Add;
                    break;

                case TokenType.Subtract:
                    this.Type = OperationType.Subtract;
                    break;

                case TokenType.Divide:
                    this.Type = OperationType.Divide;
                    break;

                case TokenType.Multiply:
                    this.Type = OperationType.Multiply;
                    break;
            }
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class NumberNode : AstNode
    {
        public int Value { get; set; }

        public NumberNode(Token token) : base(token)
        {
            this.Value = int.Parse(token.Value);
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public enum OperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public interface IVisitor<T>
    {
        T Visit(NumberNode numberNode);

        T Visit(BinaryOperationNode binaryOperationNode);
    }
}