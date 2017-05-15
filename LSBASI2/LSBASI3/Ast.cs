using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
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

        public abstract void Accept(IVisitor visitor);
    }

    public abstract class ExpressionNode : AstNode
    {
        public ExpressionNode()
        {
        }

        public ExpressionNode(Token token) : base(token)
        {
        }

        public abstract T Accept<T>(IVisitor visitor);
    }

    public class ProgramNode : StatementNode
    {
        public string Name { get; set; }
        public BlockNode Block { get; set; }

        public ProgramNode(string name, BlockNode block)
        {
            this.Name = name;
            Block = block;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> Declarations { get; set; }
        public CompoundNode Compound { get; set; }

        public BlockNode(List<StatementNode> declarations, CompoundNode compound)
        {
            Declarations = declarations;
            Compound = compound;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class DeclarationNode : StatementNode
    {
        public VariableNode Variable { get; set; }
        public TypeNode Type { get; set; }

        public DeclarationNode(VariableNode variable, TypeNode type)
        {
            Variable = variable;
            Type = type;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class TypeNode : ExpressionNode
    {
        public string Value { get; set; }

        public TypeNode(Token token) : base(token)
        {
            this.Value = token.Value;
        }

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class CompoundNode : StatementNode
    {
        public List<StatementNode> Children { get; set; }

        public CompoundNode(List<StatementNode> children)
        {
            this.Children = children;
        }

        public override void Accept(IVisitor visitor)
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NoOpNode : StatementNode
    {
        public override void Accept(IVisitor visitor)
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

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
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

                case TokenType.IntegerDivision:
                    this.Type = BinaryOperationType.Divide;
                    break;

                case TokenType.Star:
                    this.Type = BinaryOperationType.Multiply;
                    break;
            }
        }

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
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

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class NumberNode : ExpressionNode
    {
        public decimal Value { get; set; }

        public NumberNode(Token token) : base(token)
        {
            this.Value = decimal.Parse(token.Value, CultureInfo.InvariantCulture);
        }

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
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

    public interface IVisitor
    {
        void Visit(CompoundNode node);

        void Visit(NoOpNode node);

        void Visit(AssignmentNode node);

        T Visit<T>(NumberNode node);

        T Visit<T>(VariableNode node);

        T Visit<T>(UnaryOperationNode node);

        T Visit<T>(BinaryOperationNode node);

        void Visit(ProgramNode node);

        void Visit(BlockNode node);

        void Visit(DeclarationNode node);

        T Visit<T>(TypeNode node);
    }

    public interface IExpression
    {
        T Accept<T>(IVisitor visitor);
    }

    public interface IStatement
    {
        void Accept(IVisitor visitor);
    }
}