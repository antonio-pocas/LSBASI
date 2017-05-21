using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class NodeMetadata
    {
        public TypeSymbol Type { get; set; }
        public Symbol Reference { get; set; }
    }

    public abstract class AstNode
    {
        public readonly Token Token;
        public NodeMetadata Metadata { get; set; }

        public AstNode()
        {
            Metadata = new NodeMetadata();
        }

        public AstNode(Token token)
        {
            Metadata = new NodeMetadata();
            this.Token = token;
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
        public List<ParameterNode> FormalParameters { get; set; }
        public BlockNode Block { get; set; }

        public ProcedureNode(string name, List<ParameterNode> formalParameters, BlockNode block)
        {
            Name = name;
            Block = block;
            FormalParameters = formalParameters;
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

    public class FunctionNode : ProcedureNode
    {
        public TypeNode Type { get; set; }

        public FunctionNode(string name, List<ParameterNode> formalParameters, BlockNode block, TypeNode type) : base(name, formalParameters, block)
        {
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

    public class ParameterNode : AstNode
    {
        public VariableNode Variable { get; set; }
        public TypeNode Type { get; set; }

        public ParameterNode(VariableNode variable, TypeNode type)
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

    public class ProcedureCallNode : AstNode
    {
        public string Name { get; set; }
        public List<AstNode> Arguments { get; set; }

        public ProcedureCallNode(string name, List<AstNode> arguments)
        {
            Name = name;
            Arguments = arguments;
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

    public class IfNode : AstNode
    {
        public AstNode Condition { get; set; }
        public AstNode Then { get; set; }
        public AstNode Else { get; set; }

        public IfNode(AstNode condition, AstNode then)
        {
            Condition = condition;
            Then = then;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Yield<T>(IEvaluator<T> visitor)
        {
            throw new NotImplementedException();
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

    public class FunctionCallNode : ProcedureCallNode
    {
        public Type Type { get; set; }

        public FunctionCallNode(string name, List<AstNode> arguments) : base(name, arguments)
        {
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

    public class ComparisonOperationNode : AstNode
    {
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }
        public ComparisonOperationType Type { get; set; }

        public ComparisonOperationNode(Token token, AstNode left, AstNode right) : base(token)
        {
            this.Left = left;
            this.Right = right;
            switch (token.Type)
            {
                case TokenType.Equals:
                    this.Type = ComparisonOperationType.Equals;
                    break;

                case TokenType.Differs:
                    this.Type = ComparisonOperationType.Differs;
                    break;

                case TokenType.GreaterThan:
                    this.Type = ComparisonOperationType.GreaterThan;
                    break;

                case TokenType.GreaterThanOrEqual:
                    this.Type = ComparisonOperationType.GreaterThanOrEqual;
                    break;

                case TokenType.LessThan:
                    this.Type = ComparisonOperationType.LessThan;
                    break;

                case TokenType.LessThanOrEqual:
                    this.Type = ComparisonOperationType.LessThanOrEqual;
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

    public class BooleanNode : AstNode
    {
        public bool Value { get; set; }

        public BooleanNode(Token token, bool value) : base(token)
        {
            Value = value;
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

    public enum ComparisonOperationType
    {
        Equals,
        Differs,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
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

        void Visit(FunctionNode node);

        void Visit(FunctionCallNode node);

        void Visit(ParameterNode node);

        void Visit(ProcedureCallNode node);

        void Visit(BooleanNode node);

        void Visit(ComparisonOperationNode node);

        void Visit(IfNode node);
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

        T Evaluate(ParameterNode node);

        T Evaluate(ProcedureCallNode node);

        T Evaluate(FunctionNode node);

        T Evaluate(FunctionCallNode node);

        T Evaluate(BooleanNode node);

        T Evaluate(ComparisonOperationNode node);
    }
}