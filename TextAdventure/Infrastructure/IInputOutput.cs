using System;

namespace TextAdventure.Infrastructure
{
    public interface IInputOutput
    {
        void Write(ConsoleColor colour, string text, bool newLine = true);

        string Input();

        void Clear();
    }
}