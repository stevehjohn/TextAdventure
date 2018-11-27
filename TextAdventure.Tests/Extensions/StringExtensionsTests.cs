using NUnit.Framework;
using TextAdventure.Extensions;
// ReSharper disable StringLiteralTypo

namespace TextAdventure.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("Axe", "an")]
        [TestCase("Boat", "a")]
        [TestCase("Cat", "a")]
        [TestCase("Egg", "an")]
        public void Returns_correct_determiner_for_word(string word, string expected)
        {
            Assert.That(word.PrependDeterminer(), Is.EqualTo($"{expected} {word}"));
        }

        [Test]
        public void Does_not_fail_on_empty_string()
        {
            Assert.That(string.Empty.PrependDeterminer(), Is.EqualTo(string.Empty));
        }

        [TestCase("Smith", "S530")]
        [TestCase("Smythe", "S530")]
        [TestCase("a", "A000")]
        [TestCase("a", "A000")]
        public void Soundex_produces_expected_codes(string word, string expected)
        {
            Assert.That(word.ToSoundex(), Is.EqualTo(expected));
        }

        [TestCase("Smith", "Smythe")]
        [TestCase("move", "mve")]
        [TestCase("take", "tke")]
        [TestCase("take", "tak")]
        [TestCase("pick up", "pickup")]
        [TestCase("pik up", "pickup")]
        public void Soundex_tests(string left, string right)
        {
            Assert.That(left.ToSoundex(), Is.EqualTo(right.ToSoundex()));
        }
    }
}