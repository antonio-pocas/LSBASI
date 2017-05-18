using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class CSharpTranslator : IVisitor, IEvaluator<string>
    {
        private ProgramNode rootNode;
        private StringBuilder stringBuilder;
        private int currentLevel;

        public CSharpTranslator(ProgramNode program)
        {
            rootNode = program;
            stringBuilder = new StringBuilder();
            currentLevel = 0;
        }

        private void AppendIndented(string code)
        {
            var prefix = string.Empty;
            for (var i = 0; i < currentLevel; i++)
            {
                prefix = prefix + "\t";
            }
            stringBuilder.Append(prefix);
            stringBuilder.AppendLine(code);
        }

        public string Translate()
        {
            stringBuilder.AppendLine($"namespace Translated");
            stringBuilder.Append("{");
            rootNode.Accept(this);
            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }

        public void Visit(ProgramNode node)
        {
            currentLevel++;

            AppendIndented($"public static class {node.Name}");
            AppendIndented("{");
            node.Block.Accept(this);
            AppendIndented("}");

            currentLevel--;
        }

        public void Visit(BlockNode node)
        {
            currentLevel++;
            foreach (var declaration in node.Declarations)
            {
                declaration.Accept(this);
            }

            if (currentLevel == 2)
            {
                AppendIndented("public static void Execute()");
                AppendIndented("{");
                currentLevel++;
            }

            node.Compound.Accept(this);

            if (currentLevel == 2)
            {
                currentLevel--;
                AppendIndented("}");
            }

            currentLevel--;
        }

        public void Visit(DeclarationNode node)
        {
            var type = node.Type.Yield(this);
            AppendIndented($"{type} {node.Variable.Name};");
        }

        public void Visit(ProcedureNode node)
        {
            AppendIndented($"public static class {node.Name}");
            AppendIndented("{");
            currentLevel++;

            var parameterBuilder = new StringBuilder();
            foreach (var parameter in node.FormalParameters)
            {
                parameterBuilder.Append($"{parameter.Type.Yield(this)} {parameter.Variable.Name},");
            }

            AppendIndented($"public static void Execute({parameterBuilder.ToString(0, parameterBuilder.Length - 1)})");
            AppendIndented("{");

            node.Block.Accept(this);

            AppendIndented("}");
            currentLevel--;
            AppendIndented("}");
        }

        public void Visit(ParameterNode node)
        {
            throw new NotImplementedException();
        }

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
            var value = node.Result.Yield(this);
            AppendIndented($"{name} = {value};");
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

        public string Evaluate(NumberNode node)
        {
            return node.Value;
        }

        public string Evaluate(VariableNode node)
        {
            return node.Name;
        }

        public string Evaluate(UnaryOperationNode node)
        {
            return node.token.Value + node.Child.Yield(this);
        }

        public string Evaluate(BinaryOperationNode node)
        {
            var operation = node.Type == BinaryOperationType.IntegerDivision ? "/" : node.token.Value;
            return $"{node.Left.Yield(this)} {operation} {node.Right.Yield(this)}";
        }

        public string Evaluate(TypeNode node)
        {
            if (node.Value.ToLowerInvariant() == "real")
            {
                return "double";
            }

            return "int";
        }

        public string Evaluate(NoOpNode noOpNode)
        {
            throw new NotImplementedException();
        }

        public string Evaluate(ProgramNode programNode)
        {
            throw new NotImplementedException();
        }

        public string Evaluate(BlockNode blockNode)
        {
            throw new NotImplementedException();
        }

        public string Evaluate(DeclarationNode declarationNode)
        {
            throw new NotImplementedException();
        }

        public string Evaluate(CompoundNode compoundNode)
        {
            throw new NotImplementedException();
        }

        public string Evaluate(AssignmentNode assignmentNode)
        {
            throw new NotImplementedException();
        }

        public string Evaluate(ProcedureNode node)
        {
            throw new NotImplementedException();
        }

        public string Evaluate(ParameterNode node)
        {
            throw new NotImplementedException();
        }
    }
}