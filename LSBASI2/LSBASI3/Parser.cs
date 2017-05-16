using System;
using System.Collections.Generic;
using System.Linq;

namespace LSBASI3
{
    /// <summary>
    /// Grammar:
    /// Program:                    PROGRAM Variable SEMI Block DOT
    /// Block:                      Declarations CompoundStatement
    /// Declarations:               VAR (VariableDeclaration SEMI)+ | EmptyRule
    /// VariableDeclaration:        ID (COMMA ID)* COLON TypeSpecification
    /// TypeSpecification:          INTEGER | REAL
    /// CompoundStatement:          BEGIN StatementList END
    /// StatementList:              Statement | Statement SEMI StatementList
    /// Statement:                  CoumpoundStatement | AssignmentStatement | EmptyRule
    /// AssignmentStatement:        Variable ASSIGN Expr
    /// EmptyRule:
    /// Expr:                       Term ((PLUS|MINUS) Term)*
    /// Term:                       Factor ((MUL|REAL_DIV|INTEGER_DIV) Factor)*
    /// Factor:                     PLUS Factor
    ///                           | MINUS Factor
    ///                           | INTEGER_CONST
    ///                           | REAL_CONST
    ///                           | LPAREN Expr RPAREN
    ///                           | Variable
    /// Variable:                   ID
    /// </summary>
    public class Parser
    {
        private Token CurrentToken;
        private readonly Lexer lexer;

        private readonly HashSet<TokenType> termOperationTokens = new HashSet<TokenType>() { TokenType.Star, TokenType.IntegerDivision, TokenType.Slash };
        private readonly HashSet<TokenType> exprOperationTokens = new HashSet<TokenType>() { TokenType.Plus, TokenType.Minus };

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            CurrentToken = lexer.GetNextToken();
        }

        private void Eat(TokenType type)
        {
            if (CurrentToken.Type == type)
            {
                CurrentToken = lexer.GetNextToken();
            }
            else
            {
                throw new InvalidOperationException("Parser error");
            }
        }

        public ProgramNode Parse()
        {
            var node = Program();
            if (CurrentToken.Type != TokenType.EOF)
            {
                throw new InvalidOperationException("Last token is not EOF");
            }
            return node;
        }

        private ProgramNode Program()
        {
            Eat(TokenType.Program);
            var name = Variable().Name;
            Eat(TokenType.Semicolon);
            var block = Block();
            var program = new ProgramNode(name, block);
            Eat(TokenType.Dot);
            return program;
        }

        public BlockNode Block()
        {
            var declarations = Declarations();
            var program = CompoundStatement();

            return new BlockNode(declarations, program);
        }

        private List<AstNode> Declarations()
        {
            if (CurrentToken.Type != TokenType.Var)
            {
                return new List<AstNode>() { EmptyRule() };
            }

            Eat(TokenType.Var);
            var declarations = new List<AstNode>();
            while (CurrentToken.Type == TokenType.Id)
            {
                var declaration = VariableDeclaration();
                declarations.AddRange(declaration);
                Eat(TokenType.Semicolon);
            }

            return declarations;
        }

        private List<DeclarationNode> VariableDeclaration()
        {
            var variables = new List<VariableNode>() { new VariableNode(CurrentToken) };
            Eat(TokenType.Id);

            while (CurrentToken.Type == TokenType.Comma)
            {
                Eat(TokenType.Comma);
                variables.Add(new VariableNode(CurrentToken));
                Eat(TokenType.Id);
            }

            Eat(TokenType.Colon);
            var type = TypeSpecification();
            return variables.Select(x => new DeclarationNode(x, type)).ToList();
        }

        private TypeNode TypeSpecification()
        {
            var token = CurrentToken;
            if (CurrentToken.Type == TokenType.Integer)
            {
                Eat(TokenType.Integer);
                return new TypeNode(token);
            }

            Eat(TokenType.Real);
            return new TypeNode(token);
        }

        private CompoundNode CompoundStatement()
        {
            Eat(TokenType.Begin);
            var compound = new CompoundNode(StatementList());
            Eat(TokenType.End);
            return compound;
        }

        private List<AstNode> StatementList()
        {
            var nodes = new List<AstNode>() { Statement() };
            while (CurrentToken.Type == TokenType.Semicolon)
            {
                Eat(TokenType.Semicolon);
                nodes.Add(Statement());
            }

            if (CurrentToken.Type == TokenType.Id)
            {
                throw new InvalidOperationException("variable in wrong place");
            }

            return nodes;
        }

        private AstNode Statement()
        {
            if (CurrentToken.Type == TokenType.Begin)
            {
                return CompoundStatement();
            }

            if (CurrentToken.Type == TokenType.Id)
            {
                return AssignmentStatement();
            }

            return EmptyRule();
        }

        private AssignmentNode AssignmentStatement()
        {
            var variable = Variable();
            var token = CurrentToken;
            Eat(TokenType.Assign);
            var result = Expr();
            return new AssignmentNode(token, variable, result);
        }

        private AstNode EmptyRule()
        {
            return new NoOpNode();
        }

        private AstNode Expr()
        {
            var result = Term();
            while (exprOperationTokens.Contains(CurrentToken.Type))
            {
                if (CurrentToken.Type == TokenType.Plus)
                {
                    var token = CurrentToken;
                    Eat(TokenType.Plus);
                    result = new BinaryOperationNode(token, result, Term());
                }
                else if (CurrentToken.Type == TokenType.Minus)
                {
                    var token = CurrentToken;
                    Eat(TokenType.Minus);
                    result = new BinaryOperationNode(token, result, Term());
                }
            }

            return result;
        }

        private AstNode Term()
        {
            var result = Factor();
            while (termOperationTokens.Contains(CurrentToken.Type))
            {
                var token = CurrentToken;
                if (CurrentToken.Type == TokenType.Star)
                {
                    Eat(TokenType.Star);
                    result = new BinaryOperationNode(token, result, Factor());
                }
                else if (CurrentToken.Type == TokenType.IntegerDivision)
                {
                    Eat(TokenType.IntegerDivision);
                    result = new BinaryOperationNode(token, result, Factor());
                }
                else if (CurrentToken.Type == TokenType.Slash)
                {
                    Eat(TokenType.Slash);
                    result = new BinaryOperationNode(token, result, Factor());
                }
            }

            return result;
        }

        private AstNode Factor()
        {
            if (CurrentToken.Type == TokenType.Plus)
            {
                var token = CurrentToken;
                Eat(TokenType.Plus);
                return new UnaryOperationNode(token, Factor());
            }

            if (CurrentToken.Type == TokenType.Minus)
            {
                var token = CurrentToken;
                Eat(TokenType.Minus);
                return new UnaryOperationNode(token, Factor());
            }

            if (CurrentToken.Type == TokenType.IntegerConstant)
            {
                var value = new NumberNode(CurrentToken);
                Eat(TokenType.IntegerConstant);
                return value;
            }

            if (CurrentToken.Type == TokenType.RealConstant)
            {
                var value = new NumberNode(CurrentToken);
                Eat(TokenType.RealConstant);
                return value;
            }

            if (CurrentToken.Type == TokenType.LeftParen)
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
            var token = CurrentToken;
            Eat(TokenType.Id);
            return new VariableNode(token);
        }
    }
}