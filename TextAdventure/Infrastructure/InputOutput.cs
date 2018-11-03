using System;
using System.Threading;

namespace TextAdventure.Infrastructure
{
    public class InputOutput : IInputOutput
    {
        public void Write(ConsoleColor colour, string text, bool newLine = true)
        {
            Console.ForegroundColor = colour;
            Console.CursorVisible = false;

            foreach (var character in text)
            {
                Console.Write(character);
                Thread.Sleep(10);
            }

            if (newLine)
            {
                Console.WriteLine();
            }

            Console.CursorVisible = true;
        }

        public string Input()
        {
            return Console.ReadLine();
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}