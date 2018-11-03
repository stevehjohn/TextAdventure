namespace TextAdventure.Extensions
{
    public static class StringExtensions
    {
        public static string PrependDeterminer(this string noun)
        {
            if (string.IsNullOrWhiteSpace(noun))
            {
                return noun;
            }

            switch (noun.ToLower()[0])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                    return $"an {noun}";
                default:
                    return $"a {noun}";
            }
        }
    }
}