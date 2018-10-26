using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

                Do(command);
            }
        }

        private static void Output(string text)
        {
            Console.WriteLine(text);
        }

        private static string Input()
        {
            return Console.ReadLine();
        }

        private void Do(string command)
        {
            var parts = command.ToLower().Split(" ");

            switch (parts[0])
            {
                case "move":
                case "head":
                case "go":
                    Move(parts[1]);
                    break;
                default:
                    Output($"I don't know how to {command}");
                    break;
            }
        }

        private void Move(string direction)
        {
            switch (direction)
            {
                case "north":
                case "n":
                    _position.Y += 1;
                    break;
                case "south":
                case "s":
                    _position.Y -= 1;
                    break;
            }
        }
    }
}