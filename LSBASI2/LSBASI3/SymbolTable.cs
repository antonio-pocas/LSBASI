using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> table;

        public SymbolTable()
        {
            table = new Dictionary<string, Symbol>
            {
                {RealSymbol.Keyword, BuiltinType.Real},
                {IntegerSymbol.Keyword, BuiltinType.Integer}
            };
        }

        public void Define(Symbol symbol)
        {
            table[symbol.Name] = symbol;
        }

        public bool Exists(string name)
        {
            return table.ContainsKey(name);
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