using System;
using System.Collections.Generic;
using System.Reflection;

namespace LSBASI3
{
    public class ScopedSymbolTableBuilder : IVisitor
    {
        private ScopedSymbolTable currentScope;

        public ScopedSymbolTable Build(ProgramNode program)
        {
            program.Accept(this);

            return currentScope;
        }

        public void Visit(ProgramNode node)
        {
            currentScope = ScopedSymbolTable.CreateProgramScope(node.Name);
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
            var type = currentScope.Lookup<TypeSymbol>(node.Type.Value);
            if (type == null)
            {
                throw new TypeAccessException($"No type with name {node.Type.Value} declared");
            }
            var symbol = new VarSymbol(node.Variable.Name, type);
            this.currentScope.Define(symbol);
        }

        public void Visit(ProcedureNode node)
        {
            var name = node.Name;

            var symbol = currentScope.LocalLookup(name);
            if (symbol != null)
            {
                throw new Exception($"Symbol {symbol} already exists in the current scope");
            }

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

        public void Visit(FunctionNode node)
        {
            var name = node.Name;

            var symbol = currentScope.LocalLookup(name);
            if (symbol != null)
            {
                throw new Exception($"Symbol {symbol} already exists in the current scope");
            }

            var procedureScope = new ScopedSymbolTable(name, currentScope.Level + 1, currentScope);
            currentScope = procedureScope;
            var formalParameters = new List<VarSymbol>();

            foreach (var parameter in node.FormalParameters)
            {
                var parameterSymbol = new VarSymbol(parameter.Variable.Name, currentScope.Lookup<TypeSymbol>(parameter.Type.Value));
                formalParameters.Add(parameterSymbol);
                currentScope.Define(parameterSymbol);
            }

            var type = currentScope.Lookup<TypeSymbol>(node.Type.Value);

            var returnValue = new VarSymbol(name, type);
            currentScope.Define(returnValue);

            node.Block.Accept(this);

            currentScope = currentScope.EnclosingScope;
            currentScope.Define(new FunctionSymbol(name, formalParameters, procedureScope, node, type));
        }

        public void Visit(FunctionCallNode node)
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
        }

        public void Visit(ProcedureCallNode node)
        {
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
        }

        #region unused methods

        public void Visit(TypeNode typeNode)
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

        #endregion unused methods
    }
}