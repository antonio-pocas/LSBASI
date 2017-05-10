using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var x = Console.ReadLine();
            var interpreter = new Interpreter(x);
            var y = interpreter.Evaluate();
            Console.WriteLine(y);
            Console.ReadLine();
        }
    }

    internal class Interpreter
    {
        public Token CurrentToken { get; set; }
        public int Position { get; set; }
        public string Text { get; set; }

        public string CurrentChar => Position < Text.Length
            ? Text[Position].ToString()
            : null;

        public Interpreter(string input)
        {
            Text = input;
            Position = 0;
            CurrentToken = null;
        }

        public Token GetNextToken()
        {
            if (Position > Text.Length - 1)
            {
                return Token.CreateEOFToken();
            }

            while (CurrentChar == " ")
            {
                Position++;
            }

            if (CurrentChar == null)
            {
                return Token.CreateEOFToken();
            }

            int tempValue;
            if (int.TryParse(CurrentChar, out tempValue))
            {
                return new Token(TokenType.Integer, TakeInteger());
            }

            if (CurrentChar == "+")
            {
                Position++;
                return new Token(TokenType.Add, CurrentChar);
            }

            if (CurrentChar == "-")
            {
                Position++;
                return new Token(TokenType.Subtract, CurrentChar);
            }

            if (CurrentChar == "*")
            {
                Position++;
                return new Token(TokenType.Multiply, CurrentChar);
            }

            if (CurrentChar == "/")
            {
                Position++;
                return new Token(TokenType.Divide, CurrentChar);
            }

            throw new InvalidOperationException("Error parsing input");
        }

        private string TakeInteger()
        {
            var sb = new StringBuilder();
            int tempValue;
            while (int.TryParse(CurrentChar, out tempValue))
            {
                sb.Append(CurrentChar);
                Position++;
            }

            return sb.ToString();
        }

        public void Eat(TokenType type)
        {
            if (CurrentToken.Type == type)
            {
                CurrentToken = GetNextToken();
            }
            else
            {
                throw new InvalidOperationException("Error parsing input");
            }
        }

        public int Evaluate()
        {
            CurrentToken = GetNextToken();
            int tempResult = 0;
            bool isFirst = true;
            while (CurrentToken.Type != TokenType.EOF)
            {
                Token left;
                if (isFirst)
                {
                    left = CurrentToken;
                    Eat(TokenType.Integer);
                    isFirst = false;
                }
                else
                {
                    left = new Token(TokenType.Integer, tempResult.ToString());
                }

                var operation = CurrentToken;
                Func<int, int, int> operationFunction;
                switch (operation.Type)
                {
                    case TokenType.Add:
                        Eat(TokenType.Add);
                        operationFunction = (x, y) => x + y;
                        break;

                    case TokenType.Subtract:
                        Eat(TokenType.Subtract);
                        operationFunction = (x, y) => x - y;
                        break;

                    case TokenType.Multiply:
                        Eat(TokenType.Multiply);
                        operationFunction = (x, y) => x * y;
                        break;

                    case TokenType.Divide:
                        Eat(TokenType.Divide);
                        operationFunction = (x, y) => x / y;
                        break;

                    default:
                        throw new InvalidOperationException("Error parsing input");
                }

                var right = CurrentToken;
                Eat(TokenType.Integer);
                tempResult = operationFunction(Int32.Parse(left.Value), Int32.Parse(right.Value));
            }

            return tempResult;
        }

        private string GetCurrentCharAsString()
        {
            return Text[Position].ToString();
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
        Multiply,
        Divide,
        EOF
    }
}