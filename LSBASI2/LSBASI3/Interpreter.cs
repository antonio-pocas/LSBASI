using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class Interpreter : IVisitor<int>
    {
        private Parser parser;
        private readonly StatementNode rootNode;
        private readonly Dictionary<string, int> SymbolTable;

        public Interpreter(Parser parser)
        {
            this.parser = parser;
            this.SymbolTable = new Dictionary<string, int>();
            this.rootNode = parser.Program();
        }

        public void Evaluate()
        {
            this.rootNode.Accept(this);
        }

        public int Visit(UnaryOperationNode node)
        {
            switch (node.Type)
            {
                case UnaryOperationType.Plus:
                    return +node.Child.Accept(this);

                case UnaryOperationType.Minus:
                    return -node.Child.Accept(this);
            }

            throw new Exception();
        }

        void IVisitor<int>.Visit(NoOpNode node)
        {
        }

        void IVisitor<int>.Visit(AssignmentNode assignmentNode)
        {
            SymbolTable[assignmentNode.Variable.Name] = assignmentNode.Result.Accept(this);
        }

        int IVisitor<int>.Visit(VariableNode node)
        {
            int value;
            if (!SymbolTable.TryGetValue(node.Name, out value))
            {
                throw new InvalidOperationException($"Unassigned variable {node.Name}");
            }
            return value;
        }

        void IVisitor<int>.Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        int IVisitor<int>.Visit(BinaryOperationNode node)
        {
            switch (node.Type)
            {
                case BinaryOperationType.Add:
                    return node.Left.Accept(this) + node.Right.Accept(this);

                case BinaryOperationType.Subtract:
                    return node.Left.Accept(this) - node.Right.Accept(this);

                case BinaryOperationType.Multiply:
                    return node.Left.Accept(this) * node.Right.Accept(this);

                case BinaryOperationType.Divide:
                    return node.Left.Accept(this) / node.Right.Accept(this);
            }

            throw new Exception();
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

        public int Visit(TypeNode node)
        {
        }

        int IVisitor<int>.Visit(NumberNode node)
        {
            return node.Value;
        }
    }
}