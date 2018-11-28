using System;
using System.Linq;

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
                return null;
            }

            word = word.RemoveNonLetters();

            if (string.IsNullOrWhiteSpace(word))
            {
                return null;
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

        public static int GetLevenshteinDistance(this string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left))
            {
                throw new ArgumentException(nameof(left));
            }

            if (string.IsNullOrWhiteSpace(right))
            {
                throw new ArgumentException(nameof(right));
            }

            left = left.ToLower();
            right = right.ToLower();

            var matrix = new int[left.Length + 1, right.Length + 1];

            for (var i = 1; i <= left.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (var i = 1; i <= right.Length; i++)
            {
                matrix[0, i] = i;
            }

            for (var r = 1; r <= right.Length; r++)
            {
                for (var l = 1; l <= left.Length; l++)
                {
                    var cost = 0;

                    if (left[l - 1] != right[r - 1])
                    {
                        cost = 1;
                    }

                    matrix[l, r] = Min(matrix[l - 1, r] + 1, matrix[l, r - 1] + 1, matrix[l - 1, r - 1] + cost);
                }
            }

            return matrix[left.Length, right.Length];
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

        private static int Min(params int[] values)
        {
            return values.Min();
        }
    }
}