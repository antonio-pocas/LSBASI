using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI2.Part7Exercises
{
    public class RPNTranslator : IVisitor<string>
    {
        private readonly AstNode rootNode;

        public RPNTranslator(AstNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public string Evaluate()
        {
            return this.rootNode.Accept(this);
        }

        public string Visit(UnaryOperationNode binaryOperationNode)
        {
            throw new NotImplementedException();
        }

        string IVisitor<string>.Visit(BinaryOperationNode node)
        {
            return $"{node.Left.Accept(this)} {node.Right.Accept(this)} {OperationToString(node.Type)}";
        }

        private static string OperationToString(BinaryOperationType type)
        {
            switch (type)
            {
                case BinaryOperationType.Add:
                    return "+";

                case BinaryOperationType.Subtract:
                    return "-";

                case BinaryOperationType.Multiply:
                    return "*";

                case BinaryOperationType.Divide:
                    return "/";
            }
            throw new Exception();
        }

        string IVisitor<string>.Visit(NumberNode node)
        {
            return node.Value.ToString();
        }
    }

    public class SExpressionTranslator : IVisitor<string>
    {
        private readonly AstNode rootNode;

        public SExpressionTranslator(AstNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public string Evaluate()
        {
            return this.rootNode.Accept(this);
        }

        public string Visit(UnaryOperationNode binaryOperationNode)
        {
            throw new NotImplementedException();
        }

        string IVisitor<string>.Visit(BinaryOperationNode node)
        {
            return $"({OperationToString(node.Type)} {node.Left.Accept(this)} {node.Right.Accept(this)})";
        }

        private static string OperationToString(BinaryOperationType type)
        {
            switch (type)
            {
                case BinaryOperationType.Add:
                    return "+";

                case BinaryOperationType.Subtract:
                    return "-";

                case BinaryOperationType.Multiply:
                    return "*";

                case BinaryOperationType.Divide:
                    return "/";
            }
            throw new Exception();
        }

        string IVisitor<string>.Visit(NumberNode node)
        {
            return node.Value.ToString();
        }
    }
}