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
        private int _commandNumber;
        private TextAdventureEngine _engine;

        [SetUp]
        public void SetUp()
        {
            _inputOutput = new Mock<IInputOutput>();
            _map = GetBasicMap();
            _commandNumber = 0;

            _engine = new TextAdventureEngine(_inputOutput.Object, _map);
        }

        [TestCase("go north", "North")]
        [TestCase("go south", "South")]
        [TestCase("go east", "East")]
        [TestCase("go west", "West")]
        [TestCase("move north", "North")]
        [TestCase("head north", "North")]
        [TestCase("go", "Go where exactly?\n")]
        [TestCase("go ", "Go where exactly?\n")]
        public void Interprets_move_instructions_correctly(string instruction, string expected)
        {
            SetCommandSequence(instruction);

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(It.IsAny<ConsoleColor>(), expected, true));
        }

        [Test]
        public void Pickup_and_drop_items_correctly()
        {
            SetCommandSequence("take axe", "go north", "drop axe");

            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 0)
                .Items = new List<Item>
                         {
                             new Item
                             {
                                 Description = "Axe"
                             }
                         };

            _engine.RunGame();

            Assert.True(_map.First(l => l.Coords.X == 0 && l.Coords.Y == 1).Items.Any(i => i.Description == "Axe"));

            _inputOutput.Verify(io => io.Write(ConsoleColor.Cyan, "\nYou are carrying:", true));
            _inputOutput.Verify(io => io.Write(ConsoleColor.Green, "Axe", true));
        }

        [Test]
        public void Events_fire()
        {
            SetCommandSequence("go north");

            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 1)
                .Event = (items, locations) => "Something explodes!";

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.White, "\nSomething explodes!", true));
        }

        [Test]
        public void Cannot_move_out_of_map()
        {
            SetCommandSequence("go north", "go north");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "I can't move in the direction north.\n", true));
        }

        [Test]
        public void Gives_a_reason_location_cannot_be_reached()
        {
            var location = _map.First(l => l.Coords.X == 0 && l.Coords.Y == 1);
            location.Accessible = false;
            location.NonAccessibleDescription = "There is no way to cross the river.";

            SetCommandSequence("go north");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "There is no way to cross the river.\n", true));
        }

        [Test]
        public void Describe_repeats_location_description()
        {
            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 0).Description = "You are at the start of an adventure.";

            SetCommandSequence("describe");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Blue, "You are at the start of an adventure.", true), Times.Exactly(2));
        }

        [Test]
        public void User_cannot_drop_what_is_not_carried_and_uses_correct_grammar()
        {
            SetCommandSequence("drop axe", "drop cat");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "You aren't carrying an axe.\n", true));
            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "You aren't carrying a cat.\n", true));
        }

        [Test]
        public void Help_responds_accordingly()
        {
            SetCommandSequence("help");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.DarkGreen, "Arm yourself because no-one else here will save you.\n", true));
        }

        [Test]
        public void Items_cannot_exceed_usage_count()
        {
            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 0)
                .Items
                .Add(new Item
                     {
                         Description = "Axe",
                         Uses = 1,
                         LastUseResponse = "The axe broke.",
                         LocationsCanBeUsed = new List<Coords>
                                              {
                                                  new Coords(0, 0)
                                              },
                         Action = c => new ItemUseResult
                                       {
                                           ActionDescription = "You chop down a tree."
                                       }
                     });

            SetCommandSequence("take axe", "use axe", "use axe");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "The axe broke.\n", true));
            _inputOutput.Verify(io => io.Write(ConsoleColor.Magenta, "You drop the axe.\n", true));
        }

        [Test]
        public void Using_item_modifies_location_description()
        {
            var location = _map.First(l => l.Coords.X == 0 && l.Coords.Y == 0);
            location.Description = "There is a tree.";
            location.Items.Add(new Item
                               {
                                   Description = "Axe",
                                   Uses = int.MaxValue,
                                   LocationsCanBeUsed = new List<Coords>
                                                        {
                                                            new Coords(0, 0)
                                                        },
                                   Action = c => new ItemUseResult
                                                 {
                                                     CoordsToModify = new Coords(0, 0),
                                                     NewDescription = "There is a felled tree."
                                                 }
                               });

            SetCommandSequence("take axe", "use axe");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Blue, "There is a tree.", true));
            Assert.That(_map.First(l => l.Coords.X == 0 && l.Coords.Y == 0).Description, Is.EqualTo("There is a felled tree."));
        }

        [Test]
        public void Responds_to_unknown_commands()
        {
            SetCommandSequence("bugger off");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "I don't know how to bugger off.\n", true));
        }

        [Test]
        public void Cannot_use_what_is_not_carried()
        {
            SetCommandSequence("use axe");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "You aren't carrying an axe.\n", true));
        }

        [Test]
        public void Cannot_take_what_is_not_there()
        {
            SetCommandSequence("take axe");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "There is no axe here.\n", true));
        }

        [Test]
        public void Cannot_use_item_everywhere()
        {
            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 0)
                .Items
                .Add(new Item
                     {
                         Description = "Axe",
                         LocationsCanBeUsed = new List<Coords>
                                              {
                                                  new Coords(0, 1)
                                              }
                     });

            SetCommandSequence("take axe", "use axe");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "The axe can't be used here.\n", true));
        }

        [Test]
        public void Using_item_outputs_description_of_action()
        {
            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 0)
                .Items
                .Add(new Item
                     {
                         Description = "Axe",
                         LocationsCanBeUsed = new List<Coords>
                                              {
                                                  new Coords(0, 0)
                                              },
                         Action = c => new ItemUseResult
                                       {
                                           ActionDescription = "You chop the tree down."
                                       }
                     });

            SetCommandSequence("take axe", "use axe");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Cyan, "You chop the tree down.\n", true));
        }

        [Test]
        public void Using_item_makes_location_accessible()
        {
            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 1)
                .Accessible = false;

            _map.First(l => l.Coords.X == 0 && l.Coords.Y == 0)
                .Items
                .Add(new Item
                     {
                         Description = "Axe",
                         LocationsCanBeUsed = new List<Coords>
                                              {
                                                  new Coords(0, 0)
                                              },
                         Action = c => new ItemUseResult
                                       {
                                           ActionDescription = "You chop the tree down.",
                                           CoordsToMakeAccessible = new Coords(0, 1)
                                       }
                     });

            SetCommandSequence("take axe", "use axe");

            _engine.RunGame();

            Assert.True(_map.First(l => l.Coords.X == 0 && l.Coords.Y == 1).Accessible);
        }

        [Test]
        public void Take_responds_with_no_parameter()
        {
            SetCommandSequence("take");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "What do you want to pickup?\n", true));
        }

        [Test]
        public void Use_responds_with_no_parameter()
        {
            SetCommandSequence("use");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "What do you want to use?\n", true));
        }

        [Test]
        public void Drop_responds_with_no_parameter()
        {
            SetCommandSequence("drop");

            _engine.RunGame();

            _inputOutput.Verify(io => io.Write(ConsoleColor.Red, "What do you want to drop?\n", true));
        }

        private void SetCommandSequence(params string[] commands)
        {
            _inputOutput.Setup(io => io.Input())
                        .Returns(() =>
                        {
                            if (_commandNumber >= commands.Length)
                            {
                                return "bye";
                            }

                            _commandNumber++;
                            return commands[_commandNumber - 1];
                        });
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
