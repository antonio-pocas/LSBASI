using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI2.Part3
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

                if (currentChar == "+")
                {
                    position++;
                    return new Token(TokenType.Add, string.Empty);
                }

                if (currentChar == "-")
                {
                    position++;
                    return new Token(TokenType.Subtract, string.Empty);
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

    internal class Parser
    {
        private readonly Lexer lexer;

        private Token currentToken;
        private Token previousToken;

        private readonly HashSet<TokenType> validIntegerAntecedents = new HashSet<TokenType>() { TokenType.Add, TokenType.Subtract };

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            currentToken = lexer.GetNextToken();
        }

        private void Eat(TokenType type)
        {
            if (currentToken.Type == type)
            {
                previousToken = currentToken;
                currentToken = lexer.GetNextToken();
            }
            else
            {
                throw new InvalidOperationException("Parser error");
            }
        }

        public Queue<Token> GetExpression()
        {
            var expression = new Queue<Token>();

            while (currentToken.Type != TokenType.EOF)
            {
                if (currentToken.Type == TokenType.Integer
                    && (previousToken == null || validIntegerAntecedents.Contains(previousToken.Type)))
                {
                    expression.Enqueue(currentToken);
                    Eat(TokenType.Integer);
                    continue;
                }

                if (currentToken.Type == TokenType.Add
                    && (previousToken != null && previousToken.Type == TokenType.Integer))
                {
                    expression.Enqueue(currentToken);
                    Eat(TokenType.Add);
                    continue;
                }

                if (currentToken.Type == TokenType.Subtract
                    && (previousToken != null && previousToken.Type == TokenType.Integer))
                {
                    expression.Enqueue(currentToken);
                    Eat(TokenType.Subtract);
                    continue;
                }

                throw new InvalidOperationException("Parser error");
            }
            expression.Enqueue(currentToken);
            return expression;
        }
    }

    internal class Interpreter
    {
        private readonly Parser parser;

        public Interpreter(Parser parser)
        {
            this.parser = parser;
        }

        public int Interpret()
        {
            var expression = parser.GetExpression();

            int result = 0;
            Func<int, int, int> operation = (x, y) => y;
            Token current;
            while ((current = expression.Dequeue()).Type != TokenType.EOF)
            {
                switch (current.Type)
                {
                    case TokenType.Add:
                        operation = (x, y) => x + y;
                        break;

                    case TokenType.Subtract:
                        operation = (x, y) => x - y;
                        break;

                    case TokenType.Integer:
                        result = operation(result, int.Parse(current.Value));
                        break;

                    case TokenType.EOF:
                    default:
                        break;
                }
            }

            return result;
        }
    }
}