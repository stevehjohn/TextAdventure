using System;
using System.Collections.Generic;
using System.Linq;
using TextAdventure.Infrastructure;
using TextAdventure.Models;

namespace TextAdventure.Engine
{
    public class TextAdventureEngine
    {
        private readonly IOutput _output;
        private readonly List<Location> _map;
        private readonly List<Item> _items;

        private Coords _position;
        private Location _location;

        public TextAdventureEngine(IOutput output, List<Location> map)
        {
            _output = output;
            _map = map;
            _items = new List<Item>();
        }

        public void RunGame()
        {
            _output.Write(ConsoleColor.DarkGray, "Simple text adventure engine by Stevö John.\n");

            _position = new Coords(0, 0);

            while (true)
            {
                _location = _map.First(l => l.Coords.X == _position.X && l.Coords.Y == _position.Y);

                _output.Write(ConsoleColor.Blue, _location.Description);

                if (_location.Event != null)
                {
                    _output.Write(ConsoleColor.White, $"\n{_location.Event.Invoke(_items, _map)}");

                    if (_location.OneOffEvent)
                    {
                        _location.Event = null;
                    }
                }

                if (_location.Items.Count > 0)
                {
                    _output.Write(ConsoleColor.Cyan, "\nItems in the area:");

                    foreach (var item in _location.Items)
                    {
                        _output.Write(ConsoleColor.Green, item.Description);
                    }
                }

                if (_items.Count > 0)
                {
                    _output.Write(ConsoleColor.Cyan, "\nYou are carrying:");

                    foreach (var item in _items)
                    {
                        _output.Write(ConsoleColor.Green, item.Description);
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
                        _output.Write(ConsoleColor.Red, "Go where exactly?\n");
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
                    _output.Write(ConsoleColor.DarkGreen, "Arm yourself because no-one else here will save you.\n");
                    return false;
                case "exit":
                case "quit":
                case "end":
                case "bye":
                    return true;
            }

            _output.Write(ConsoleColor.Red, $"I don't know how to {command}.\n");

            return false;
        }

        private void Move(string direction)
        {
            if (string.IsNullOrWhiteSpace(direction))
            {
                _output.Write(ConsoleColor.Red, "Go where exactly?\n");
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

            if (dx == 0 && dy == 0)
            {
                _output.Write(ConsoleColor.Red, $"I can't move in the direction {direction}.\n");
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
                _output.Write(ConsoleColor.Red, $"I can't move in the direction {direction}.\n");
                return;
            }

            _output.Write(ConsoleColor.Red, $"{reason}\n");
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
                _output.Write(ConsoleColor.Red, $"There is no {description.ToLower()} here.\n");
                return;
            }

            _items.Add(item);
            _location.Items.Remove(item);

            _output.Write(ConsoleColor.Magenta, $"You take the {item.Description.ToLower()}.\n");
        }

        private void Use(string description)
        {
            var item = _items.FirstOrDefault(i => string.Equals(i.Description, description, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                _output.Write(ConsoleColor.Red, $"You aren't carrying a {description.ToLower()}.\n");
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


                _output.Write(ConsoleColor.Cyan, $"{result.ActionDescription}\n");
                return;
            }

            _output.Write(ConsoleColor.Red, $"The {item.Description} can't be used here.\n");
        }

        private void Drop(string description)
        {
            var item = _items.FirstOrDefault(i => string.Equals(i.Description, description, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                _output.Write(ConsoleColor.Red, $"You aren't carrying a {description.ToLower()}\n");
                return;
            }

            _location.Items.Add(item);
            _items.Remove(item);

            _output.Write(ConsoleColor.Magenta, $"You drop the {item.Description.ToLower()}\n");
        }
    }
}