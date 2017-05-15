using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3.Part9
{
    public class Lexer
    {
        private string CurrentChar => Position < Input.Length ? Input[Position].ToString() : null;
        private readonly string Input;
        private int Position;

        private readonly Dictionary<string, Token> ReservedKeywords = new Dictionary<string, Token>()
        {
            { "BEGIN", new Token(TokenType.Begin, "BEGIN") },
            { "END", new Token(TokenType.End, "END") },
        };

        public Lexer(string input)
        {
            this.Input = input;
        }

        public Token GetNextToken()
        {
            SkipWhitespace();

            if (CurrentChar != null)
            {
                if (char.IsLetter(Input[Position]))
                {
                    return IdOrReserved();
                }

                if (CurrentChar == ":" && Peek() == "=")
                {
                    Position = Position + 2;
                    return Token.Assign();
                }

                if (CurrentChar == ";")
                {
                    Position++;
                    return Token.Semicolon();
                }

                if (CurrentChar == ".")
                {
                    Position++;
                    return Token.Dot();
                }

                int tempValue;
                if (int.TryParse(CurrentChar, out tempValue))
                {
                    return Integer();
                }

                if (CurrentChar == "/")
                {
                    Position++;
                    return Token.Divide();
                }

                if (CurrentChar == "*")
                {
                    Position++;
                    return Token.Multiply();
                }

                if (CurrentChar == "+")
                {
                    Position++;
                    return Token.Add();
                }

                if (CurrentChar == "-")
                {
                    Position++;
                    return Token.Subtract();
                }

                if (CurrentChar == "(")
                {
                    Position++;
                    return Token.LeftParen();
                }

                if (CurrentChar == ")")
                {
                    Position++;
                    return Token.RightParen();
                }

                throw new InvalidOperationException("Lexer error");
            }

            return Token.EOF();
        }

        private Token Integer()
        {
            var sb = new StringBuilder();
            int tempValue;
            do
            {
                sb.Append(CurrentChar);
                Position++;
            } while (int.TryParse(CurrentChar, out tempValue));

            return new Token(TokenType.Integer, sb.ToString());
        }

        private void SkipWhitespace()
        {
            while (Position < Input.Length && char.IsWhiteSpace(Input[Position]))
            {
                Position++;
            }
        }

        private Token IdOrReserved()
        {
            var builder = new StringBuilder();
            while (CurrentChar != null && char.IsLetterOrDigit(Input[Position]))
            {
                builder.Append(Input[Position++]);
            }

            var word = builder.ToString();
            Token token;
            ReservedKeywords.TryGetValue(word, out token);

            return token ?? Token.Id(word);
        }

        private string Peek() => Position + 1 < Input.Length ? Input[Position + 1].ToString() : null;
    }

    /// <summary>
    /// Grammar:
    /// Program:                    CompoundStatement DOT
    /// CompoundStatement:          BEGIN StatementList END
    /// StatementList:              Statement | Statement SEMI StatementList
    /// Statement:                  CoumpoundStatement | AssignmentStatement | EmptyRule
    /// AssignmentStatement:        Variable ASSIGN Expr
    /// Variable:                   ID
    /// EmptyRule:
    /// Expr:                       Term ((ADD|SUB) Term)*
    /// Term:                       Factor ((MUL|DIV) Factor)*
    /// Factor:                     + Factor | -Factor | INTEGER | LPAREN Expr RPAREN | Variable
    /// </summary>
    public class Parser
    {
        private Token currentToken;
        private readonly Lexer lexer;

        private readonly HashSet<TokenType> termOperationTokens = new HashSet<TokenType>() { TokenType.Multiply, TokenType.Divide };
        private readonly HashSet<TokenType> exprOperationTokens = new HashSet<TokenType>() { TokenType.Add, TokenType.Subtract };

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            currentToken = lexer.GetNextToken();
        }

        private void Eat(TokenType type)
        {
            if (currentToken.Type == type)
            {
                currentToken = lexer.GetNextToken();
            }
            else
            {
                throw new InvalidOperationException("Parser error");
            }
        }

        public StatementNode Parse()
        {
            var node = Program();
            if (currentToken.Type != TokenType.EOF)
            {
                throw new InvalidOperationException("Last token is not EOF");
            }
            return node;
        }

        public StatementNode Program()
        {
            var program = CompoundStatement();
            Eat(TokenType.Dot);
            return program;
        }

        private StatementNode CompoundStatement()
        {
            Eat(TokenType.Begin);
            var compound = new CompoundNode(StatementList());
            Eat(TokenType.End);
            return compound;
        }

        private List<StatementNode> StatementList()
        {
            var nodes = new List<StatementNode>() { Statement() };
            while (currentToken.Type == TokenType.Semicolon)
            {
                Eat(TokenType.Semicolon);
                nodes.Add(Statement());
            }

            if (currentToken.Type == TokenType.Id)
            {
                throw new InvalidOperationException("variable in wrong place");
            }

            return nodes;
        }

        private StatementNode Statement()
        {
            if (currentToken.Type == TokenType.Begin)
            {
                return CompoundStatement();
            }

            if (currentToken.Type == TokenType.Id)
            {
                return AssignmentStatement();
            }

            return EmptyRule();
        }

        private AssignmentNode AssignmentStatement()
        {
            var variable = Variable();
            var token = currentToken;
            Eat(TokenType.Assign);
            var result = Expr();
            return new AssignmentNode(token, variable, result);
        }

        private StatementNode EmptyRule()
        {
            return new NoOpNode();
        }

        private ExpressionNode Expr()
        {
            var result = Term();
            while (exprOperationTokens.Contains(currentToken.Type))
            {
                if (currentToken.Type == TokenType.Add)
                {
                    var token = currentToken;
                    Eat(TokenType.Add);
                    result = new BinaryOperationNode(token, result, Term());
                }
                else if (currentToken.Type == TokenType.Subtract)
                {
                    var token = currentToken;
                    Eat(TokenType.Subtract);
                    result = new BinaryOperationNode(token, result, Term());
                }
            }

            return result;
        }

        private ExpressionNode Term()
        {
            var result = Factor();
            while (termOperationTokens.Contains(currentToken.Type))
            {
                if (currentToken.Type == TokenType.Multiply)
                {
                    var token = currentToken;
                    Eat(TokenType.Multiply);
                    result = new BinaryOperationNode(token, result, Factor());
                }
                else if (currentToken.Type == TokenType.Divide)
                {
                    var token = currentToken;
                    Eat(TokenType.Divide);
                    result = new BinaryOperationNode(token, result, Factor());
                }
            }

            return result;
        }

        private ExpressionNode Factor()
        {
            if (currentToken.Type == TokenType.Add)
            {
                var token = currentToken;
                Eat(TokenType.Add);
                return new UnaryOperationNode(token, Factor());
            }

            if (currentToken.Type == TokenType.Subtract)
            {
                var token = currentToken;
                Eat(TokenType.Subtract);
                return new UnaryOperationNode(token, Factor());
            }

            if (currentToken.Type == TokenType.Integer)
            {
                var value = new NumberNode(currentToken);
                Eat(TokenType.Integer);
                return value;
            }

            if (currentToken.Type == TokenType.LeftParen)
            {
                Eat(TokenType.LeftParen);
                var ret = Expr();
                Eat(TokenType.RightParen);
                return ret;
            }

            return Variable();
        }

        private VariableNode Variable()
        {
            var token = currentToken;
            Eat(TokenType.Id);
            return new VariableNode(token);
        }
    }

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

        int IVisitor<int>.Visit(NumberNode node)
        {
            return node.Value;
        }
    }
}