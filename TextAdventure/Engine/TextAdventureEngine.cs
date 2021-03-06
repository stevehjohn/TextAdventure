﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextAdventure.Extensions;
using TextAdventure.Infrastructure;
using TextAdventure.Models;

namespace TextAdventure.Engine
{
    public class TextAdventureEngine
    {
        private readonly IInputOutput _inputOutput;
        private readonly List<Location> _map;
        private readonly List<Item> _items;

        private Coords _position;
        private Location _location;

        public TextAdventureEngine(IInputOutput inputOutput, List<Location> map)
        {
            _inputOutput = inputOutput;
            _map = map;
            _items = new List<Item>();
        }

        public void RunGame()
        {
            _inputOutput.Write(ConsoleColor.DarkGray, "Simple text adventure engine by Stevö John.\n");

            _position = new Coords(0, 0);

            while (true)
            {
                _location = _map.GetLocation(_position);

                _inputOutput.Write(ConsoleColor.Blue, _location.Description);

                if (_location.Event != null)
                {
                    _inputOutput.Write(ConsoleColor.White, $"\n{_location.Event.Invoke(_items, _map)}");

                    if (_location.OneOffEvent)
                    {
                        _location.Event = null;
                    }
                }

                if (_location.Items.Count > 0)
                {
                    _inputOutput.Write(ConsoleColor.Cyan, "\nItems in the area:");

                    foreach (var item in _location.Items)
                    {
                        _inputOutput.Write(ConsoleColor.Green, item.Description);
                    }
                }

                if (_items.Count > 0)
                {
                    _inputOutput.Write(ConsoleColor.Cyan, "\nYou are carrying:");

                    foreach (var item in _items)
                    {
                        _inputOutput.Write(ConsoleColor.Green, item.Description);
                    }
                }

                string command;

                _inputOutput.Write(ConsoleColor.Yellow, string.Empty);

                do
                {
                    _inputOutput.Write(ConsoleColor.Yellow, "> ", false);
                    command = _inputOutput.Input();
                } while (string.IsNullOrWhiteSpace(command));

                _inputOutput.Write(ConsoleColor.Yellow, string.Empty);

                if (Do(command))
                {
                    break;
                }
            }
        }

        private bool Do(string command)
        {
            var parts = command.ToLower().Split(" ");

            switch (parts[0].ToSoundex())
            {
                case "M100": // move
                case "H300": // head
                case "G000": // go
                    if (parts.Length > 1)
                    {
                        Move(parts[1]);
                    }
                    else
                    {
                        _inputOutput.Write(ConsoleColor.Red, "Go where exactly?\n");
                    }
                    return false;
                case "T200": // take
                case "P210": // pickup
                    if (parts.Length > 1)
                    {
                        Take(parts[1]);
                        return false;
                    }
                    else
                    {
                        _inputOutput.Write(ConsoleColor.Red, "What do you want to pickup?\n");
                    }
                    return false;
                case "U200": // use
                    if (parts.Length > 1)
                    {
                        Use(parts[1]);
                        return false;
                    }
                    else
                    {
                        _inputOutput.Write(ConsoleColor.Red, "What do you want to use?\n");
                    }
                    return false;
                case "D610": // drop
                    if (parts.Length > 1)
                    {
                        Drop(parts[1]);
                        return false;
                    }
                    else
                    {
                        _inputOutput.Write(ConsoleColor.Red, "What do you want to drop?\n");
                    }
                    return false;
                case "D200": // desc
                case "D261": // describe
                case "W600": // where
                    return false;
                case "H410":
                    _inputOutput.Write(ConsoleColor.DarkGreen, "Arm yourself because no-one else here will save you.\n");
                    return false;
                case "C420": // cls
                case "C460": // clear
                    _inputOutput.Clear();
                    return false;
                //case "O135": // options
                //    return false;
                case "E230": // exit
                case "Q300": // quit
                case "E530": // end
                case "B000": // bye
                    return true;
                default:
                    _inputOutput.Write(ConsoleColor.Red, $"I don't know how to {command}.\n");
                    return false;
            }
        }

        private void Move(string direction)
        {
            if (string.IsNullOrWhiteSpace(direction))
            {
                _inputOutput.Write(ConsoleColor.Red, "Go where exactly?\n");
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
                default:
                    _inputOutput.Write(ConsoleColor.Red, $"I can't move in the direction {direction}.\n");
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
                _inputOutput.Write(ConsoleColor.Red, $"I can't move in the direction {direction}.\n");
                return;
            }

            _inputOutput.Write(ConsoleColor.Red, $"{reason}\n");
        }

        private bool CheckMove(int x, int y, out string reason)
        {
            reason = null;

            var location = _map.GetLocation(x, y);

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
                _inputOutput.Write(ConsoleColor.Red, $"There is no {description.ToLower()} here.\n");
                return;
            }

            _items.Add(item);
            _location.Items.Remove(item);

            _inputOutput.Write(ConsoleColor.Magenta, $"You take the {item.Description.ToLower()}.\n");
        }

        private void Use(string description)
        {
            var item = _items.FirstOrDefault(i => string.Equals(i.Description, description, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                _inputOutput.Write(ConsoleColor.Red, $"You aren't carrying {description.ToLower().PrependDeterminer()}.\n");
                return;
            }

            if (item.LocationsCanBeUsed.Any(l => l.X == _position.X && l.Y == _position.Y))
            {
                var result = item.Action.Invoke(_position);

                if (result.CoordsToModify != null)
                {
                    _map.GetLocation(result.CoordsToModify).Description = result.NewDescription;
                }

                if (result.CoordsToMakeAccessible != null)
                {
                    _map.GetLocation(result.CoordsToMakeAccessible).Accessible = true;
                }

                _inputOutput.Write(ConsoleColor.Cyan, $"{result.ActionDescription}\n");

                item.Uses--;
                if (item.Uses == 0)
                {
                    _inputOutput.Write(ConsoleColor.Red, $"{item.LastUseResponse}\n");
                    _inputOutput.Write(ConsoleColor.Magenta, $"You drop the {item.Description.ToLower()}.\n");
                }

                return;
            }

            _inputOutput.Write(ConsoleColor.Red, $"The {item.Description.ToLower()} can't be used here.\n");
        }

        private void Drop(string description)
        {
            var item = _items.FirstOrDefault(i => string.Equals(i.Description, description, StringComparison.InvariantCultureIgnoreCase));

            if (item == null)
            {
                _inputOutput.Write(ConsoleColor.Red, $"You aren't carrying {description.ToLower().PrependDeterminer()}.\n");
                return;
            }

            _location.Items.Add(item);
            _items.Remove(item);

            _inputOutput.Write(ConsoleColor.Magenta, $"You drop the {item.Description.ToLower()}.\n");
        }
    }
}