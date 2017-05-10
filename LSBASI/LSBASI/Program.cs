using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = Console.ReadLine();
            var interpreter = new Interpreter(x);
            var y = interpreter.DoTheThings();
            Console.WriteLine(y);
            Console.ReadLine();
        }
    }

    class Interpreter
    {
        public Token CurrentToken { get; set; }
        public int Position { get; set; }
        public string Text { get; set; }

        public Interpreter(string input)
        {
            Text = input;
            Position = 0;
            CurrentToken = null;
        }

        public Token GetNextToken()
        {
            if(Position > Text.Length - 1)
            {
                return Token.CreateEOFToken();
            }

            while(Text[Position] == ' ')
            {
                Position++;
            }

            var currentChar = GetCurrentCharAsString();
            var sb = new StringBuilder();

            bool isInt = false;
            int tempValue;
            while(int.TryParse(currentChar, out tempValue))
            {
                isInt = true;
                sb.Append(currentChar);
                Position++;
                if (Position < Text.Length)
                {
                    currentChar = GetCurrentCharAsString();
                }
                else
                {
                    break;
                }
            }
            if(isInt)
            {
                return new Token(TokenType.Integer, sb.ToString());
            }

            if(currentChar == "+")
            {
                Position++;
                return new Token(TokenType.Plus, currentChar);
            }

            throw new InvalidOperationException("Error parsing input");
        }

        public void Eat(TokenType type)
        {
            if(CurrentToken.Type == type)
            {
                CurrentToken = GetNextToken();
            }
            else
            {
                throw new InvalidOperationException("Error parsing input");
            }
        }

        public int DoTheThings()
        {
            CurrentToken = GetNextToken();

            var left = CurrentToken;
            Eat(TokenType.Integer);

            var operation = CurrentToken;
            Eat(TokenType.Plus);

            var right = CurrentToken;
            Eat(TokenType.Integer);

            return Int32.Parse(left.Value) + Int32.Parse(right.Value);
        }

        private string GetCurrentCharAsString()
        {
            return Text[Position].ToString();
        }
    }

    class Token
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

    enum TokenType
    {
        Integer,
        Plus,
        Minus,
        EOF
    }
}
