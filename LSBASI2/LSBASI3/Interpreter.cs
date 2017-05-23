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
            // maybe use the symbol as the stack frame key, that way we can just add the symbol during semantic analysis
            // and use that instead. That way there's no need for AddAtDepth
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
            var function = node.Function;
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
            var procedure = node.Procedure;
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
            return node.TypedValue;
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
            return node.Operation(value);
        }

        public TypedValue Evaluate(BinaryOperationNode node)
        {
            var left = node.Left.Yield(this);
            var right = node.Right.Yield(this);

            return node.Operation(left, right);
        }

        public TypedValue Evaluate(FunctionCallNode node)
        {
            var name = node.Name;
            var function = node.Function;
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
            return node.TypedValue;
        }

        public TypedValue Evaluate(ComparisonOperationNode node)
        {
            var left = node.Left.Yield(this);
            var right = node.Right.Yield(this);

            return node.Operation(left, right);
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

        public TypedValue Evaluate(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(BooleanNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ComparisonOperationNode node)
        {
            throw new NotImplementedException();
        }

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