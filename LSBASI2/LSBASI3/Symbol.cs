using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public abstract class Symbol
    {
        public string Name { get; set; }

        public Symbol(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProgramSymbol : Symbol
    {
        public ScopedSymbolTable Scope { get; set; }

        public ProgramSymbol(string name, ScopedSymbolTable scope) : base(name)
        {
            Scope = scope;
        }
    }

    public abstract class TypedSymbol : Symbol
    {
        public TypeSymbol Type { get; set; }

        public TypedSymbol(string name, TypeSymbol type) : base(name)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"{Name} : {Type}";
        }
    }

    public class VarSymbol : TypedSymbol
    {
        public VarSymbol(string name, TypeSymbol type) : base(name, type)
        {
        }
    }

    public class FunctionSymbol : TypedSymbol
    {
        public List<VarSymbol> Parameters { get; set; }
        public ScopedSymbolTable Scope { get; set; }
        public FunctionNode Reference { get; set; }

        public FunctionSymbol(string name, List<VarSymbol> parameters, ScopedSymbolTable scope, FunctionNode function, TypeSymbol type) : base(name, type)
        {
            Parameters = parameters;
            Scope = scope;
            Reference = function;
        }

        public override string ToString()
        {
            var parameters = Parameters.Any() ? $"({string.Join(", ", Parameters)})" : "()";
            return $"{Name}{parameters} : {Type}";
        }
    }

    public class ProcedureSymbol : Symbol
    {
        public List<VarSymbol> Parameters { get; set; }
        public ScopedSymbolTable Scope { get; set; }
        public ProcedureNode Reference { get; set; }

        public ProcedureSymbol(string name, List<VarSymbol> parameters, ScopedSymbolTable scope, ProcedureNode procedure) : base(name)
        {
            Parameters = parameters;
            Scope = scope;
            Reference = procedure;
        }

        public override string ToString()
        {
            var parameters = Parameters.Any() ? $"({string.Join(", ", Parameters)})" : string.Empty;
            return Name + parameters;
        }
    }
}