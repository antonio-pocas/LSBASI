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
            var x = @"program Main;
   var x, y: real;

   procedure Alpha(a : integer);
      var y : integer;
   begin
      x := a + x + y;
   end;

begin { Main }

end.  { Main }";

            var lexer = new Lexer(x);
            var parser = new Parser(lexer);
            var interpreter = new Interpreter(parser, new SemanticAnalyzer());
            interpreter.Interpret();

            //Console.WriteLine(y);
            Console.ReadLine();
        }
    }
}