using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TextAdventure.Engine;
using TextAdventure.Infrastructure;
using TextAdventure.Models;

namespace TextAdventure.Tests.Engine
{
    [TestFixture]
    public class TextAdventureEngineTests
    {
        private Mock<IInputOutput> _inputOutput;
        private List<Location> _map;
        private TextAdventureEngine _engine;

        [SetUp]
        public void SetUp()
        {
            _inputOutput = new Mock<IInputOutput>();
            _map = GetBasicMap();

            _engine = new TextAdventureEngine(_inputOutput.Object, _map);
        }

        [TestCase("go north", "North")]
        [TestCase("go south", "South")]
        [TestCase("go east", "East")]
        [TestCase("go west", "West")]
        [TestCase("move north", "North")]
        [TestCase("head north", "North")]
        [TestCase("go", "Go where exactly?\n")]
        public void Interprets_move_instructions_correctly(string instruction, string expected)
        {
            var count = 0;

            _inputOutput.Setup(io => io.Input())
                        .Returns(() =>
                        {
                            count++;
                            return count == 1 ? instruction : "bye";
                        });

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(It.IsAny<ConsoleColor>(), expected, true));
        }

        [Test]
        public void Pickup_and_drop_items_correctly()
        {
            var count = 0;

            _inputOutput.Setup(io => io.Input())
                        .Returns(() =>
                        {
                            count++;
                            switch (count)
                            {
                                case 1:
                                    return "take axe";
                                case 2:
                                    return "go north";
                                case 3:
                                    return "drop axe";
                                default:
                                    return "bye";
                            }
                        });

            _engine.RunGame();

            Assert.True(_map.First(l => l.Coords.X == 0 && l.Coords.Y == 1).Items.Any(i => i.Description == "Axe"));
        }

        private static List<Location> GetBasicMap()
        {
            return new List<Location>
            {
                new Location
                {
                    Coords = new Coords(0, 0),
                    Description = "Start",
                    Items = new List<Item>
                    {
                        new Item
                        {
                            Description = "Axe"
                        }
                    }
                },
                new Location
                {
                    Coords = new Coords(-1, 0),
                    Description = "West"
                },
                new Location
                {
                    Coords = new Coords(0, 1),
                    Description = "North"
                },
                new Location
                {
                    Coords = new Coords(1, 0),
                    Description = "East"
                },
                new Location
                {
                    Coords = new Coords(0, -1),
                    Description = "South"
                }
            };
        }
    }
}
