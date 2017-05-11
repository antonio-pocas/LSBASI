using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var x = Console.ReadLine();

            var lexer = new Lexer(x);
            var parser = new Parser(lexer);
            var interpreter = new Interpreter(parser);
            var y = interpreter.Interpret();

            Console.WriteLine(y);
            Console.ReadLine();
        }
    }

    internal class Lexer
    {
        private string currentChar => position < input.Length ? input[position].ToString() : null;
        private string input;
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

    internal class Parser
    {
        private Lexer lexer;

        private Token currentToken;
        private Token previousToken;

        private HashSet<TokenType> validIntegerAntecedents = new HashSet<TokenType>() { TokenType.Add, TokenType.Subtract };

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
                if (currentToken.Type == TokenType.Integer)
                {
                    if (previousToken == null || validIntegerAntecedents.Contains(previousToken.Type))
                    {
                        expression.Enqueue(currentToken);
                        Eat(TokenType.Integer);
                    }
                }
                else if (currentToken.Type == TokenType.Add)
                {
                    if (previousToken != null && previousToken.Type == TokenType.Integer)
                    {
                        expression.Enqueue(currentToken);
                        Eat(TokenType.Add);
                    }
                }
                else if (currentToken.Type == TokenType.Subtract)
                {
                    if (previousToken != null && previousToken.Type == TokenType.Integer)
                    {
                        expression.Enqueue(currentToken);
                        Eat(TokenType.Subtract);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Parser error");
                }
            }
            expression.Enqueue(currentToken);
            return expression;
        }
    }

    internal class Interpreter
    {
        private Parser parser;

        public Interpreter(Parser parser)
        {
            this.parser = parser;
        }

        public int Interpret()
        {
            var expression = parser.GetExpression();
            var current = expression.Dequeue();

            if (current.Type == TokenType.EOF)
            {
                return 0;
            }

            var result = Int32.Parse(current.Value);
            Func<int, int, int> operation = (x, y) => x;
            while (current.Type != TokenType.EOF)
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
                current = expression.Dequeue();
            }

            return result;
        }
    }

    internal class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType tokenType, string value)
        {
            Type = tokenType;
            Value = value;
        }

        public override string ToString()
        {
            return $"Token({Type.ToString()}, {Value})";
        }

        public static Token CreateEOFToken()
        {
            return new Token(TokenType.EOF, string.Empty);
        }
    }

    internal enum TokenType
    {
        Integer,
        Add,
        Subtract,
        EOF
    }
}