using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LSBASI3
{
    public class SemanticAnalyzer : IVisitor, IEvaluator<TypeSymbol>
    {
        private ScopedSymbolTable currentScope;

        public SemanticAnalyzer()
        {
        }

        public ScopedSymbolTable Analyze(ProgramNode program)
        {
            program.Accept(this);

            return currentScope;
        }

        public void Visit(ProgramNode node)
        {
            var programScope = ScopedSymbolTable.CreateProgramScope(node.Name);
            currentScope = new ScopedSymbolTable("Global", 1, programScope);
            node.Block.Accept(this);

            currentScope = currentScope.EnclosingScope;
        }

        public void Visit(ProcedureNode node)
        {
            var name = node.Name;

            var procedureScope = new ScopedSymbolTable(name, currentScope.Level + 1, currentScope);
            currentScope = procedureScope;
            var formalParameters = new List<VarSymbol>();

            foreach (var parameter in node.FormalParameters)
            {
                var parameterSymbol = new VarSymbol(parameter.Variable.Name, currentScope.Lookup<TypeSymbol>(parameter.Type.Value));
                formalParameters.Add(parameterSymbol);
                currentScope.Define(parameterSymbol);
            }

            node.Block.Accept(this);

            currentScope = currentScope.EnclosingScope;
            currentScope.Define(new ProcedureSymbol(name, formalParameters, procedureScope, node));
        }

        public void Visit(ParameterNode node)
        {
        }

        public void Visit(ProcedureCallNode node)
        {
            var name = node.Name;
            var procedure = currentScope.Lookup<ProcedureSymbol>(name);
            if (procedure == null)
            {
                throw new MissingMethodException($"Use of undefined procedure {name}");
            }

            var parameters = procedure.Parameters;
            var arguments = node.Arguments;
            if (arguments.Count != parameters.Count)
            {
                throw new ArgumentException(
                    $"Error in call to {name}, number of arguments ({arguments.Count}) differs from number of defined parameters ({parameters.Count})");
            }

            for (int i = 0; i < arguments.Count; i++)
            {
                var argumentType = arguments[i].Yield(this);
                if (!TypeChecker.AreCompatible(parameters[i].Type, argumentType))
                {
                    throw new ArgumentException($"Error in call to {name}, cannot pass argument of type {argumentType} to parameter {parameters[i]}");
                }
            }
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
            var symbol = new VarSymbol(node.Variable.Name, currentScope.Lookup<BuiltinTypeSymbol>(node.Type.Value));
            this.currentScope.Define(symbol);
        }

        public void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public void Visit(VariableNode variableNode)
        {
            var name = variableNode.Name;
            if (currentScope.Lookup<VarSymbol>(name) == null)
            {
                throw new Exception($"Use of undeclared variable {name}");
            }
        }

        public void Visit(NoOpNode node)
        {
        }

        public void Visit(AssignmentNode node)
        {
            var name = node.Variable.Name;
            var variable = currentScope.Lookup<VarSymbol>(name);
            if (variable == null)
            {
                throw new Exception($"Use of undeclared variable {name}");
            }

            var valueType = node.Result.Yield(this);

            if (valueType != variable.Type && !valueType.CanCastTo(variable.Type))
            {
                throw new TypeAccessException(
                    $"Cannot assign value of type {valueType} to variable {variable}");
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
            var variable = currentScope.Lookup<TypedSymbol>(node.Name);
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

        public TypeSymbol Evaluate(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(ProcedureCallNode node)
        {
            throw new NotImplementedException();
        }
    }
}