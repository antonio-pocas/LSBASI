using System;
using System.Collections.Generic;

namespace LSBASI3
{
    public class AssignmentAnalyzer : IVisitor
    {
        private ScopedSymbolTable currentScope;
        private ScopedAnalysisTable currentAnalysisScope;
        public List<string> Warnings { get; private set; }

        public AssignmentAnalyzer()
        {
            currentAnalysisScope = new ScopedAnalysisTable(null);
            Warnings = new List<string>();
        }

        public List<string> Analyze(ProgramNode program, ScopedSymbolTable globalScope)
        {
            currentScope = globalScope;
            program.Accept(this);

            return Warnings;
        }

        public void Visit(ProgramNode node)
        {
            currentScope = currentScope.Lookup<ProgramSymbol>(node.Name).Scope;

            node.Block.Accept(this);

            currentScope = currentScope.EnclosingScope;
        }

        public void Visit(BlockNode node)
        {
            node.Compound.Accept(this);
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

            node.Result.Accept(this);

            var symbol = currentScope.Lookup(name);
            currentAnalysisScope[name] = new SymbolInfo()
            {
                Depth = currentScope.Level,
                HasValue = true,
                Type = SymbolType.Variable,
                Scope = currentScope,
                Symbol = symbol,
            };
        }

        public void Visit(VariableNode variableNode)
        {
            var name = variableNode.Name;
            var symbolInfo = currentAnalysisScope[name];
            if (symbolInfo == null || !symbolInfo.HasValue)
            {
                var symbol = currentScope.Lookup(name);
                Warnings.Add($"Use of unassigned variable {symbol}");
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

        public void Visit(ProcedureCallNode node)
        {
            var myScope = currentScope;
            var myAnalysisScope = currentAnalysisScope;
            var procedure = currentScope.Lookup<ProcedureSymbol>(node.Name);

            currentAnalysisScope = new ScopedAnalysisTable(currentAnalysisScope);
            currentScope = procedure.Scope;

            var parameters = procedure.Parameters;

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                currentAnalysisScope[parameter.Name] = new SymbolInfo()
                {
                    Depth = currentScope.Level,
                    HasValue = true,
                    Type = SymbolType.Variable,
                    Symbol = parameter,
                    Scope = currentScope
                };
            }

            procedure.Reference.Block.Compound.Accept(this);

            currentScope = myScope;
            currentAnalysisScope = myAnalysisScope;
        }

        public void Visit(FunctionCallNode node)
        {
            var myScope = currentScope;
            var myAnalysisScope = currentAnalysisScope;
            var function = currentScope.Lookup<FunctionSymbol>(node.Name);

            currentAnalysisScope = new ScopedAnalysisTable(currentAnalysisScope);
            currentScope = function.Scope;

            var parameters = function.Parameters;

            foreach (var parameter in parameters)
            {
                currentAnalysisScope[parameter.Name] = new SymbolInfo()
                {
                    Depth = currentScope.Level,
                    HasValue = true,
                    Type = SymbolType.Variable,
                    Symbol = parameter,
                    Scope = currentScope
                };
            }

            function.Reference.Block.Compound.Accept(this);

            currentScope = myScope;
            currentAnalysisScope = myAnalysisScope;
        }

        #region unused methods

        public void Visit(TypeNode typeNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(ProcedureNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(DeclarationNode node)
        {
            throw new NotImplementedException();
        }

        #endregion unused methods
    }
}