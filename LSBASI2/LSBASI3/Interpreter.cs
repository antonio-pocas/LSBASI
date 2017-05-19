using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class Interpreter : IVisitor, IEvaluator<TypedValue>
    {
        private readonly AstNode rootNode;
        private readonly ScopedSymbolTable scopedSymbolTable;
        private readonly ScopedMemory scopedMemory;

        public Interpreter(Parser parser, SemanticAnalyzer semanticAnalyzer)
        {
            var node = parser.Parse();
            this.rootNode = node;
            this.scopedSymbolTable = semanticAnalyzer.Analyze(node);
            this.scopedMemory = ScopedMemory.ProgramMemory(node.Name);
        }

        public void Interpret()
        {
            this.rootNode.Accept(this);
        }

        public void Visit(ProgramNode node)
        {
            node.Block.Accept(this);
        }

        public void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
            {
                declaration.Accept(this);
            }

            node.Compound.Accept(this);
        }

        public void Visit(DeclarationNode node)
        {
        }

        public void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public void Visit(NoOpNode node)
        {
        }

        public void Visit(AssignmentNode node)
        {
            var name = node.Variable.Name;
            var result = node.Result.Yield(this);
            var variable = scopedSymbolTable.Lookup<TypedSymbol>(name);

            if (variable.Type != result.Type)
            {
                if (!result.Type.CanCastTo(variable.Type))
                {
                    throw new TypeAccessException(
                        $"Cannot assign value of type {result.Type} to variable {variable}");
                }
                result = variable.Type.Cast(result);
            }

            scopedMemory[name] = result;
        }

        public void Visit(VariableNode variableNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(BinaryOperationNode binaryOperationNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(UnaryOperationNode unaryOperationNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(NumberNode numberNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(TypeNode typeNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(ProcedureNode node)
        {
        }

        public void Visit(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ProcedureCallNode node)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(NumberNode node)
        {
            if (node.token.Type == TokenType.IntegerConstant)
            {
                return new TypedValue(BuiltinType.Integer, int.Parse(node.Value));
            }

            if (node.token.Type == TokenType.RealConstant)
            {
                return new TypedValue(BuiltinType.Real, double.Parse(node.Value, CultureInfo.InvariantCulture));
            }

            throw new Exception("Unsupported type for number node");
        }

        public TypedValue Evaluate(VariableNode node)
        {
            var name = node.Name;
            var value = scopedMemory[name];

            if (value == null)
            {
                throw new Exception($"Use of unassigned variable {name}");
            }

            return value;
        }

        public TypedValue Evaluate(UnaryOperationNode node)
        {
            var value = node.Child.Yield(this);

            NumberTypeSymbol type;
            try
            {
                type = TypeChecker.CheckUnaryOperation(value.Type);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Unsupported value {value} for operation {node.Type.ToString()}");
            }

            switch (node.Type)
            {
                case UnaryOperationType.Plus:
                    return type.Plus(value);

                case UnaryOperationType.Minus:
                    return type.Minus(value);
            }

            throw new Exception();
        }

        public TypedValue Evaluate(BinaryOperationNode node)
        {
            var leftValue = node.Left.Yield(this);
            var rightValue = node.Right.Yield(this);

            NumberTypeSymbol numberType;
            try
            {
                numberType = TypeChecker.CheckBinaryOperation(node.Type, leftValue.Type, rightValue.Type);
            }
            catch (TypeAccessException ex)
            {
                throw new InvalidOperationException($"Cannot perform operation {node.Type.ToString()} between value {leftValue} and {rightValue}", ex);
            }

            switch (node.Type)
            {
                case BinaryOperationType.Add:
                    return numberType.Add(leftValue, rightValue);

                case BinaryOperationType.Subtract:
                    return numberType.Subtract(leftValue, rightValue);

                case BinaryOperationType.Multiply:
                    return numberType.Multiply(leftValue, rightValue);

                case BinaryOperationType.IntegerDivision:
                    if (numberType != BuiltinType.Integer)
                    {
                        throw new InvalidOperationException($"Integer division may only be done on integers");
                    }
                    return BuiltinType.Integer.Divide(leftValue, rightValue);

                case BinaryOperationType.RealDivision:
                    return BuiltinType.Real.Divide(leftValue, rightValue);
            }

            throw new Exception();
        }

        public TypedValue Evaluate(TypeNode node)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(NoOpNode noOpNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(ProgramNode programNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(BlockNode blockNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(DeclarationNode declarationNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(CompoundNode compoundNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(AssignmentNode assignmentNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(ProcedureNode node)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(ProcedureCallNode node)
        {
            throw new NotImplementedException();
        }
    }
}