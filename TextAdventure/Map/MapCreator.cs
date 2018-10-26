using System.Collections.Generic;
using System.Linq;
using TextAdventure.Models;

namespace TextAdventure.Map
{
    public static class MapCreator
    {
        public static List<Location> CreateMap()
        {
            var locations = new List<Location>
            {
                new Location
                {
                    Coords = new Coords(0, 0),
                    Description = "You are in a forest."
                },
                new Location
                {
                    Coords = new Coords(0, 1),
                    Description = "You are at the edge of a forest."
                },
                new Location
                {
                    Coords = new Coords(0, 2),
                    Description = "You are in a clearing.",
                    Items = new List<Item>
                    {
                        new Item
                        {
                            Description = "Axe",
                            LocationsCanBeUsed = new List<Coords>
                            {
                                new Coords(-1, 2)
                            },
                            SingleUse = false,
                            Action = c => new ItemUseResult
                            {
                                CoordsToModify = new Coords(-1, 2),
                                NewDescription = "You are at the edge of a river. There is a tree that you murdered crossing it. Convenient.",
                                ActionDescription = "You chop down the tree, and luckily it makes a bridge across the river. Woohoo!",
                                CoordsToMakeAccessible = new Coords(-2, 2)
                            }
                        }
                    }
                },
                new Location
                {
                    Coords = new Coords(-1, 2),
                    Description = "You are by a river. There is a large tree on the riverbank."
                },
                new Location
                {
                    Coords = new Coords(-2, 2),
                    Accessible = false,
                    NonAccessibleDescription = "You are unable to get across the river.",
                    Description = "You are at a riverbank.",
                    Event = (items, map) =>
                    {
                        var location = map.First(l => l.Coords.X == -1 && l.Coords.Y == 2);
                        location.Accessible = false;
                        location.NonAccessibleDescription = "You are unable to get across the river.";
                        return "There is a crash and the tree collapses into the river.";
                    }
                },
                new Location
                {
                    Coords = new Coords(-3, 2),
                    Description = "You are in the ruins of a castle.",
                    Event = (items, map) =>
                    {
                        items.Add(new Item
                        {
                            Description = "Magic wand"
                        });
                        return "A wizard walks towards you and hands you a magic wand. Then he buggers off.";
                    }
                },
                new Location
                {
                    Coords = new Coords(-3, 3),
                    Description = "You are on a castle drawbridge."
                },
                new Location
                {
                    Coords = new Coords(-3, 4),
                    Description = "You are in an enchanted shrubbery.",
                    Event = (items, map) =>
                    {
                        var location = map.First(l => l.Coords.X == -3 && l.Coords.Y == 3);
                        location.Accessible = false;
                        location.NonAccessibleDescription = "The drawbridge is up.";
                        return "You hear a loud clanking behind you - the castle drawbridge rises swiftly and closes.";
                    }
                }
            };

            return locations;
        }
    }
}