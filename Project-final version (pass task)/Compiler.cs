using System;
using System.IO;

namespace CompilerSimpleCSharp
{
    static class Compiler
    {
        public static string fName = @"C:\Users\ТЕОДОРА\Desktop\SimpleCSharpCompilerfinal\SimpleCSharpCompiler\CompilerSimpleCSharp\bin\Debug\ups.txt";
        internal static bool Comp()
        {
            TextReader reader = new StreamReader(fName);
            Scanner scanner = new Scanner(reader);
            Table symbolTable = new Table();
            Emit emiter = new Emit(Path.GetFileNameWithoutExtension(fName) + ".exe", symbolTable);
            Parser parser = new Parser(scanner, symbolTable, emiter);

            emiter.InitProgram();
            bool isProgram = parser.Parse();

            if (isProgram)
            {
                Console.WriteLine("The program is SUCCESSFUL compile!!!! ");
                Console.WriteLine(symbolTable);
                emiter.WriteExecutable();
                return true;
            }
            else
            {
                Console.WriteLine("Compilation error!!");
                return false;
            }
        }
    }
}