using System;
using System.Collections.Generic;
using System.Text;

namespace AlbionTradeHelper
{
    class Menu
    {
        public bool StartMenu()
        {
            Console.WriteLine("Welcome Albion Trade Helper!");

            while (true)
            {
                Console.WriteLine("Do You want percentage order (y/n)?: ");
                var answer = Console.ReadKey();
                if (answer.Key != ConsoleKey.Y && answer.Key != ConsoleKey.N)
                {
                    Console.WriteLine("Please response with 'y' or 'n'!");
                }
                else if (answer.Key == ConsoleKey.Y)
                {
                    return true;
                }
                else if (answer.Key == ConsoleKey.N)
                {
                    return false;
                }
            }
        }
    }
}
