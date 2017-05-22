using System;
using System.Collections.Generic;
using System.Linq;

namespace LSBASI3
{
    public class AnalysisBranch
    {
        public AnalysisBranch()
        {
        }
    }

    public class AssignmentAnalyzer : IVisitor
    {
        private ScopedSymbolTable currentScope;
        private ScopedAnalysisTable currentAnalysisScope;
        private AnalysisBranch currentAnalysisBranch;
        public List<string> Warnings { get; private set; }

        public AssignmentAnalyzer()
        {
            Warnings = new List<string>();
        }

        public List<string> Analyze(ProgramNode program, ScopedSymbolTable globalScope)
        {
            currentAnalysisScope = new ScopedAnalysisTable(null, globalScope);
            currentAnalysisBranch = new AnalysisBranch();
            Warnings = new List<string>();

            currentScope = globalScope;
            program.Accept(this);

            return Warnings;
        }

        public void Visit(ProgramNode node)
        {
            currentScope = currentScope.Lookup<ProgramSymbol>(node.Name).Scope;
            currentAnalysisScope = new ScopedAnalysisTable(currentAnalysisScope, currentScope);

            node.Block.Accept(this);

            currentScope = currentScope.EnclosingScope;
            currentAnalysisScope = currentAnalysisScope.EnclosingScope;
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
            node.Result.Accept(this);

            var symbol = node.Variable.Metadata.Reference as VarSymbol;
            currentAnalysisScope.Update(symbol, currentAnalysisBranch);
        }

        public void Visit(VariableNode variableNode)
        {
            var variable = variableNode.Metadata.Reference as VarSymbol;
            var symbolInfo = currentAnalysisScope[variable];
            if (!symbolInfo.HasValueInBranches.Contains(currentAnalysisBranch))
            {
                Warnings.Add($"Possible use of unassigned variable {variable}. Assigned to in {symbolInfo.HasValueInBranches.Count} branches");
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
            var procedure = node.Metadata.Reference as ProcedureSymbol;

            currentScope = procedure.Scope;
            currentAnalysisScope = new ScopedAnalysisTable(currentAnalysisScope, currentScope);

            var parameters = procedure.Parameters;

            foreach (var parameter in parameters)
            {
                currentAnalysisScope.Update(parameter, currentAnalysisBranch);
            }

            procedure.Reference.Block.Compound.Accept(this);

            currentScope = myScope;
            currentAnalysisScope = myAnalysisScope;
        }

        public void Visit(BooleanNode node)
        {
        }

        public void Visit(ComparisonOperationNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        public void Visit(IfNode node)
        {
            node.Condition.Accept(this);

            var myAnalysisBranch = currentAnalysisBranch;
            // Branch
            var thenBranch = new AnalysisBranch();
            currentAnalysisBranch = thenBranch;
            node.Then.Accept(this);
            currentAnalysisBranch = myAnalysisBranch;

            if (node.Else != null)
            {
                var elseBranch = new AnalysisBranch();
                currentAnalysisBranch = elseBranch;
                node.Else.Accept(this);
                currentAnalysisBranch = myAnalysisBranch;
                var assignedVariables = currentAnalysisScope.GetSymbolInfos()
                    .Where(x => !x.HasValueInBranches.Contains(myAnalysisBranch)
                                && x.HasValueInBranches.Contains(thenBranch)
                                && x.HasValueInBranches.Contains(elseBranch));
                foreach (var variable in assignedVariables)
                {
                    currentAnalysisScope.Update(variable.Symbol, currentAnalysisBranch);
                }
            }
        }

        public void Visit(FunctionCallNode node)
        {
            var myScope = currentScope;
            var myAnalysisScope = currentAnalysisScope;
            var function = node.Metadata.Reference as FunctionSymbol;

            currentScope = function.Scope;
            currentAnalysisScope = new ScopedAnalysisTable(currentAnalysisScope, currentScope);

            var parameters = function.Parameters;

            foreach (var parameter in parameters)
            {
                currentAnalysisScope.Update(parameter, currentAnalysisBranch);
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