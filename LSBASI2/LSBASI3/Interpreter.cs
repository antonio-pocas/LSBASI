//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LSBASI3
//{
//    public class Interpreter : IVisitor
//    {
//        private Parser parser;
//        private readonly StatementNode rootNode;
//        private readonly Dictionary<string, object> SymbolTable;

//        public Interpreter(Parser parser)
//        {
//            this.parser = parser;
//            this.SymbolTable = new Dictionary<string, object>();
//            this.rootNode = parser.Program();
//        }

//        public void Evaluate()
//        {
//            this.rootNode.Accept(this);
//        }

//        public int Visit(UnaryOperationNode node)
//        {
//            switch (node.Type)
//            {
//                case UnaryOperationType.Plus:
//                    return +node.Child.Accept<int>(this);

//                case UnaryOperationType.Minus:
//                    return -node.Child.Accept<int>(this);
//            }

//            throw new Exception();
//        }

//        void IVisitor.Visit(NoOpNode node)
//        {
//        }

//        void IVisitor.Visit(AssignmentNode assignmentNode)
//        {
//            SymbolTable[assignmentNode.Variable.Name] = assignmentNode.Result.Accept<int>(this);
//        }

//        T IVisitor.Visit<T>(VariableNode node)
//        {
//            object value;
//            if (!SymbolTable.TryGetValue(node.Name, out value))
//            {
//                throw new InvalidOperationException($"Unassigned variable {node.Name}");
//            }
//            return (T)value;
//        }

//        void IVisitor.Visit(CompoundNode node)
//        {
//            foreach (var child in node.Children)
//            {
//                child.Accept(this);
//            }
//        }

//        T IVisitor.Visit<T>(BinaryOperationNode node)
//        {
//            switch (node.Type)
//            {
//                case BinaryOperationType.Add:
//                    return node.Left.Accept<T>(this) + node.Right.Accept<T>(this);

//                case BinaryOperationType.Subtract:
//                    return node.Left.Accept<T>(this) - node.Right.Accept<T>(this);

//                case BinaryOperationType.Multiply:
//                    return node.Left.Accept<T>(this) * node.Right.Accept<T>(this);

//                case BinaryOperationType.Divide:
//                    return node.Left.Accept<T>(this) / node.Right.Accept<T>(this);
//            }

//            throw new Exception();
//        }

//        public void Visit(ProgramNode node)
//        {
//            node.Block.Accept(this);
//        }

//        public void Visit(BlockNode node)
//        {
//            foreach (var declaration in node.Declarations)
//            {
//                declaration.Accept(this);
//            }

//            node.Compound.Accept(this);
//        }

//        public void Visit(DeclarationNode node)
//        {
//        }

//        public int Visit(TypeNode node)
//        {
//        }

//        int IVisitor.Visit(NumberNode node)
//        {
//            return node.Value;
//        }
//    }
//}