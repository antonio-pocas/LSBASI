using System.Collections.Generic;

namespace LSBASI3
{
    public class ScopedAnalysisTable
    {
        public ScopedAnalysisTable EnclosingScope { get; set; }
        private readonly Dictionary<string, SymbolInfo> symbolInformation;

        public SymbolInfo this[string name]
        {
            get
            {
                SymbolInfo value;
                if (!symbolInformation.TryGetValue(name, out value) && EnclosingScope != null)
                {
                    value = EnclosingScope[name];
                }
                return value;
            }

            set { symbolInformation[name] = value; }
        }

        public ScopedAnalysisTable(ScopedAnalysisTable enclosingScope)
        {
            EnclosingScope = enclosingScope;
            symbolInformation = new Dictionary<string, SymbolInfo>();
        }

        public bool Exists(string name)
        {
            return symbolInformation.ContainsKey(name);
        }
    }

    public class SymbolInfo
    {
        public Symbol Symbol { get; set; }
        public SymbolType Type { get; set; }
        public bool HasValue { get; set; }

        //public bool IsEvaluated { get; set; }
        public int Depth { get; set; }

        public ScopedSymbolTable Scope { get; set; }
    }

    public enum SymbolType
    {
        Procedure,
        Variable,
        Type
    }
}