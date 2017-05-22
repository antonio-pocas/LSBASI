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
        public List<ExecutionContext> ExecutionContexts { get; set; }
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
            ExecutionContexts = new List<ExecutionContext>();
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

            //TODO use information from static analysis here, try to figure out if we can get rid of AddAtDepth or LookupInfo
            var variableInfo = currentScope.LookupInfo<TypedSymbol>(name);

            var variable = variableInfo.Symbol;

            if (variable.Type != result.Type)
            {
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
            var name = node.Name;
            var function = node.Metadata.Reference as FunctionSymbol;
            var parameters = function.Parameters;
            var arguments = node.Arguments;

            var procedureMemory = new StackFrame(name, currentStackFrame.Depth + 1, currentStackFrame);
            for (int i = 0; i < parameters.Count; i++)
            {
                procedureMemory[parameters[i].Name] = arguments[i].Yield(this);
            }

            currentStackFrame = procedureMemory;
            var myScope = currentScope;
            currentScope = function.Scope;
#if DEBUG
            AddScopes();
#endif
            function.Reference.Block.Accept(this);

            currentScope = myScope;
            currentStackFrame = procedureMemory.PreviousFrame;
        }

        public void Visit(ProcedureCallNode node)
        {
            var name = node.Name;
            var procedure = node.Metadata.Reference as ProcedureSymbol;
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

        public void Visit(BooleanNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ComparisonOperationNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(IfNode node)
        {
            var conditionValue = node.Condition.Yield(this);
            var condition = BooleanSymbol.GetValue(conditionValue);
            if (condition)
            {
                node.Then.Accept(this);
            }
            else
            {
                node.Else?.Accept(this);
            }
        }

        public TypedValue Evaluate(NumberNode node)
        {
            return node.Metadata.Value;
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

            var type = value.Type as NumberTypeSymbol;

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

            var numberType = node.Metadata.Type as NumberTypeSymbol;

            switch (node.Type)
            {
                case BinaryOperationType.Add:
                    return numberType.Add(leftValue, rightValue);

                case BinaryOperationType.Subtract:
                    return numberType.Subtract(leftValue, rightValue);

                case BinaryOperationType.Multiply:
                    return numberType.Multiply(leftValue, rightValue);

                case BinaryOperationType.IntegerDivision:
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
            var function = node.Metadata.Reference as FunctionSymbol;
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

        public TypedValue Evaluate(BooleanNode node)
        {
            return node.Metadata.Value;
        }

        public TypedValue Evaluate(ComparisonOperationNode node)
        {
            var left = node.Left.Yield(this);
            var right = node.Right.Yield(this);

            if (node.Type == ComparisonOperationType.Equals)
            {
                return left.Type.Equals(left, right);
            }

            if (node.Type == ComparisonOperationType.Differs)
            {
                return left.Type.Differs(left, right);
            }

            var leftAsReal = left.Type as RealSymbol;
            var rightAsReal = right.Type as RealSymbol;
            NumberTypeSymbol finalType;
            if (leftAsReal != null || (rightAsReal != null))
            {
                finalType = BuiltinType.Real;
            }
            else
            {
                finalType = BuiltinType.Integer;
            }

            switch (node.Type)
            {
                case ComparisonOperationType.GreaterThan:
                    return finalType.GreaterThan(left, right);

                case ComparisonOperationType.GreaterThanOrEqual:
                    return finalType.GreaterThanOrEqual(left, right);

                case ComparisonOperationType.LessThan:
                    return finalType.LessThan(left, right);

                case ComparisonOperationType.LessThanOrEqual:
                    return finalType.LessThanOrEqual(left, right);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if DEBUG

        private void AddScopes()
        {
            ExecutionContexts.Add(new ExecutionContext()
            {
                Scope = currentScope,
                StackFrame = currentStackFrame
            });
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

#if DEBUG

        public sealed class ExecutionContext
        {
            public StackFrame StackFrame { get; set; }
            public ScopedSymbolTable Scope { get; set; }
        }

#endif
    }
}