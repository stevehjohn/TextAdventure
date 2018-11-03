using System;
using System.Threading;

namespace TextAdventure.Infrastructure
{
    public class Output : IOutput
    {
        public void Write(ConsoleColor colour, string text)
        {
            Console.ForegroundColor = colour;
            Console.CursorVisible = false;
            foreach (var character in text)
            {
                Console.Write(character);
                Thread.Sleep(10);
            }
            Console.WriteLine();
            Console.CursorVisible = true;
        }
    }
}