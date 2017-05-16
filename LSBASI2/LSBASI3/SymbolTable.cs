using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class SymbolTableBuilder : IVisitor
    {
        private SymbolTable table;

        public SymbolTableBuilder()
        {
            this.table = new SymbolTable();
        }

        public SymbolTable Build(ProgramNode program)
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
    }

    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> table;

        public SymbolTable()
        {
            table = new Dictionary<string, Symbol>
            {
                {RealSymbol.Keyword, RealSymbol.Instance},
                {IntegerSymbol.Keyword, IntegerSymbol.Instance}
            };
        }

        public void Define(Symbol symbol)
        {
            table[symbol.Name] = symbol;
        }

        public T Lookup<T>(string name)
            where T : Symbol
        {
            Symbol symbol;
            table.TryGetValue(name, out symbol);
            return symbol as T;
        }

        public Symbol Lookup(string name)
        {
            Symbol symbol;
            table.TryGetValue(name, out symbol);
            return symbol;
        }
    }
}