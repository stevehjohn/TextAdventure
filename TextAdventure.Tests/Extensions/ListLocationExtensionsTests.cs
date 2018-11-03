using NUnit.Framework;
using System.Collections.Generic;
using TextAdventure.Extensions;
using TextAdventure.Models;

namespace TextAdventure.Tests.Extensions
{
    [TestFixture]
    public class ListLocationExtensionsTests
    {
        [Test]
        public void Returns_correct_item_from_locations_list()
        {
            var locations = new List<Location>
                            {
                                new Location
                                {
                                    Coords = new Coords(0, 0),
                                    Description = "Start"
                                },
                                new Location
                                {
                                    Coords = new Coords(-1, 0),
                                    Description = "West"
                                }
                            };

            Assert.That(locations.GetLocation(0, 0).Description, Is.EqualTo("Start"));
            Assert.That(locations.GetLocation(-1, 0).Description, Is.EqualTo("West"));
            Assert.That(locations.GetLocation(-2, 0), Is.Null);

            Assert.That(locations.GetLocation(new Coords(0, 0)).Description, Is.EqualTo("Start"));
            Assert.That(locations.GetLocation(new Coords(-1, 0)).Description, Is.EqualTo("West"));
            Assert.That(locations.GetLocation(new Coords(-2, 0)), Is.Null);
        }
    }
}