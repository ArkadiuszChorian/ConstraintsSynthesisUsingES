using System;
using CSUES.Common;

namespace CSUES.WinApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var database = new DatabaseContext("Test.db");

            Console.WriteLine("FINISHED!");
            Console.ReadKey();
        }
    }
}
