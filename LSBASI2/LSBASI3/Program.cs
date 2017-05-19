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
      y := a + 2;
      x :=  x + y; { ERROR here! }
   end;

begin { Main }
    y := 2 + 40;
    Alpha(3);
end.  { Main }";

            var lexer = new Lexer(x);
            var parser = new Parser(lexer);
            var node = new Parser(new Lexer(x)).Parse();
            var translator = new CSharpTranslator(node);
            var text = translator.Translate();
            var interpreter = new Interpreter(parser, new SemanticAnalyzer());
            interpreter.Interpret();

            //Console.WriteLine(y);
            Console.ReadLine();
        }
    }
}