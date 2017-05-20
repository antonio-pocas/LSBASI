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
            var programScope = new ScopedSymbolTable(programName, builtins);
            var globalScope = new ScopedSymbolTable("Global", 1, programScope);

            var program = new ProgramSymbol(programName, globalScope);
            builtins.Add(programName, program);

            return globalScope;
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
            if (!table.TryGetValue(name, out symbol) && EnclosingScope != null)
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

        public SymbolInformation<T> LookupInfo<T>(string name)
            where T : Symbol
        {
            Symbol symbol;
            if (table.TryGetValue(name, out symbol))
            {
                var actualSymbol = symbol as T;
                if (actualSymbol == null)
                {
                    throw new Exception($"Symbol type error for symbol {symbol.Name}, expected {typeof(T).Name}, got {symbol.GetType()}");
                }
                return new SymbolInformation<T>(actualSymbol, Level);
            }

            if (EnclosingScope != null)
            {
                return EnclosingScope.LookupInfo<T>(name);
            }

            throw new Exception($"Symbol {name} not present in current scope");
        }
    }

    public sealed class SymbolInformation<T>
        where T : Symbol
    {
        public T Symbol { get; }
        public int ScopeLevel { get; }

        public SymbolInformation(T symbol, int scopeLevel)
        {
            Symbol = symbol;
            ScopeLevel = scopeLevel;
        }
    }
}