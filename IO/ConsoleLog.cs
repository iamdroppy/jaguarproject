using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JaguarProject.IO
{
    class ConsoleLog
    {
        public static void Log(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[" + DateTime.Now.ToLongTimeString() + "] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text);
        }
        public static void LogLine(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[" + DateTime.Now.ToLongTimeString() + "] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }
        public static void LogError(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[" + DateTime.Now.ToLongTimeString() + "] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void LogErrorLine(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[" + DateTime.Now.ToLongTimeString() + "] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
