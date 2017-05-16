using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var x = Console.ReadLine();
            var x = @"PROGRAM NameError2;
VAR
   b : INTEGER;

BEGIN
   b := 1;
   a := b + 2;
END.";

            var lexer = new Lexer(x);
            var parser = new Parser(lexer);
            //var interpreter = new Interpreter(parser);
            var symbolTableBuilder = new SymbolTableBuilder();
            var table = symbolTableBuilder.Build(parser.Parse());
            //interpreter.Evaluate();

            //Console.WriteLine(y);
            Console.ReadLine();
        }
    }
}