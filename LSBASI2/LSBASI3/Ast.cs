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

        public abstract T Yield<T>(IEvaluator<T> visitor);
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

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
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

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
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

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
        }
    }

    public class ProcedureNode : AstNode
    {
        public string Name { get; set; }
        public BlockNode Block { get; set; }

        public ProcedureNode(string name, BlockNode block)
        {
            Name = name;
            Block = block;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
        }
    }

    public class TypeNode : AstNode
    {
        public string Value { get; set; }

        public TypeNode(Token token) : base(token)
        {
            this.Value = token.Value;
        }

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
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

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
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

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
        }
    }

    public class NoOpNode : AstNode
    {
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
        }
    }

    public class VariableNode : AstNode
    {
        public string Name { get; set; }

        public VariableNode(Token token) : base(token)
        {
            this.Name = token.Value;
        }

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
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
                    this.Type = BinaryOperationType.IntegerDivision;
                    break;

                case TokenType.Slash:
                    this.Type = BinaryOperationType.RealDivision;
                    break;

                case TokenType.Star:
                    this.Type = BinaryOperationType.Multiply;
                    break;
            }
        }

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
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

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NumberNode : AstNode
    {
        public string Value { get; set; }

        public NumberNode(Token token) : base(token)
        {
            this.Value = token.Value;
        }

        public override T Yield<T>(IEvaluator<T> evaluator)
        {
            return evaluator.Evaluate(this);
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
        IntegerDivision,
        RealDivision
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

        void Visit(ProgramNode node);

        void Visit(BlockNode node);

        void Visit(DeclarationNode node);

        void Visit(VariableNode variableNode);

        void Visit(BinaryOperationNode binaryOperationNode);

        void Visit(UnaryOperationNode unaryOperationNode);

        void Visit(NumberNode numberNode);

        void Visit(TypeNode typeNode);

        void Visit(ProcedureNode node);
    }

    public interface IEvaluator<T>
    {
        T Evaluate(NumberNode node);

        T Evaluate(VariableNode node);

        T Evaluate(UnaryOperationNode node);

        T Evaluate(BinaryOperationNode node);

        T Evaluate(TypeNode node);

        T Evaluate(NoOpNode noOpNode);

        T Evaluate(ProgramNode programNode);

        T Evaluate(BlockNode blockNode);

        T Evaluate(DeclarationNode declarationNode);

        T Evaluate(CompoundNode compoundNode);

        T Evaluate(AssignmentNode assignmentNode);

        T Evaluate(ProcedureNode node);
    }
}