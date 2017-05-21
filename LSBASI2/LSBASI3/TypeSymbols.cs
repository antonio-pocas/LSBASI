using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
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

        public abstract TypedValue Equals(TypedValue left, TypedValue right);

        public abstract TypedValue Differs(TypedValue left, TypedValue right);
    }

    public abstract class BuiltinTypeSymbol : TypeSymbol
    {
        protected BuiltinTypeSymbol(string name) : base(name)
        {
        }
    }

    public class BooleanSymbol : BuiltinTypeSymbol
    {
        public const string Keyword = "BOOLEAN";
        public static BooleanSymbol Instance { get; }

        static BooleanSymbol()
        {
            Instance = new BooleanSymbol(Keyword);
        }

        protected BooleanSymbol(string name) : base(name)
        {
        }

        public override TypedValue Cast(TypedValue from)
        {
            throw new InvalidCastException($"Cannot cast from {from} to {this}");
        }

        public override bool CanCastTo(TypeSymbol type)
        {
            return false;
        }

        public override TypedValue Equals(TypedValue left, TypedValue right)
        {
            return BooleanOperations.Equals(left, right);
        }

        public override TypedValue Differs(TypedValue left, TypedValue right)
        {
            return BooleanOperations.Differs(left, right);
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

        public abstract TypedValue GreaterThan(TypedValue left, TypedValue right);

        public abstract TypedValue GreaterThanOrEqual(TypedValue left, TypedValue right);

        public abstract TypedValue LessThan(TypedValue left, TypedValue right);

        public abstract TypedValue LessThanOrEqual(TypedValue left, TypedValue right);
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

        public override TypedValue Equals(TypedValue left, TypedValue right)
        {
            return IntegerOperations.Equals(left, right);
        }

        public override TypedValue Differs(TypedValue left, TypedValue right)
        {
            return IntegerOperations.Differs(left, right);
        }

        public override TypedValue GreaterThan(TypedValue left, TypedValue right)
        {
            return IntegerOperations.GreaterThan(left, right);
        }

        public override TypedValue GreaterThanOrEqual(TypedValue left, TypedValue right)
        {
            return IntegerOperations.GreaterThanOrEqual(left, right);
        }

        public override TypedValue LessThan(TypedValue left, TypedValue right)
        {
            return IntegerOperations.LessThan(left, right);
        }

        public override TypedValue LessThanOrEqual(TypedValue left, TypedValue right)
        {
            return IntegerOperations.LessThanOrEqual(left, right);
        }

        public override bool CanCastTo(TypeSymbol type)
        {
            return type == BuiltinType.Real;
        }

        public override TypedValue Cast(TypedValue from)
        {
            ValidateCastOrThrowException(from.Type);

            return new TypedValue(Instance, Convert.ToInt32(from.Value));
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

        public override TypedValue Equals(TypedValue left, TypedValue right)
        {
            return RealOperations.Equals(left, right);
        }

        public override TypedValue Differs(TypedValue left, TypedValue right)
        {
            return RealOperations.Differs(left, right);
        }

        public override TypedValue GreaterThan(TypedValue left, TypedValue right)
        {
            return RealOperations.GreaterThan(left, right);
        }

        public override TypedValue GreaterThanOrEqual(TypedValue left, TypedValue right)
        {
            return RealOperations.GreaterThanOrEqual(left, right);
        }

        public override TypedValue LessThan(TypedValue left, TypedValue right)
        {
            return RealOperations.LessThan(left, right);
        }

        public override TypedValue LessThanOrEqual(TypedValue left, TypedValue right)
        {
            return RealOperations.LessThanOrEqual(left, right);
        }

        public override TypedValue Cast(TypedValue from)
        {
            ValidateCastOrThrowException(from.Type);

            return new TypedValue(Instance, Convert.ToDouble(from.Value));
        }

        public override bool CanCastTo(TypeSymbol type)
        {
            return false;
        }
    }

    public static class BuiltinType
    {
        public static IntegerSymbol Integer => IntegerSymbol.Instance;
        public static RealSymbol Real => RealSymbol.Instance;
        public static BooleanSymbol Boolean => BooleanSymbol.Instance;
    }
}