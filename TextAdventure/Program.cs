using System;
using TextAdventure.Engine;
using TextAdventure.Infrastructure;
using TextAdventure.Map;

namespace TextAdventure
{
    internal static class Program
    {
        private static void Main()
        {
            var engine = new TextAdventureEngine(new Output(), MapCreator.CreateMap());

            engine.RunGame();

            Console.WriteLine("Bye!");

            Console.ReadLine();
        }
    }
}
