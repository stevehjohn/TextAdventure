using System.Collections.Generic;
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
                    Accessible = true,
                    Description = "You are in a forest."
                },
                new Location
                {
                    Coords = new Coords(0, 1),
                    Accessible = true,
                    Description = "The trees are thinning out."
                }
            };

            return locations;
        }
    }
}