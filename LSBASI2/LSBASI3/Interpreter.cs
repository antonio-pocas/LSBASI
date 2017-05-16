using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class Interpreter : IVisitor, IEvaluator<TypedValue>
    {
        private readonly AstNode rootNode;
        private readonly SymbolTable symbolTable;
        private readonly Dictionary<string, TypedValue> globalMemory;

        public Interpreter(Parser parser, SymbolTableBuilder symbolTableBuilder)
        {
            var node = parser.Parse();
            this.rootNode = node;
            this.symbolTable = symbolTableBuilder.Build(node);
            this.globalMemory = new Dictionary<string, TypedValue>();
        }

        public void Interpret()
        {
            this.rootNode.Accept(this);
        }

        //    public int Visit(UnaryOperationNode node)
        //    {
        //        switch (node.Type)
        //        {
        //            case UnaryOperationType.Plus:
        //                return +node.Child.Accept<int>(this);

        //            case UnaryOperationType.Minus:
        //                return -node.Child.Accept<int>(this);
        //        }

        //        throw new Exception();
        //    }

        //    void IVisitor.Visit(NoOpNode node)
        //    {
        //    }

        //    void IVisitor.Visit(AssignmentNode assignmentNode)
        //    {
        //        SymbolTable[assignmentNode.Variable.Name] = assignmentNode.Result.Accept<int>(this);
        //    }

        //    T IVisitor.Visit<T>(VariableNode node)
        //    {
        //        object value;
        //        if (!SymbolTable.TryGetValue(node.Name, out value))
        //        {
        //            throw new InvalidOperationException($"Unassigned variable {node.Name}");
        //        }
        //        return (T)value;
        //    }

        //    void IVisitor.Visit(CompoundNode node)
        //    {
        //        foreach (var child in node.Children)
        //        {
        //            child.Accept(this);
        //        }
        //    }

        //    T IVisitor.Visit<T>(BinaryOperationNode node)
        //    {
        //switch (node.Type)
        //{
        //    case BinaryOperationType.Add:
        //        return node.Left.Accept<T>(this) + node.Right.Accept<T>(this);

        //    case BinaryOperationType.Subtract:
        //        return node.Left.Accept<T>(this) - node.Right.Accept<T>(this);

        //    case BinaryOperationType.Multiply:
        //        return node.Left.Accept<T>(this) * node.Right.Accept<T>(this);

        //    case BinaryOperationType.Divide:
        //        return node.Left.Accept<T>(this) / node.Right.Accept<T>(this);
        //}

        //        throw new Exception();
        //    }

        //    public void Visit(ProgramNode node)
        //    {
        //        node.Block.Accept(this);
        //    }

        //    public void Visit(BlockNode node)
        //    {
        //        foreach (var declaration in node.Declarations)
        //        {
        //            declaration.Accept(this);
        //        }

        //        node.Compound.Accept(this);
        //    }

        //    public void Visit(DeclarationNode node)
        //    {
        //    }

        //    public int Visit(TypeNode node)
        //    {
        //    }

        //    int IVisitor.Visit(NumberNode node)
        //    {
        //        return node.Value;
        //    }
        //}
        public void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        public void Visit(NoOpNode node)
        {
        }

        public void Visit(AssignmentNode node)
        {
            var name = node.Variable.Name;
            var result = node.Result.Yield(this);
            var symbol = symbolTable.Lookup<TypedSymbol>(name);

            if (symbol.Type != result.Type)
            {
                if (!result.Type.CanCastTo(symbol.Type))
                {
                    throw new TypeAccessException(
                        $"Cannot assign value of type {result.Type} to variable of type {symbol.Type} ({name})");
                }
                result = symbol.Type.Cast(result);
            }

            globalMemory[node.Variable.Name] = result;
        }

        public void Visit(ProgramNode node)
        {
            node.Block.Accept(this);
        }

        public void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
            {
                declaration.Accept(this);
            }

            node.Compound.Accept(this);
        }

        public void Visit(DeclarationNode node)
        {
        }

        public void Visit(VariableNode variableNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(BinaryOperationNode binaryOperationNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(UnaryOperationNode unaryOperationNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(NumberNode numberNode)
        {
            throw new NotImplementedException();
        }

        public void Visit(TypeNode typeNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(NumberNode node)
        {
            if (node.token.Type == TokenType.IntegerConstant)
            {
                return new TypedValue(IntegerSymbol.Instance, int.Parse(node.Value));
            }

            if (node.token.Type == TokenType.RealConstant)
            {
                return new TypedValue(RealSymbol.Instance, double.Parse(node.Value, CultureInfo.InvariantCulture));
            }

            throw new Exception("Unsupported type for number node");
        }

        public TypedValue Evaluate(VariableNode node)
        {
            var name = node.Name;

            TypedValue value;
            if (!globalMemory.TryGetValue(name, out value))
            {
                throw new Exception($"Unassigned variable {name}");
            }

            return value;
        }

        public TypedValue Evaluate(UnaryOperationNode node)
        {
            var value = node.Child.Yield(this);
            var numberType = value.Type as NumberTypeSymbol;
            if (numberType == null)
            {
                throw new InvalidOperationException($"Unsupported type for operation {node.Type.ToString()}");
            }

            switch (node.Type)
            {
                case UnaryOperationType.Plus:
                    return numberType.Plus(value);

                case UnaryOperationType.Minus:
                    return numberType.Minus(value);
            }

            throw new Exception();
        }

        public TypedValue Evaluate(BinaryOperationNode node)
        {
            var leftValue = node.Left.Yield(this);
            var rightValue = node.Right.Yield(this);

            var numberType = leftValue.Type as NumberTypeSymbol;
            if (numberType == null)
            {
                throw new InvalidOperationException($"Unsupported type for operation {node.Type.ToString()}");
            }

            if (numberType != rightValue.Type && numberType.CanCastTo(rightValue.Type))
            {
                numberType = rightValue.Type as NumberTypeSymbol;
            }

            switch (node.Type)
            {
                case BinaryOperationType.Add:
                    return numberType.Add(leftValue, rightValue);

                case BinaryOperationType.Subtract:
                    return numberType.Subtract(leftValue, rightValue);

                case BinaryOperationType.Multiply:
                    return numberType.Multiply(leftValue, rightValue);

                case BinaryOperationType.IntegerDivision:
                    var integer = numberType as IntegerSymbol;
                    if (integer == null)
                    {
                        throw new InvalidOperationException($"Integer division may only be done on integers");
                    }
                    return integer.Divide(leftValue, rightValue);

                case BinaryOperationType.RealDivision:
                    return RealSymbol.Instance.Divide(leftValue, rightValue);
            }

            throw new Exception();
        }

        public TypedValue Evaluate(TypeNode node)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(NoOpNode noOpNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(ProgramNode programNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(BlockNode blockNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(DeclarationNode declarationNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(CompoundNode compoundNode)
        {
            throw new NotImplementedException();
        }

        public TypedValue Evaluate(AssignmentNode assignmentNode)
        {
            throw new NotImplementedException();
        }
    }
}