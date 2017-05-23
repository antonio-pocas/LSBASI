using System.Collections.Generic;
using System.Linq;

namespace LSBASI3
{
    public class ScopedAnalysisTable
    {
        public ScopedAnalysisTable EnclosingScope { get; set; }
        private readonly Dictionary<Symbol, SymbolInfo> symbolInformation;

        public SymbolInfo this[Symbol symbol]
        {
            get
            {
                SymbolInfo value;
                if (!symbolInformation.TryGetValue(symbol, out value) && EnclosingScope != null)
                {
                    value = EnclosingScope[symbol];
                }
                return value;
            }
        }

        public ScopedAnalysisTable(ScopedAnalysisTable enclosingScope, ScopedSymbolTable symbolScope)
        {
            EnclosingScope = enclosingScope;
            symbolInformation = new Dictionary<Symbol, SymbolInfo>();
            foreach (var symbol in symbolScope.GetLocalSymbols().OfType<VarSymbol>())
            {
                symbolInformation.Add(symbol, new SymbolInfo()
                {
                    Depth = symbolScope.Level,
                    HasValueInBranches = new HashSet<AnalysisBranch>(),
                    HasValue = false,
                    Type = SymbolType.Variable,
                    Scope = symbolScope,
                    Symbol = symbol
                });
            }
        }

        public void Update(Symbol symbol, AnalysisBranch branch)
        {
            var symbolInfo = this[symbol];
            symbolInfo.HasValueInBranches.Add(branch);
        }

        public IEnumerable<SymbolInfo> GetSymbolInfos()
        {
            var symbolInfos = symbolInformation.Values;
            if (EnclosingScope != null)
            {
                return symbolInfos.Concat(EnclosingScope.GetSymbolInfos());
            }

            return symbolInfos;
        }
    }

    public class SymbolInfo
    {
        public Symbol Symbol { get; set; }
        public SymbolType Type { get; set; }
        public bool HasValue { get; set; }
        public HashSet<AnalysisBranch> HasValueInBranches { get; set; }

        //public bool IsEvaluated { get; set; }
        public int Depth { get; set; }

        public ScopedSymbolTable Scope { get; set; }
    }

    public enum SymbolType
    {
        Procedure,
        Function,
        Variable,
        Type
    }
}