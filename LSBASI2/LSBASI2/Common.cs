using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI2
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType tokenType, string value)
        {
            Type = tokenType;
            Value = value;
        }

        public override string ToString()
        {
            return $"Token({Type.ToString()}, {Value})";
        }

        public static Token CreateEOFToken()
        {
            return new Token(TokenType.EOF, string.Empty);
        }
    }

    public enum TokenType
    {
        Integer,
        Add,
        Subtract,
        Divide,
        Multiply,
        LeftParen,
        RightParen,
        EOF
    }

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
        public BinaryOperationType Type { get; set; }

        public BinaryOperationNode(Token token, AstNode left, AstNode right) : base(token)
        {
            this.Left = left;
            this.Right = right;
            switch (token.Type)
            {
                case TokenType.Add:
                    this.Type = BinaryOperationType.Add;
                    break;

                case TokenType.Subtract:
                    this.Type = BinaryOperationType.Subtract;
                    break;

                case TokenType.Divide:
                    this.Type = BinaryOperationType.Divide;
                    break;

                case TokenType.Multiply:
                    this.Type = BinaryOperationType.Multiply;
                    break;
            }
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class UnaryOperationNode : AstNode
    {
        public AstNode Child { get; set; }
        public UnaryOperationType Type { get; set; }

        public UnaryOperationNode(Token token, AstNode child) : base(token)
        {
            this.Child = child;
            switch (token.Type)
            {
                case TokenType.Add:
                    this.Type = UnaryOperationType.Plus;
                    break;

                case TokenType.Subtract:
                    this.Type = UnaryOperationType.Minus;
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

    public enum BinaryOperationType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public enum UnaryOperationType
    {
        Plus,
        Minus
    }

    public interface IVisitor<T>
    {
        T Visit(NumberNode node);

        T Visit(UnaryOperationNode node);

        T Visit(BinaryOperationNode node);
    }
}