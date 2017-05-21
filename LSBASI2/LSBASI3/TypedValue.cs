using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public override string ToString()
        {
            return $"{Value} : {Type}";
        }
    }

    public static class IntegerOperations
    {
        public static TypedValue IntegerValue(object value)
        {
            return new TypedValue(BuiltinType.Integer, value);
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

        public static TypedValue Equals(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return BooleanOperations.BooleanValue(leftValue == rightValue);
        }

        public static TypedValue Differs(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return BooleanOperations.BooleanValue(leftValue != rightValue);
        }

        public static TypedValue GreaterThan(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return BooleanOperations.BooleanValue(leftValue > rightValue);
        }

        public static TypedValue GreaterThanOrEqual(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return BooleanOperations.BooleanValue(leftValue >= rightValue);
        }

        public static TypedValue LessThan(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return BooleanOperations.BooleanValue(leftValue < rightValue);
        }

        public static TypedValue LessThanOrEqual(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToInt32(left.Value);
            var rightValue = Convert.ToInt32(right.Value);

            return BooleanOperations.BooleanValue(leftValue <= rightValue);
        }
    }

    public static class RealOperations
    {
        public static TypedValue RealValue(object value)
        {
            return new TypedValue(BuiltinType.Real, value);
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

        public static TypedValue Equals(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return BooleanOperations.BooleanValue(leftValue == rightValue);
        }

        public static TypedValue Differs(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return BooleanOperations.BooleanValue(leftValue != rightValue);
        }

        public static TypedValue GreaterThan(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return BooleanOperations.BooleanValue(leftValue > rightValue);
        }

        public static TypedValue GreaterThanOrEqual(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return BooleanOperations.BooleanValue(leftValue >= rightValue);
        }

        public static TypedValue LessThan(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return BooleanOperations.BooleanValue(leftValue < rightValue);
        }

        public static TypedValue LessThanOrEqual(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToDouble(left.Value);
            var rightValue = Convert.ToDouble(right.Value);

            return BooleanOperations.BooleanValue(leftValue <= rightValue);
        }
    }

    public static class BooleanOperations
    {
        public static TypedValue BooleanValue(object value)
        {
            return new TypedValue(BuiltinType.Boolean, value);
        }

        public static TypedValue Equals(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToBoolean(left.Value);
            var rightValue = Convert.ToBoolean(right.Value);

            return BooleanOperations.BooleanValue(leftValue == rightValue);
        }

        public static TypedValue Differs(TypedValue left, TypedValue right)
        {
            var leftValue = Convert.ToBoolean(left.Value);
            var rightValue = Convert.ToBoolean(right.Value);

            return BooleanOperations.BooleanValue(leftValue != rightValue);
        }
    }
}