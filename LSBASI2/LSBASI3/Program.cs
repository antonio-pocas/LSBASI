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
            var x = @"PROGRAM Part10;
VAR
   number     : INTEGER;
   a, b, c, x : INTEGER;
   y          : REAL;

BEGIN {Part10}
   BEGIN
      number := 2;
      a := number;
      b := 10 * a + 10 * number DIV 4;
      c := a - - b
   END;
   x := 11;
   y := 20 / 7 + 3.14;
   { writeln('a = ', a); }
   { writeln('b = ', b); }
   { writeln('c = ', c); }
   { writeln('number = ', number); }
   { writeln('x = ', x); }
   { writeln('y = ', y); }
END.  {Part10}";

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