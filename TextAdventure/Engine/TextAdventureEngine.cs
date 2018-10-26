using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TextAdventure.Models;

namespace TextAdventure.Engine
{
    public class TextAdventureEngine
    {
        private readonly List<Location> _map;
        private readonly List<Item> _items;

        private Coords _position;
        private Location _location;

        public TextAdventureEngine(List<Location> map)
        {
            _map = map;
            _items = new List<Item>();
        }

        public void RunGame()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Output("Simple text adventure engine by Stevö John.\n");

            _position = new Coords(0, 0);

            while (true)
            {
                _location = _map.First(l => l.Coords.X == _position.X && l.Coords.Y == _position.Y);

                Console.ForegroundColor = ConsoleColor.Blue;
                Output(_location.Description);

                if (_location.Event != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Output(_location.Event.Invoke(_items, _map));

                    if (_location.OneOffEvent)
                    {
                        _location.Event = null;
                    }
                }

                if (_location.Items.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Output("\nItems in the area:");

                    Console.ForegroundColor = ConsoleColor.Green;
                    foreach (var item in _location.Items)
                    {
                        Output(item.Description);
                    }
                }

                if (_items.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Output("\nYou are carrying:");

                    Console.ForegroundColor = ConsoleColor.Green;
                    foreach (var item in _items)
                    {
                        Output(item.Description);
                    }
                }

                string command;

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;

                do
                {
                    Console.Write("> ");
                    command = Input();
                } while (string.IsNullOrWhiteSpace(command));

                Console.WriteLine();

                if (Do(command))
                {
                    break;
                }
            }
        }

        private static void Output(string text)
        {
            Console.CursorVisible = false;
            foreach (var character in text)
            {
                Console.Write(character);
                Thread.Sleep(10);
            }
            Console.WriteLine();
            Console.CursorVisible = true;
        }

        private static string Input()
        {
            return Console.ReadLine();
        }

        private bool Do(string command)
        {
            var parts = command.ToLower().Split(" ");

            switch (parts[0])
            {
                case "move":
                case "head":
                case "go":
                    if (parts.Length > 1)
                    {
                        Move(parts[1]);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Output("Go where exactly?\n");
                    }
                    return false;
                case "take":
                case "pickup":
                    if (parts.Length > 1)
                    {
                        Take(parts[1]);
                        return false;
                    }
                    break;
                case "use":
                    if (parts.Length > 1)
                    {
                        Use(parts[1]);
                        return false;
                    }
                    break;
                case "drop":
                    if (parts.Length > 1)
                    {
                        Drop(parts[1]);
                        return false;
                    }
                    break;
                case "desc":
                case "describe":
                case "where":
                    return false;
                case "help":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Output("There is no help here my friend.\n");
                    return false;
                case "exit":
                case "quit":
                case "end":
                case "bye":
                    return true;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Output($"I don't know how to {command}.\n");

            return false;
        }

        private void Move(string direction)
        {
            if (string.IsNullOrWhiteSpace(direction))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output("Go where exactly?\n");
            }

            var dx = 0;
            var dy = 0;

            switch (direction)
            {
                case "north":
                case "n":
                    dy = 1;
                    break;
                case "south":
                case "s":
                    dy = -1;
                    break;
                case "east":
                case "e":
                    dx = 1;
                    break;
                case "west":
                case "w":
                    dx = -1;
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Red;

            if (dx == 0 && dy == 0)
            {
                Output($"I can't move in the direction {direction}.\n");
                return;
            }

            if (CheckMove(_position.X + dx, _position.Y + dy, out var reason))
            {
                _position.X += dx;
                _position.Y += dy;
                return;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                Output($"I can't move in the direction {direction}.\n");
                return;
            }

            Output($"{reason}\n");
        }

        private bool CheckMove(int x, int y, out string reason)
        {
            reason = null;

            var location = _map.FirstOrDefault(l => l.Coords.X == x && l.Coords.Y == y);

            if (location == null)
            {
                return false;
            }

            reason = location.NonAccessibleDescription;

            return location.Accessible;
        }

        private void Take(string description)
        {
            var item = _location.Items.FirstOrDefault(i => string.Equals(i.Description, description, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output($"There is no {description.ToLower()} here.\n");
                return;
            }

            _items.Add(item);
            _location.Items.Remove(item);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Output($"You take the {item.Description.ToLower()}.\n");
        }

        private void Use(string description)
        {
            var item = _items.FirstOrDefault(i => string.Equals(i.Description, description, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output($"You aren't carrying a {description.ToLower()}.\n");
                return;
            }

            if (item.LocationsCanBeUsed.Any(l => l.X == _position.X && l.Y == _position.Y))
            {
                var result = item.Action.Invoke(_position);

                _map.First(l => l.Coords.X == result.CoordsToModify.X && l.Coords.Y == result.CoordsToModify.Y).Description = result.NewDescription;
                if (result.CoordsToMakeAccessible != null)
                {
                    _map.First(l => l.Coords.X == result.CoordsToMakeAccessible.X && l.Coords.Y == result.CoordsToMakeAccessible.Y).Accessible = true;
                }


                Console.ForegroundColor = ConsoleColor.Cyan;
                Output($"{result.ActionDescription}\n");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Output($"The {item.Description} can't be used here.\n");
        }

        private void Drop(string description)
        {
            var item = _items.FirstOrDefault(i => string.Equals(i.Description, description, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output($"You aren't carrying a {description.ToLower()}\n");
                return;
            }

            _location.Items.Add(item);
            _items.Remove(item);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Output($"You drop the {item.Description.ToLower()}\n");
        }
    }
}