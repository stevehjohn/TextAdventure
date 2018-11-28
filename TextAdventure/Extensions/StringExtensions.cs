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

        public static string ToSoundex(this string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return string.Empty;
            }

            word = word.RemoveNonLetters();

            if (string.IsNullOrWhiteSpace(word))
            {
                return string.Empty;
            }

            var code = word[0];

            word = word.Substring(1);

            word = word.Replace("a", "-").Replace("e", "-").Replace("i", "-").Replace("o", "-").Replace("u", "-")
                       .Replace("h", "-").Replace("w", "-").Replace("y", "-")
                       .Replace("b", "1").Replace("f", "1").Replace("p", "1").Replace("v", "1")
                       .Replace("c", "2").Replace("g", "2").Replace("j", "2").Replace("k", "2").Replace("q", "2").Replace("s", "2").Replace("x", "2").Replace("z", "2")
                       .Replace("d", "3").Replace("t", "3")
                       .Replace("l", "4")
                       .Replace("m", "5").Replace("n", "5")
                       .Replace("r", "6");

            bool paired;

            do
            {
                paired = false;
                for (var i = 0; i < word.Length - 1; i++)
                {
                    if (word[i] != word[i + 1])
                    {
                        continue;
                    }

                    word = word.Remove(i, 1);
                    paired = true;
                }
            } while (paired);

            word = word.Replace("-", string.Empty);

            if (word.Length > 3)
            {
                word = word.Substring(0, 3);
            }

            if (word.Length < 3)
            {
                word = $"{word}{new string('0', 3 - word.Length)}";
            }

            return $"{code}{word}".ToUpper();
        }

        private static string RemoveNonLetters(this string word)
        {
            var result = string.Empty;

            foreach (var letter in word.ToLower())
            {
                if (letter >= 'a' && letter <= 'z')
                {
                    result += letter;
                }
            }

            return result;
        }
    }
}