using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI2.Part4
{
    internal class Lexer
    {
        private string currentChar => position < input.Length ? input[position].ToString() : null;
        private readonly string input;
        private int position;

        public Lexer(string input)
        {
            this.input = input;
        }

        public Token GetNextToken()
        {
            SkipWhitespace();

            if (currentChar != null)
            {
                int tempValue;
                if (int.TryParse(currentChar, out tempValue))
                {
                    return Integer();
                }

                if (currentChar == "/")
                {
                    position++;
                    return new Token(TokenType.Divide, string.Empty);
                }

                if (currentChar == "*")
                {
                    position++;
                    return new Token(TokenType.Multiply, string.Empty);
                }

                throw new InvalidOperationException("Lexer error");
            }

            return Token.CreateEOFToken();
        }

        private Token Integer()
        {
            var sb = new StringBuilder();
            int tempValue;
            do
            {
                sb.Append(currentChar);
                position++;
            } while (int.TryParse(currentChar, out tempValue));

            return new Token(TokenType.Integer, sb.ToString());
        }

        private void SkipWhitespace()
        {
            while (currentChar == " ")
            {
                position++;
            }
        }
    }

    /// <summary>
    /// Grammar:
    /// expr:      factor ((MUL|DIV) factor)*
    /// factor:    INTEGER
    /// </summary>
    internal class Parser
    {
        private Token currentToken;
        private readonly Lexer lexer;

        private readonly HashSet<TokenType> operationTokens = new HashSet<TokenType>() { TokenType.Multiply, TokenType.Divide };

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

        public int expr()
        {
            var result = factor();
            while (operationTokens.Contains(currentToken.Type))
            {
                if (currentToken.Type == TokenType.Multiply)
                {
                    Eat(TokenType.Multiply);
                    result = result * factor();
                }
                else if (currentToken.Type == TokenType.Divide)
                {
                    Eat(TokenType.Divide);
                    result = result / factor();
                }
            }

            return result; ;
        }

        private int factor()
        {
            var value = currentToken.Value;
            Eat(TokenType.Integer);
            return int.Parse(value);
        }
    }

    //internal class Interpreter
    //{
    //    private readonly Parser parser;

    //    public Interpreter(Parser parser)
    //    {
    //        this.parser = parser;
    //    }

    //    public int Interpret()
    //    {
    //        var expression = parser.GetExpression();

    //        int result = 0;
    //        Func<int, int, int> operation = (x, y) => y;
    //        Token current;
    //        while ((current = expression.Dequeue()).Type != TokenType.EOF)
    //        {
    //            switch (current.Type)
    //            {
    //                case TokenType.Add:
    //                    operation = (x, y) => x + y;
    //                    break;

    //                case TokenType.Subtract:
    //                    operation = (x, y) => x - y;
    //                    break;

    //                case TokenType.Integer:
    //                    result = operation(result, int.Parse(current.Value));
    //                    break;

    //                case TokenType.EOF:
    //                default:
    //                    break;
    //            }
    //        }

    //        return result;
    //    }
    //}
}