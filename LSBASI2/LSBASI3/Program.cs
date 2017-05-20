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
   var x, y, tempum, tempdois : real;
   var aaa, bbb : integer;

   procedure Alpha(double : real; int: integer);
      var y : integer;
      var z : real;
   begin
      z := double + 2;
      y := int + 2;
      tempum := z;
      tempdois := y;
   end;

begin { Main }
      bbb := 234;
    y := 2 + 40;
    Alpha(123.456, 2);
end.  { Main }";

            var lexer = new Lexer(x);
            var parser = new Parser(lexer);
            var node = new Parser(new Lexer(x)).Parse();
            var translator = new CSharpTranslator(node);
            var text = translator.Translate();
            var interpreter = new Interpreter(parser, new ScopedSymbolTableBuilder(), new SemanticAnalyzer());
            interpreter.Interpret();

            //Console.WriteLine(y);
            Console.ReadLine();
        }
    }
}