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

    public class TypeSymbol : Symbol
    {
        public TypeSymbol(string name) : base(name)
        {
        }
    }

    public class BuiltinTypeSymbol : TypeSymbol
    {
        public BuiltinTypeSymbol(string name) : base(name)
        {
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
}