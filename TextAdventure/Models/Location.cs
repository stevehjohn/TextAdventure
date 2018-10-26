using System.Collections.Generic;

namespace TextAdventure.Models
{
    public class Location
    {
        public Coords Coords { get; set; }
        public List<Item> Items { get; set; }
        public bool Accessible { get; set; }
        public string Description { get; set; }

        public Location()
        {
            Items = new List<Item>();
        }

        public Location(int x, int y, string description)
        {
            Coords = new Coords(x, y);
            Description = description;
            Items = new List<Item>();
        }
    }
}