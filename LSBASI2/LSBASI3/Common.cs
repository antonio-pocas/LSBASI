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

        public static Token EOF()
        {
            return new Token(TokenType.EOF, string.Empty);
        }

        public static Token Assign()
        {
            return new Token(TokenType.Assign, ":=");
        }

        public static Token Add()
        {
            return new Token(TokenType.Add, "+");
        }

        public static Token Subtract()
        {
            return new Token(TokenType.Subtract, "-");
        }

        public static Token Divide()
        {
            return new Token(TokenType.Divide, "/");
        }

        public static Token Multiply()
        {
            return new Token(TokenType.Multiply, "*");
        }

        public static Token LeftParen()
        {
            return new Token(TokenType.LeftParen, "(");
        }

        public static Token RightParen()
        {
            return new Token(TokenType.RightParen, ")");
        }

        public static Token Begin()
        {
            return new Token(TokenType.Begin, "BEGIN");
        }

        public static Token End()
        {
            return new Token(TokenType.End, "END");
        }

        public static Token Semicolon()
        {
            return new Token(TokenType.Semicolon, ";");
        }

        public static Token Dot()
        {
            return new Token(TokenType.Dot, ".");
        }

        public static Token Id(string value)
        {
            return new Token(TokenType.Id, value);
        }

        public static Token Integer(string value)
        {
            return new Token(TokenType.Integer, value);
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
        Begin,
        End,
        Semicolon,
        Dot,
        Assign,
        Id,
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

    //public class StatementNode : AstNode
    //{
    //    public AstNode Child { get; set; }
    //    public UnaryOperationType Type { get; set; }

    //    public StatementNode(Token token, AstNode child) : base(token)
    //    {
    //        this.Child = child;
    //        switch (token.Type)
    //        {
    //            case TokenType.Add:
    //                this.Type = UnaryOperationType.Plus;
    //                break;

    //            case TokenType.Subtract:
    //                this.Type = UnaryOperationType.Minus;
    //                break;
    //        }
    //    }

    //    public override T Accept<T>(IVisitor<T> visitor)
    //    {
    //        return visitor.Visit(this);
    //    }
    //}

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