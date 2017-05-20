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
        private readonly ScopedSymbolTableBuilder scopedSymbolTableBuilder;
        private readonly SemanticAnalyzer semanticAnalyzer;
        private readonly AssignmentAnalyzer assignmentAnalyzer;
        private readonly ProgramNode rootNode;
        private ScopedSymbolTable currentScope;
        private StackFrame currentStackFrame;
#if DEBUG
        public HashSet<ScopedSymbolTable> Scopes { get; set; }
        public HashSet<StackFrame> StackFrames { get; set; }
#endif

        public Interpreter(Parser parser, ScopedSymbolTableBuilder scopedSymbolTableBuilder, SemanticAnalyzer semanticAnalyzer, AssignmentAnalyzer assignmentAnalyzer)
        {
            this.scopedSymbolTableBuilder = scopedSymbolTableBuilder;
            this.semanticAnalyzer = semanticAnalyzer;
            this.assignmentAnalyzer = assignmentAnalyzer;

            var node = parser.Parse();
            this.rootNode = node;

            this.currentScope = scopedSymbolTableBuilder.Build(node);

            this.currentStackFrame = StackFrame.ProgramMemory(node.Name);
#if DEBUG
            Scopes = new HashSet<ScopedSymbolTable>();
            StackFrames = new HashSet<StackFrame>();
#endif
        }

        public void Interpret()
        {
            semanticAnalyzer.Analyze(this.rootNode, this.currentScope);
            var warnings = assignmentAnalyzer.Analyze(this.rootNode, this.currentScope);
            this.rootNode.Accept(this);
        }

        public void Visit(ProgramNode node)
        {
#if DEBUG
            AddScopes();
#endif
            var program = currentScope.Lookup<ProgramSymbol>(node.Name);
            currentScope = program.Scope;
            currentStackFrame = new StackFrame("Global", 1, currentStackFrame);
#if DEBUG
            AddScopes();
#endif
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
            var variableInfo = currentScope.LookupInfo<TypedSymbol>(name);

            var variable = variableInfo.Symbol;

            if (variable.Type != result.Type)
            {
                if (!result.Type.CanCastTo(variable.Type))
                {
                    throw new TypeAccessException(
                        $"Cannot assign value of type {result.Type} to variable {variable}");
                }
                result = variable.Type.Cast(result);
            }

            currentStackFrame.AddAtDepth(name, result, variableInfo.ScopeLevel);
        }

        public void Visit(ProcedureNode node)
        {
        }

        public void Visit(FunctionNode node)
        {
        }

        public void Visit(FunctionCallNode node)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void Visit(ProcedureCallNode node)
        {
            var name = node.Name;
            var procedure = currentScope.Lookup<ProcedureSymbol>(name);
            var parameters = procedure.Parameters;
            var arguments = node.Arguments;

            var procedureMemory = new StackFrame(name, currentStackFrame.Depth + 1, currentStackFrame);
            for (int i = 0; i < parameters.Count; i++)
            {
                procedureMemory[parameters[i].Name] = arguments[i].Yield(this);
            }

            currentStackFrame = procedureMemory;
            var myScope = currentScope;
            currentScope = procedure.Scope;
#if DEBUG
            AddScopes();
#endif
            procedure.Reference.Block.Accept(this);

            currentScope = myScope;
            currentStackFrame = procedureMemory.PreviousFrame;
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
            var value = currentStackFrame[name];

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
                throw new InvalidOperationException($"Unsupported value {value} for operation {node.Type.ToString()}", ex);
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

        public TypedValue Evaluate(FunctionNode node)
        {
            //TODO

            throw new NotImplementedException();
        }

        public TypedValue Evaluate(FunctionCallNode node)
        {
            var name = node.Name;
            var function = currentScope.Lookup<FunctionSymbol>(name);
            var parameters = function.Parameters;
            var arguments = node.Arguments;

            var functionStackFrame = new StackFrame(name, currentStackFrame.Depth + 1, currentStackFrame);
            for (int i = 0; i < parameters.Count; i++)
            {
                functionStackFrame[parameters[i].Name] = arguments[i].Yield(this);
            }

            currentStackFrame = functionStackFrame;
            var myScope = currentScope;
            currentScope = function.Scope;
#if DEBUG
            AddScopes();
#endif
            function.Reference.Block.Accept(this);

            var result = currentStackFrame[name];

            currentScope = myScope;
            currentStackFrame = functionStackFrame.PreviousFrame;

            return result;
        }

#if DEBUG

        private void AddScopes()
        {
            Scopes.Add(currentScope);
            StackFrames.Add(currentStackFrame);
        }

#endif

        #region unused methods

        public void Visit(ParameterNode node)
        {
            throw new NotImplementedException();
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

        #endregion unused methods
    }
}