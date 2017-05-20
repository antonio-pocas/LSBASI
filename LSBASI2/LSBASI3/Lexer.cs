using System;
using System.Collections.Generic;
using System.Text;

namespace LSBASI3
{
    public class Lexer
    {
        private string CurrentChar => Position < Input.Length ? Input[Position].ToString() : null;
        private readonly string Input;
        private int Position;

        private static readonly Dictionary<string, Token> ReservedKeywords;

        static Lexer()
        {
            ReservedKeywords = new Dictionary<string, Token>();
            AddKeyword(Token.Program());
            AddKeyword(Token.Procedure());
            AddKeyword(Token.Var());
            AddKeyword(Token.Integer());
            AddKeyword(Token.Real());
            AddKeyword(Token.Begin());
            AddKeyword(Token.End());
            AddKeyword(Token.IntegerDivision());
            AddKeyword(Token.Function());
        }

        private static void AddKeyword(Token keyword)
        {
            ReservedKeywords.Add(keyword.Value.ToLowerInvariant(), keyword);
        }

        public Lexer(string input)
        {
            this.Input = input;
        }

        public Token GetNextToken()
        {
            while (CurrentChar != null)
            {
                SkipWhitespace();

                if (CurrentChar == "{")
                {
                    Position++;
                    SkipComments();
                    continue;
                }

                if (CurrentChar == "_" || char.IsLetter(Input[Position]))
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

                if (CurrentChar == ":")
                {
                    Position++;
                    return Token.Colon();
                }

                if (CurrentChar == ",")
                {
                    Position++;
                    return Token.Comma();
                }

                if (CurrentChar == ".")
                {
                    Position++;
                    return Token.Dot();
                }

                int tempValue;
                if (int.TryParse(CurrentChar, out tempValue))
                {
                    return Number();
                }

                if (CurrentChar == "*")
                {
                    Position++;
                    return Token.Multiply();
                }

                if (CurrentChar == "/")
                {
                    Position++;
                    return Token.Slash();
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

        private Token Number()
        {
            var sb = new StringBuilder();
            int tempValue;
            do
            {
                sb.Append(CurrentChar);
                Position++;
            } while (int.TryParse(CurrentChar, out tempValue));

            if (CurrentChar != ".")
            {
                return Token.IntegerConstant(sb.ToString());
            }

            do
            {
                sb.Append(CurrentChar);
                Position++;
            } while (int.TryParse(CurrentChar, out tempValue));

            return Token.RealConstant(sb.ToString());
        }

        private Token IdOrReserved()
        {
            var builder = new StringBuilder();
            while (CurrentChar != null && (CurrentChar == "_" || char.IsLetterOrDigit(Input[Position])))
            {
                builder.Append(Input[Position++]);
            }

            var word = builder.ToString();
            Token token;
            ReservedKeywords.TryGetValue(word.ToLowerInvariant(), out token);

            return token ?? Token.Id(word);
        }

        private void SkipWhitespace()
        {
            while (Position < Input.Length && char.IsWhiteSpace(Input[Position]))
            {
                Position++;
            }
        }

        private void SkipComments()
        {
            while (CurrentChar != null && Input[Position] != '}')
            {
                Position++;
            }
            Position++;
        }

        private string Peek() => Position + 1 < Input.Length ? Input[Position + 1].ToString() : null;
    }
}