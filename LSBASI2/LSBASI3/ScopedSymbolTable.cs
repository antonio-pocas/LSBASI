using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class ScopedSymbolTable
    {
        private readonly Dictionary<string, Symbol> table;
        public string Name { get; private set; }
        public int Level { get; private set; }
        public ScopedSymbolTable EnclosingScope { get; private set; }

        private ScopedSymbolTable(string name, Dictionary<string, Symbol> table)
        {
            Level = 0;
            Name = name;
            this.table = table;
        }

        public ScopedSymbolTable(string name, int level, ScopedSymbolTable enclosingScope)
        {
            Level = level;
            EnclosingScope = enclosingScope;
            Name = name;
            table = new Dictionary<string, Symbol>();
        }

        public static ScopedSymbolTable CreateProgramScope(string programName)
        {
            var builtins = CreateBuiltinsDictionary();
            builtins.Add(programName, Symbol.Program(programName));

            return new ScopedSymbolTable(programName, builtins);
        }

        private static Dictionary<string, Symbol> CreateBuiltinsDictionary()
        {
            return new Dictionary<string, Symbol>
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
            if (!table.TryGetValue(name, out symbol) && EnclosingScope != null)
            {
                symbol = EnclosingScope.Lookup(name);
            }
            return symbol as T;
        }

        public Symbol Lookup(string name)
        {
            Symbol symbol;
            if (!table.TryGetValue(name, out symbol))
            {
                symbol = EnclosingScope.Lookup(name);
            }
            return symbol;
        }

        public T LocalLookup<T>(string name)
            where T : Symbol
        {
            Symbol symbol;
            table.TryGetValue(name, out symbol);
            return symbol as T;
        }

        public Symbol LocalLookup(string name)
        {
            Symbol symbol;
            table.TryGetValue(name, out symbol);
            return symbol;
        }
    }
}