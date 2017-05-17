using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class TypedValue
    {
        public TypeSymbol Type { get; set; }
        public object Value { get; set; }

        public TypedValue(TypeSymbol type, object value)
        {
            Type = type;
            Value = value;
        }

        public TypedValue BinaryOperation(TypedValue other, Func<TypedValue, TypedValue, TypedValue> operation)
        {
            if (this.Type != other.Type)
            {
                throw new TypeAccessException($"Cannot perform operation between values of different types: {this.Type} and {other.Type}");
            }
            return operation(this, other);
        }

        public TypedValue UnaryOperation(Func<TypedValue, TypedValue> operation)
        {
            return operation(this);
        }

        public override string ToString()
        {
            return $"{Value} : {Type}";
        }
    }

    public static class IntegerOperations
    {
        public static TypedValue IntegerValue(object value)
        {
            return new TypedValue(IntegerSymbol.Instance, value);
        }

        public static TypedValue Add(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return IntegerValue(leftValue + rightValue);
        }

        public static TypedValue Subtract(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return IntegerValue(leftValue - rightValue);
        }

        public static TypedValue Divide(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return IntegerValue(leftValue / rightValue);
        }

        public static TypedValue Multiply(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return IntegerValue(leftValue * rightValue);
        }

        public static TypedValue Plus(TypedValue value)
        {
            var actual = Convert.ToInt32(value.Value);

            return IntegerValue(+actual);
        }

        public static TypedValue Minus(TypedValue value)
        {
            var actual = Convert.ToInt32(value.Value);

            return IntegerValue(-actual);
        }
    }

    public static class RealOperations
    {
        public static TypedValue RealValue(object value)
        {
            return new TypedValue(RealSymbol.Instance, value);
        }

        public static TypedValue Add(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return RealValue(leftValue + rightValue);
        }

        public static TypedValue Subtract(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return RealValue(leftValue - rightValue);
        }

        public static TypedValue Divide(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return RealValue(leftValue / rightValue);
        }

        public static TypedValue Multiply(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return RealValue(leftValue * rightValue);
        }

        public static TypedValue Plus(TypedValue value)
        {
            var actual = Convert.ToDouble(value.Value);

            return RealValue(+actual);
        }

        public static TypedValue Minus(TypedValue value)
        {
            var actual = Convert.ToDouble(value.Value);

            return RealValue(-actual);
        }
    }
}