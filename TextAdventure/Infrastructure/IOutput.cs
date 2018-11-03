using System;

namespace TextAdventure.Infrastructure
{
    public interface IOutput
    {
        void Write(ConsoleColor colour, string text);
    }
}