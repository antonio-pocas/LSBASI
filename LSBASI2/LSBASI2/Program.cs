using System;
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

            var lexer = new Part8.Lexer(x);
            var parser = new Part8.Parser(lexer);
            var interpreter = new Part8.Interpreter(parser);
            var y = interpreter.Evaluate();

            Console.WriteLine(y);
            Console.ReadLine();
        }
    }
}