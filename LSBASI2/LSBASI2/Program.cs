﻿using System;
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

            var lexer = new Part6Exercise.Lexer(x);
            var parser = new Part6Exercise.Parser(lexer);
            //var interpreter = new Part3.Interpreter(parser);
            var y = parser.expr();

            Console.WriteLine(y);
            Console.ReadLine();
        }
    }

    public class Token
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

    public enum TokenType
    {
        Integer,
        Add,
        Subtract,
        Divide,
        Multiply,
        LeftParen,
        RightParen,
        EOF
    }
}