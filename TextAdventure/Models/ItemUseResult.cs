namespace TextAdventure.Models
{
    public class ItemUseResult
    {
        public Coords CoordsToModify { get; set; }
        public string NewDescription { get; set; }
        public string ActionDescription { get; set; }
        public Coords CoordsToMakeAccessible { get; set; }
    }
}