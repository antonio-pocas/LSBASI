using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace LSBASI3
{
    //TODO make SemanticAnalyzer build a simplified AST
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

            node.Metadata.Reference = function;
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

            node.Metadata.Reference = function;
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

            node.Metadata.Reference = procedure;
        }

        public void Visit(BooleanNode node)
        {
            //TODO??
            throw new NotImplementedException();
        }

        public void Visit(ComparisonOperationNode node)
        {
            //TODO??
            throw new NotImplementedException();
        }

        public void Visit(IfNode node)
        {
            var conditionType = node.Condition.Yield(this);
            if (conditionType != BuiltinType.Boolean)
            {
                throw new InvalidExpressionException("If keyword must always be followed by a boolean expression");
            }

            node.Then.Accept(this);
            node.Else?.Accept(this);
        }

        public TypeSymbol Evaluate(BooleanNode node)
        {
            if (node.Token.Type == TokenType.True)
            {
                node.Metadata.Value = BooleanSymbol.True;
                return BuiltinType.Boolean;
            }

            if (node.Token.Type == TokenType.False)
            {
                node.Metadata.Value = BooleanSymbol.False;
                return BuiltinType.Boolean;
            }

            throw new Exception("Boolean must be true or false");
        }

        public TypeSymbol Evaluate(ComparisonOperationNode node)
        {
            var left = node.Left.Yield(this);
            var right = node.Right.Yield(this);

            if (!TypeChecker.CheckComparisonOperation(node.Type, left, right))
            {
                throw new InvalidOperationException(
                    $"Cannot perform comparison {node.Type.ToString()} between types {left} and {right}");
            }

            return BuiltinType.Boolean;
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

            variableNode.Metadata.Reference = variable;
        }

        public void Visit(NoOpNode node)
        {
        }

        public void Visit(AssignmentNode node)
        {
            node.Variable.Accept(this);
            var variable = node.Variable.Metadata.Reference as VarSymbol;

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
            if (node.Token.Type == TokenType.IntegerConstant)
            {
                node.Metadata.Value = new TypedValue(BuiltinType.Integer, int.Parse(node.Value));
                return BuiltinType.Integer;
            }

            if (node.Token.Type == TokenType.RealConstant)
            {
                node.Metadata.Value = new TypedValue(BuiltinType.Real, double.Parse(node.Value, CultureInfo.InvariantCulture));
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

            node.Metadata.Reference = variable;

            return variable.Type;
        }

        public TypeSymbol Evaluate(UnaryOperationNode node)
        {
            node.Child = FixParenlessFunctionCalls(node.Child);
            var type = node.Child.Yield(this);

            var resultType = TypeChecker.CheckUnaryOperation(type);

            node.Metadata.Type = resultType;
            return resultType;
        }

        public TypeSymbol Evaluate(BinaryOperationNode node)
        {
            node.Left = FixParenlessFunctionCalls(node.Left);
            node.Right = FixParenlessFunctionCalls(node.Right);

            var leftType = node.Left.Yield(this);
            var rightType = node.Right.Yield(this);

            var resultType = TypeChecker.CheckBinaryOperation(node.Type, leftType, rightType);
            node.Metadata.Type = resultType;

            return resultType;
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

    public static class TypeChecker
    {
        public static bool AreCompatible(TypeSymbol expected, TypeSymbol actual)
        {
            return expected == actual || actual.CanCastTo(expected);
        }

        public static bool AreComparable(TypeSymbol left, TypeSymbol right)
        {
            return left == right || right.CanCastTo(left) || left.CanCastTo(right);
        }

        public static bool CheckComparisonOperation(ComparisonOperationType operation, TypeSymbol left, TypeSymbol right)
        {
            if (!TypeChecker.AreComparable(left, right))
            {
                return false;
            }

            if (operation == ComparisonOperationType.Equals || operation == ComparisonOperationType.Differs)
            {
                return true;
            }

            var leftAsNumber = left as NumberTypeSymbol;
            var rightAsNumber = right as NumberTypeSymbol;

            return leftAsNumber != null && rightAsNumber != null;
        }

        public static NumberTypeSymbol CheckBinaryOperation(BinaryOperationType operation, TypeSymbol left, TypeSymbol right)
        {
            var leftAsNumber = left as NumberTypeSymbol;
            var rightAsNumber = right as NumberTypeSymbol;

            if (leftAsNumber == null || rightAsNumber == null)
            {
                throw new TypeAccessException($"Cannot perform operation {operation.ToString()} between types {left} and {right}");
            }

            if (operation == BinaryOperationType.IntegerDivision
                && (leftAsNumber == BuiltinType.Real || rightAsNumber == BuiltinType.Real))
            {
                throw new TypeAccessException($"Cannot perform operation {BinaryOperationType.IntegerDivision.ToString()} between types {left} and {right}");
            }

            if (operation == BinaryOperationType.RealDivision)
            {
                return BuiltinType.Real;
            }

            if (leftAsNumber == rightAsNumber || rightAsNumber.CanCastTo(leftAsNumber))
            {
                return leftAsNumber;
            }

            if (leftAsNumber.CanCastTo(rightAsNumber))
            {
                return rightAsNumber;
            }

            throw new TypeAccessException($"Cannot perform operation {operation.ToString()} between types {left} and {right}");
        }

        public static NumberTypeSymbol CheckUnaryOperation(TypeSymbol type)
        {
            var numberType = type as NumberTypeSymbol;

            if (numberType == null)
            {
                throw new TypeAccessException($"Cannot perform a unary operation on the type {type}");
            }

            return numberType;
        }
    }
}