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
    }

    public static class TypeChecker
    {
        public static bool AreCompatible(TypeSymbol expected, TypeSymbol actual)
        {
            if (expected == actual || actual.CanCastTo(expected))
            {
                return true;
            }

            return false;
        }

        public static NumberTypeSymbol CheckBinaryOperation(BinaryOperationType operation, TypeSymbol left, TypeSymbol right)
        {
            var leftAsNumber = left as NumberTypeSymbol;
            var rightAsNumber = right as NumberTypeSymbol;

            if (leftAsNumber == null || rightAsNumber == null)
            {
                throw new TypeAccessException($"Cannot perform operation {operation.ToString()} between types {left} and {right}");
            }

            if (operation == BinaryOperationType.IntegerDivision
                && (leftAsNumber == BuiltinType.Real || rightAsNumber == BuiltinType.Real))
            {
                throw new TypeAccessException($"Cannot perform operation {BinaryOperationType.IntegerDivision.ToString()} between types {left} and {right}");
            }

            if (operation == BinaryOperationType.RealDivision)
            {
                return BuiltinType.Real;
            }

            if (leftAsNumber == rightAsNumber || rightAsNumber.CanCastTo(leftAsNumber))
            {
                return leftAsNumber;
            }

            if (leftAsNumber.CanCastTo(rightAsNumber))
            {
                return rightAsNumber;
            }

            throw new TypeAccessException($"Cannot perform operation {operation.ToString()} between types {left} and {right}");
        }

        public static NumberTypeSymbol CheckUnaryOperation(TypeSymbol type)
        {
            var numberType = type as NumberTypeSymbol;

            if (numberType == null)
            {
                throw new TypeAccessException($"Cannot perform a unary operation on the type {type}");
            }

            return numberType;
        }
    }
}