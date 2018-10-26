using System;
using System.Collections.Generic;
using System.Linq;
using TextAdventure.Models;

namespace TextAdventure.Engine
{
    public class TextAdventureEngine
    {
        private readonly List<Location> _map;

        private Coords _position;

        public TextAdventureEngine(List<Location> map)
        {
            _map = map;
        }

        public void RunGame()
        {
            _position = new Coords(0, 0);

            while (true)
            {
                var location = _map.First(l => l.Coords.X == _position.X && l.Coords.Y == _position.Y);

                Output(location.Description);

                var command = Input();
            }
        }

        private static void Output(string text)
        {
            Console.WriteLine(text);
        }

        public static string Input()
        {
            return Console.ReadLine();
        }
    }
}