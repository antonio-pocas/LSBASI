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

    public abstract class TypeSymbol : Symbol
    {
        public TypeSymbol(string name) : base(name)
        {
        }

        public abstract TypedValue Cast(TypedValue from);

        public abstract bool CanCastTo(TypeSymbol type);

        protected void ValidateCastOrThrowException(TypeSymbol to)
        {
            if (!to.CanCastTo(this))
            {
                throw new InvalidCastException($"Cannot cast from {to} to {this}");
            }
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

    public abstract class BuiltinTypeSymbol : TypeSymbol
    {
        protected BuiltinTypeSymbol(string name) : base(name)
        {
        }
    }

    public abstract class NumberTypeSymbol : BuiltinTypeSymbol
    {
        protected NumberTypeSymbol(string name) : base(name)
        {
        }

        public abstract TypedValue Add(TypedValue left, TypedValue right);

        public abstract TypedValue Subtract(TypedValue left, TypedValue right);

        public abstract TypedValue Divide(TypedValue left, TypedValue right);

        public abstract TypedValue Multiply(TypedValue left, TypedValue right);

        public abstract TypedValue Plus(TypedValue value);

        public abstract TypedValue Minus(TypedValue value);
    }

    public class IntegerSymbol : NumberTypeSymbol
    {
        public const string Keyword = "INTEGER";
        public static IntegerSymbol Instance { get; }

        static IntegerSymbol()
        {
            Instance = new IntegerSymbol(Keyword);
        }

        protected IntegerSymbol(string name) : base(name)
        {
        }

        public override TypedValue Add(TypedValue left, TypedValue right)
        {
            return IntegerOperations.Add(left, right);
        }

        public override TypedValue Subtract(TypedValue left, TypedValue right)
        {
            return IntegerOperations.Subtract(left, right);
        }

        public override TypedValue Divide(TypedValue left, TypedValue right)
        {
            return IntegerOperations.Divide(left, right);
        }

        public override TypedValue Multiply(TypedValue left, TypedValue right)
        {
            return IntegerOperations.Multiply(left, right);
        }

        public override TypedValue Plus(TypedValue value)
        {
            return IntegerOperations.Plus(value);
        }

        public override TypedValue Minus(TypedValue value)
        {
            return IntegerOperations.Minus(value);
        }

        public override TypedValue Cast(TypedValue from)
        {
            ValidateCastOrThrowException(from.Type);

            return new TypedValue(Instance, Convert.ToInt32(from.Value));
        }

        public override bool CanCastTo(TypeSymbol type)
        {
            return type == BuiltinType.Real;
        }
    }

    public class RealSymbol : NumberTypeSymbol
    {
        public const string Keyword = "REAL";
        public static RealSymbol Instance { get; }

        static RealSymbol()
        {
            Instance = new RealSymbol(Keyword);
        }

        protected RealSymbol(string name) : base(name)
        {
        }

        public override TypedValue Add(TypedValue left, TypedValue right)
        {
            return RealOperations.Add(left, right);
        }

        public override TypedValue Subtract(TypedValue left, TypedValue right)
        {
            return RealOperations.Subtract(left, right);
        }

        public override TypedValue Divide(TypedValue left, TypedValue right)
        {
            return RealOperations.Divide(left, right);
        }

        public override TypedValue Multiply(TypedValue left, TypedValue right)
        {
            return RealOperations.Multiply(left, right);
        }

        public override TypedValue Plus(TypedValue value)
        {
            return RealOperations.Plus(value);
        }

        public override TypedValue Minus(TypedValue value)
        {
            return RealOperations.Minus(value);
        }

        public override TypedValue Cast(TypedValue from)
        {
            ValidateCastOrThrowException(from.Type);

            return new TypedValue(BuiltinType.Real, Convert.ToDouble(from.Value));
        }

        public override bool CanCastTo(TypeSymbol type)
        {
            return false;
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

    public static class BuiltinType
    {
        public static IntegerSymbol Integer => IntegerSymbol.Instance;
        public static RealSymbol Real => RealSymbol.Instance;
    }
}