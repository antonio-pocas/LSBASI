using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
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
            return new Token(TokenType.Plus, "+");
        }

        public static Token Subtract()
        {
            return new Token(TokenType.Minus, "-");
        }

        public static Token Divide()
        {
            return new Token(TokenType.Division, "div");
        }

        public static Token Multiply()
        {
            return new Token(TokenType.Star, "*");
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
        Plus,
        Minus,
        Division,
        Star,
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

        public AstNode()
        {
        }

        public AstNode(Token token)
        {
            this.token = token;
        }
    }

    public abstract class StatementNode : AstNode
    {
        public StatementNode()
        {
        }

        public StatementNode(Token token) : base(token)
        {
        }

        public abstract void Accept<T>(IVisitor<T> visitor);
    }

    public abstract class ExpressionNode : AstNode
    {
        public ExpressionNode()
        {
        }

        public ExpressionNode(Token token) : base(token)
        {
        }

        public abstract T Accept<T>(IVisitor<T> visitor);
    }

    public class CompoundNode : StatementNode
    {
        public List<StatementNode> Children { get; set; }

        public CompoundNode(List<StatementNode> children)
        {
            this.Children = children;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }

    public class AssignmentNode : StatementNode
    {
        public VariableNode Variable { get; set; }
        public ExpressionNode Result { get; set; }

        public AssignmentNode(Token token, VariableNode variable, ExpressionNode result) : base(token)
        {
            this.Variable = variable;
            this.Result = result;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NoOpNode : StatementNode
    {
        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(this);
        }
    }

    public class VariableNode : ExpressionNode
    {
        public string Name { get; set; }

        public VariableNode(Token token) : base(token)
        {
            this.Name = token.Value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class BinaryOperationNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
        public BinaryOperationType Type { get; set; }

        public BinaryOperationNode(Token token, ExpressionNode left, ExpressionNode right) : base(token)
        {
            this.Left = left;
            this.Right = right;
            switch (token.Type)
            {
                case TokenType.Plus:
                    this.Type = BinaryOperationType.Add;
                    break;

                case TokenType.Minus:
                    this.Type = BinaryOperationType.Subtract;
                    break;

                case TokenType.Division:
                    this.Type = BinaryOperationType.Divide;
                    break;

                case TokenType.Star:
                    this.Type = BinaryOperationType.Multiply;
                    break;
            }
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class UnaryOperationNode : ExpressionNode
    {
        public ExpressionNode Child { get; set; }
        public UnaryOperationType Type { get; set; }

        public UnaryOperationNode(Token token, ExpressionNode child) : base(token)
        {
            this.Child = child;
            switch (token.Type)
            {
                case TokenType.Plus:
                    this.Type = UnaryOperationType.Plus;
                    break;

                case TokenType.Minus:
                    this.Type = UnaryOperationType.Minus;
                    break;
            }
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class NumberNode : ExpressionNode
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
        void Visit(CompoundNode node);

        void Visit(NoOpNode node);

        void Visit(AssignmentNode node);

        T Visit(NumberNode node);

        T Visit(VariableNode node);

        T Visit(UnaryOperationNode node);

        T Visit(BinaryOperationNode node);
    }
}