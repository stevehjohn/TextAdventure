using NUnit.Framework;
using TextAdventure.Extensions;

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
    }
}