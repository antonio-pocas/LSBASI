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
            node.Variable.Accept<bool>(this);
            node.Result.Accept<bool>(this);
        }

        public T Visit<T>(NumberNode node)
        {
            return default(T);
        }

        public T Visit<T>(VariableNode node)
        {
            return default(T);
        }

        public T Visit<T>(UnaryOperationNode node)
        {
            return node.Child.Accept<T>(this);
        }

        public T Visit<T>(BinaryOperationNode node)
        {
            node.Left.Accept<bool>(this);
            node.Right.Accept<bool>(this);
            return default(T);
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
        }

        public void Visit(DeclarationNode node)
        {
            var symbol = new VarSymbol(node.Variable.Name, table.Lookup<BuiltinTypeSymbol>(node.Type.Value));
            this.table.Define(symbol);
        }

        public T Visit<T>(TypeNode node)
        {
            return default(T);
        }
    }

    public class SymbolTable
    {
        private Dictionary<string, Symbol> table;

        public SymbolTable()
        {
            table = new Dictionary<string, Symbol>
            {
                {"REAL", new BuiltinTypeSymbol("REAL")},
                {"INTEGER", new BuiltinTypeSymbol("INTEGER")}
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
    }
}