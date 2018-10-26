using System;
using TextAdventure.Engine;
using TextAdventure.Map;

namespace TextAdventure
{
    internal static class Program
    {
        private static void Main()
        {
            var engine = new TextAdventureEngine(MapCreator.CreateMap());

            engine.RunGame();

            Console.ReadLine();
        }
    }
}
