using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ComputeZobristKey
{
    class Program
    {
        static void Main(string[] args)
        {
            string userCommand = "";
            Zobrist zobrist = new Zobrist();
            string ouput = "";
            while (userCommand != "exit")
            {
                userCommand = Console.ReadLine();
                int count = Regex.Matches(userCommand, @"/").Count;
                if (count != 9)
                {
                    Console.WriteLine("input error");
                    continue;
                }
                if (userCommand.IndexOf("k") == -1 && userCommand.IndexOf("K") == -1)
                {
                    Console.WriteLine("input error");
                    continue;
                }
                if (!(userCommand.IndexOf("b - - 0 1") != -1 || userCommand.IndexOf("w - - 0 1") != -1))
                {
                    Console.WriteLine("input error");
                    continue;
                }
                ouput = zobrist.getKey(userCommand).ToString("X");
                Console.WriteLine(ouput);
            }
            Console.ReadKey(); 
       }
    }
}
