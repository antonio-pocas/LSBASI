﻿using System;
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
   var x, y, tempum, tempdois, newtestvar : real;
   var aaa, bbb, testeste : integer;
   var abcdef, bool : boolean;

   procedure Alpha(Alpha : real; int: integer);
      var y : integer;
      var z : real;
   begin
      z := Alpha + 2;
      y := int + 2;
      {tempum := tempdois + z; }
      tempdois := y;
      {tempum := bbb;}
   end;

function one: integer;
    begin
        one := 1;
    end;

    function plustwo(numba : integer): integer;
        var
           result: integer;
        begin
           result := numba + 2;
           plustwo := result;
        end;
begin { Main }
      {bbb := 234;}
    y := 2 + 40 + plustwo(5) + one;
    Alpha(123.456, 2);
abcdef := 4 * 40 + 2 / 3 > 5;

if (4 > 3) then
  begin
    abcdef := false;
  end;
else
  begin
    abcdef := true;
    newtestvar := 6.7;
    if (5 > 3) then
        bbb := 1;
    else
        newtestvar := newtestvar - bbb;
  end;
bbb := 1;

newtestvar := newtestvar + 1;
end.  { Main }";

            var lexer = new Lexer(x);
            var parser = new Parser(lexer);
            //var node = new Parser(new Lexer(x)).Parse();
            //var translator = new CSharpTranslator(node);
            //var text = translator.Translate();
            var interpreter = new Interpreter(parser, new ScopedSymbolTableBuilder(), new SemanticAnalyzer(), new AssignmentAnalyzer());
            interpreter.Interpret();

            //Console.WriteLine(y);
            Console.ReadLine();
        }
    }
}