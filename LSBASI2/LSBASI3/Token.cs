namespace LSBASI3
{
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

        public static Token EOF()
        {
            return new Token(TokenType.EOF, string.Empty);
        }

        public static Token Program()
        {
            return new Token(TokenType.Program, "PROGRAM");
        }

        public static Token Procedure()
        {
            return new Token(TokenType.Procedure, "PROCEDURE");
        }

        public static Token Function()
        {
            return new Token(TokenType.Function, "FUNCTION");
        }

        public static Token Var()
        {
            return new Token(TokenType.Var, "VAR");
        }

        public static Token Colon()
        {
            return new Token(TokenType.Colon, ":");
        }

        public static Token Comma()
        {
            return new Token(TokenType.Comma, ",");
        }

        public static Token Assign()
        {
            return new Token(TokenType.Assign, ":=");
        }

        public static Token Add()
        {
            return new Token(TokenType.Plus, "+");
        }

        public static Token Subtract()
        {
            return new Token(TokenType.Minus, "-");
        }

        public static Token IntegerDivision()
        {
            return new Token(TokenType.IntegerDivision, "div");
        }

        public static Token Slash()
        {
            return new Token(TokenType.Slash, "/");
        }

        public static Token Multiply()
        {
            return new Token(TokenType.Star, "*");
        }

        public static Token LeftParen()
        {
            return new Token(TokenType.LeftParen, "(");
        }

        public static Token RightParen()
        {
            return new Token(TokenType.RightParen, ")");
        }

        public static Token Begin()
        {
            return new Token(TokenType.Begin, "BEGIN");
        }

        public static Token End()
        {
            return new Token(TokenType.End, "END");
        }

        public static Token Semicolon()
        {
            return new Token(TokenType.Semicolon, ";");
        }

        public static Token Dot()
        {
            return new Token(TokenType.Dot, ".");
        }

        public static Token Id(string value)
        {
            return new Token(TokenType.Id, value);
        }

        public static Token IntegerConstant(string value)
        {
            return new Token(TokenType.IntegerConstant, value);
        }

        public static Token RealConstant(string value)
        {
            return new Token(TokenType.RealConstant, value);
        }

        public static Token Integer()
        {
            return new Token(TokenType.Integer, "INTEGER");
        }

        public static Token Real()
        {
            return new Token(TokenType.Real, "REAL");
        }
    }

    public enum TokenType
    {
        Program,
        Procedure,
        Function,
        Var,
        Colon,
        Comma,
        Integer,
        Real,
        IntegerConstant,
        RealConstant,
        Plus,
        Minus,
        Slash,
        IntegerDivision,
        Star,
        LeftParen,
        RightParen,
        Begin,
        End,
        Semicolon,
        Dot,
        Assign,
        Id,
        EOF
    }
}