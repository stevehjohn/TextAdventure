using System;
using System.Collections.Generic;

namespace TextAdventure.Models
{
    public class Item
    {
        public string Description { get; set; }
        public Func<Coords, ItemUseResult> Action { get; set; }
        public List<Coords> LocationsCanBeUsed { get; set;  }
        public int Uses { get; set; }
        public string LastUseResponse { get; set; }

        public Item()
        {
            LocationsCanBeUsed = new List<Coords>();
        }
    }
}