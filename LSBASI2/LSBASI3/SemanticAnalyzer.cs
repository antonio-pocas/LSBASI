using System;
using System.Collections.Generic;
using System.Linq;

namespace LSBASI3
{
    public class SemanticAnalyzer : IVisitor, IEvaluator<TypeSymbol>
    {
        private ScopedSymbolTable currentScope;

        public SemanticAnalyzer()
        {
        }

        public ScopedSymbolTable Analyze(ProgramNode program, ScopedSymbolTable globalScope)
        {
            currentScope = globalScope;
            program.Accept(this);

            return currentScope;
        }

        public void Visit(ProgramNode node)
        {
            currentScope = currentScope.Lookup<ProgramSymbol>(node.Name).Scope;

            node.Block.Accept(this);

            currentScope = currentScope.EnclosingScope;
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

        public void Visit(ProcedureNode node)
        {
            var name = node.Name;

            var myScope = currentScope;

            var procedureScope = currentScope.Lookup<ProcedureSymbol>(name).Scope;
            currentScope = procedureScope;

            node.Block.Accept(this);

            currentScope = myScope;
        }

        public void Visit(FunctionNode node)
        {
            var name = node.Name;

            var myScope = currentScope;

            var functionScope = currentScope.Lookup<FunctionSymbol>(name).Scope;
            currentScope = functionScope;

            node.Block.Accept(this);

            currentScope = myScope;
        }

        public void Visit(FunctionCallNode node)
        {
            var name = node.Name;
            var function = currentScope.Lookup<FunctionSymbol>(name);
            if (function == null)
            {
                throw new MissingMethodException($"Use of undefined function {name}");
            }

            var parameters = function.Parameters;
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

        public TypeSymbol Evaluate(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol Evaluate(FunctionCallNode node)
        {
            var name = node.Name;
            var function = currentScope.Lookup<FunctionSymbol>(name);
            if (function == null)
            {
                throw new MissingMethodException($"Use of undefined function {name}");
            }

            var parameters = function.Parameters;
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

            return function.Type;
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
            var symbol = currentScope.Lookup(name);

            var variable = symbol as VarSymbol;

            if (variable == null)
            {
                if (symbol != null)
                {
                    throw new Exception($"Cannot assign value to symbol {symbol}");
                }
                throw new Exception($"Use of undeclared variable {name}");
            }
        }

        public void Visit(NoOpNode node)
        {
        }

        public void Visit(AssignmentNode node)
        {
            var name = node.Variable.Name;
            var symbol = currentScope.Lookup(name);

            var variable = symbol as VarSymbol;

            if (variable == null)
            {
                if (symbol != null)
                {
                    throw new Exception($"Cannot assign value to symbol {symbol}");
                }
                throw new Exception($"Use of undeclared variable {name}");
            }

            node.Result = FixParenlessFunctionCalls(node.Result);

            var valueType = node.Result.Yield(this);

            if (!TypeChecker.AreCompatible(variable.Type, valueType))
            {
                throw new TypeAccessException(
                    $"Cannot assign value of type {valueType} to variable {variable}");
            }
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
            var name = node.Name;
            var variable = currentScope.Lookup<TypedSymbol>(name);
            if (variable == null)
            {
                throw new TypeAccessException($"Use of undeclared variable or function {name}");
            }

            return variable.Type;
        }

        public TypeSymbol Evaluate(UnaryOperationNode node)
        {
            node.Child = FixParenlessFunctionCalls(node.Child);
            var type = node.Child.Yield(this);
            return TypeChecker.CheckUnaryOperation(type);
        }

        public TypeSymbol Evaluate(BinaryOperationNode node)
        {
            node.Left = FixParenlessFunctionCalls(node.Left);
            node.Right = FixParenlessFunctionCalls(node.Right);

            var leftType = node.Left.Yield(this);
            var rightType = node.Right.Yield(this);

            return TypeChecker.CheckBinaryOperation(node.Type, leftType, rightType);
        }

        private AstNode FixParenlessFunctionCalls(AstNode node)
        {
            var nodeAsVariable = node as VariableNode;
            if (nodeAsVariable != null)
            {
                var functionSymbol = currentScope.Lookup<TypedSymbol>(nodeAsVariable.Name) as FunctionSymbol;
                if (functionSymbol != null)
                {
                    return new FunctionCallNode(nodeAsVariable.Name, new List<AstNode>());
                }
            }

            return node;
        }

        #region unused methods

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

        #endregion unused methods
    }
}