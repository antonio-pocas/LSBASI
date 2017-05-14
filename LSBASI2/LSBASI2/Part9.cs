using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI2.Part9
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
                    return Token.Semicolon();
                }

                if (CurrentChar == ".")
                {
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
            while (CurrentChar == " ")
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
    /// Statement:                  CoumpoundStatement | AssignmentStatement | Empty
    /// AssignmentStatement:        Variable ASSIGN Expr
    /// Variable:                   ID
    /// Empty:
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
        private readonly HashSet<TokenType> factorOperationTokens = new HashSet<TokenType>() { TokenType.Add, TokenType.Subtract };

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

        public AstNode Program()
        {
            var program = CompoundStatement();
            Eat(TokenType.Dot);
            return program;
        }

        private AstNode CompoundStatement()
        {
            Eat(TokenType.Begin);
            var node = StatementList();
            Eat(TokenType.End);
            return node;
        }

        private AstNode StatementList()
        {
            throw new NotImplementedException();
        }

        private AstNode Expr()
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

        private AstNode Term()
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

        private AstNode Factor()
        {
            if (factorOperationTokens.Contains(currentToken.Type))
            {
                var token = currentToken;
                Eat(currentToken.Type);
                return new UnaryOperationNode(token, Factor());
            }
            if (currentToken.Type == TokenType.Integer)
            {
                var value = new NumberNode(currentToken);
                Eat(TokenType.Integer);
                return value;
            }

            Eat(TokenType.LeftParen);
            var ret = Expr();
            Eat(TokenType.RightParen);
            return ret;
        }
    }

    //public class Interpreter : IVisitor<int>
    //{
    //    private Parser parser;
    //    private readonly AstNode rootNode;

    //    public Interpreter(Parser parser)
    //    {
    //        this.parser = parser;
    //        this.rootNode = parser.Expr();
    //    }

    //    public int Evaluate()
    //    {
    //        return this.rootNode.Accept(this);
    //    }

    //    public int Visit(UnaryOperationNode node)
    //    {
    //        switch (node.Type)
    //        {
    //            case UnaryOperationType.Plus:
    //                return +node.Child.Accept(this);

    //            case UnaryOperationType.Minus:
    //                return -node.Child.Accept(this);
    //        }

    //        throw new Exception();
    //    }

    //    int IVisitor<int>.Visit(BinaryOperationNode node)
    //    {
    //        switch (node.Type)
    //        {
    //            case BinaryOperationType.Add:
    //                return node.Left.Accept(this) + node.Right.Accept(this);

    //            case BinaryOperationType.Subtract:
    //                return node.Left.Accept(this) - node.Right.Accept(this);

    //            case BinaryOperationType.Multiply:
    //                return node.Left.Accept(this) * node.Right.Accept(this);

    //            case BinaryOperationType.Divide:
    //                return node.Left.Accept(this) / node.Right.Accept(this);
    //        }

    //        throw new Exception();
    //    }

    //    int IVisitor<int>.Visit(NumberNode node)
    //    {
    //        return node.Value;
    //    }
    //}
}