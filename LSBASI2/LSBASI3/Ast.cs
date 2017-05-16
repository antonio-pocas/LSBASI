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

        public abstract void Accept(IVisitor visitor);

        public abstract T Accept<T>(IVisitor visitor);
    }

    public class ProgramNode : AstNode
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

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class BlockNode : AstNode
    {
        public List<AstNode> Declarations { get; set; }
        public CompoundNode Compound { get; set; }

        public BlockNode(List<AstNode> declarations, CompoundNode compound)
        {
            Declarations = declarations;
            Compound = compound;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class DeclarationNode : AstNode
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

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class TypeNode : AstNode
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class CompoundNode : AstNode
    {
        public List<AstNode> Children { get; set; }

        public CompoundNode(List<AstNode> children)
        {
            this.Children = children;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class AssignmentNode : AstNode
    {
        public VariableNode Variable { get; set; }
        public AstNode Result { get; set; }

        public AssignmentNode(Token token, VariableNode variable, AstNode result) : base(token)
        {
            this.Variable = variable;
            this.Result = result;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class NoOpNode : AstNode
    {
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Accept<T>(IVisitor visitor)
        {
            return visitor.Visit<T>(this);
        }
    }

    public class VariableNode : AstNode
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NumberNode : AstNode
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
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

        T Visit<T>(NoOpNode noOpNode);

        void Visit(VariableNode variableNode);

        void Visit(BinaryOperationNode binaryOperationNode);

        void Visit(UnaryOperationNode unaryOperationNode);

        void Visit(NumberNode numberNode);

        T Visit<T>(ProgramNode programNode);

        T Visit<T>(BlockNode blockNode);

        T Visit<T>(DeclarationNode declarationNode);

        void Visit(TypeNode typeNode);

        T Visit<T>(CompoundNode compoundNode);

        T Visit<T>(AssignmentNode assignmentNode);
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