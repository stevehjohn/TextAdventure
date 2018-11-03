using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TextAdventure.Engine;
using TextAdventure.Infrastructure;
using TextAdventure.Models;

namespace TextAdventure.Tests.Engine
{
    [TestFixture]
    public class TextAdventureEngineTests
    {
        private Mock<IInputOutput> _inputOutput;
        private TextAdventureEngine _engine;

        [SetUp]
        public void SetUp()
        {
            _inputOutput = new Mock<IInputOutput>();

            _engine = new TextAdventureEngine(_inputOutput.Object, GetBasicMap());
        }

        [TestCase("go north", "North")]
        [TestCase("go south", "South")]
        [TestCase("go east", "East")]
        [TestCase("go west", "West")]
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

        private static List<Location> GetBasicMap()
        {
            return new List<Location>
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
