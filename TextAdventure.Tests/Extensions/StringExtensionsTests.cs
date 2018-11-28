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

        [TestCase("smith", "S530")]
        [TestCase("smythe", "S530")]
        [TestCase("a", "A000")]
        [TestCase("move", "M100")]
        [TestCase("head", "H300")]
        [TestCase("go", "G000")]
        [TestCase("take", "T200")]
        [TestCase("pickup", "P210")]
        [TestCase("use", "U200")]
        [TestCase("drop", "D610")]
        [TestCase("desc", "D200")]
        [TestCase("describe", "D261")]
        [TestCase("where", "W600")]
        [TestCase("help", "H410")]
        [TestCase("cls", "C420")]
        [TestCase("clear", "C460")]
        [TestCase("exit", "E230")]
        [TestCase("quit", "Q300")]
        [TestCase("end", "E530")]
        [TestCase("bye", "B000")]
        public void Soundex_produces_expected_codes(string word, string expected)
        {
            Assert.That(word.ToSoundex(), Is.EqualTo(expected));
        }

        [TestCase("smith", "smythe")]
        [TestCase("move", "mve")]
        [TestCase("take", "tke")]
        [TestCase("take", "tak")]
        [TestCase("pick up", "pickup")]
        [TestCase("pik up", "pickup")]
        [TestCase("pic up", "pickup")]
        [TestCase("steve", "glyn", false)]
        public void Soundex_tests(string left, string right, bool isEqual = true)
        {
            if (isEqual)
            {
                Assert.That(left.ToSoundex(), Is.EqualTo(right.ToSoundex()));
            }
            else
            {
                Assert.True(left.ToSoundex() != right.ToSoundex());
            }
        }
    }
}