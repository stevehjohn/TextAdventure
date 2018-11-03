using System.Collections.Generic;
using System.Linq;
using TextAdventure.Models;

namespace TextAdventure.Extensions
{
    public static class ListLocationExtensions
    {
        public static Location GetLocation(this List<Location> locations, int x, int y)
        {
            return locations.FirstOrDefault(l => l.Coords.X == x && l.Coords.Y == y);
        }

        public static Location GetLocation(this List<Location> locations, Coords coords)
        {
            return locations.FirstOrDefault(l => l.Coords.X == coords.X && l.Coords.Y == coords.Y);
        }
    }
}