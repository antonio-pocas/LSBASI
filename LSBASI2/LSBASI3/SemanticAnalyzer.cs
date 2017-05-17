using System;

namespace LSBASI3
{
    public class SemanticAnalyzer : IVisitor, IEvaluator<TypeSymbol>
    {
        private SymbolTable table;

        public SemanticAnalyzer()
        {
            this.table = new SymbolTable();
        }

        public SymbolTable Analyze(ProgramNode program)
        {
            program.Accept(this);
            var ret = table;
            table = new SymbolTable();
            return ret;
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
            var symbol = table.Lookup<VarSymbol>(name);
            if (symbol == null)
            {
                throw new Exception($"Assignment of uninitialized variable {name}");
            }

            var valueType = node.Result.Yield(this);

            if (valueType != symbol.Type && !valueType.CanCastTo(symbol.Type))
            {
                throw new TypeAccessException($"Cannot assign value of type {valueType} to variable {name} of type {symbol.Type}");
            }
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
            var symbol = new VarSymbol(node.Variable.Name, table.Lookup<BuiltinTypeSymbol>(node.Type.Value));
            this.table.Define(symbol);
        }

        public void Visit(VariableNode variableNode)
        {
            var name = variableNode.Name;
            if (table.Lookup<VarSymbol>(name) == null)
            {
                throw new Exception($"Use of uninitialized variable {name}");
            }
        }

        public void Visit(BinaryOperationNode binaryOperationNode)
        {
            binaryOperationNode.Left.Accept(this);
            binaryOperationNode.Right.Accept(this);
        }

        public void Visit(UnaryOperationNode unaryOperationNode)
        {
            unaryOperationNode.Child.Accept(this);
        }

        public void Visit(NumberNode numberNode)
        {
        }

        public void Visit(TypeNode typeNode)
        {
        }

        public void Visit(ProcedureNode node)
        {
        }

        public TypeSymbol Evaluate(NumberNode node)
        {
            if (node.token.Type == TokenType.IntegerConstant)
            {
                return BuiltinType.Integer;
            }

            if (node.token.Type == TokenType.RealConstant)
            {
                return BuiltinType.Real;
            }

            throw new TypeAccessException("Number must be a real or integer");
        }

        public TypeSymbol Evaluate(VariableNode node)
        {
            var variable = table.Lookup<TypedSymbol>(node.Name);
            if (variable == null)
            {
                throw new TypeAccessException($"Use of undeclared variable {node.Name}");
            }

            return variable.Type;
        }

        public TypeSymbol Evaluate(UnaryOperationNode node)
        {
            var type = node.Child.Yield(this);
            return TypeChecker.CheckUnaryOperation(type);
        }

        public TypeSymbol Evaluate(BinaryOperationNode node)
        {
            var leftType = node.Left.Yield(this);
            var rightType = node.Right.Yield(this);

            return TypeChecker.CheckBinaryOperation(node.Type, leftType, rightType);
        }

        public TypeSymbol Evaluate(TypeNode node)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(NoOpNode noOpNode)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(ProgramNode programNode)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(BlockNode blockNode)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(DeclarationNode declarationNode)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(CompoundNode compoundNode)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(AssignmentNode assignmentNode)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(ProcedureNode node)
        {
            throw new NotImplementedException();
        }
    }
}