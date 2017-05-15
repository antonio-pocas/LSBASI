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
            var x = @"BEGIN
    BEGIN
        number := 2;
        a := number;
        b := 10 * a + 10 * number div 4;
        _c := a - - b;
    END;
    x := 11;
END.";

            var lexer = new Part9.Lexer(x);
            var parser = new Part9.Parser(lexer);
            var interpreter = new Part9.Interpreter(parser);
            interpreter.Evaluate();

            //Console.WriteLine(y);
            Console.ReadLine();
        }
    }
}