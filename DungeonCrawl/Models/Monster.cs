using System;
using System.Numerics;
using System.Collections.Generic;

namespace DungeonCrawl
{
    internal class Monster
    {
        public string name;
        public Vector2 position;
        public int hitpoints;
        public char symbol;
        public ConsoleColor color;
        public string behavior; // New property to define the monster's behavior

        public static Monster CreateMonster(string name, int hitpoints, char symbol, ConsoleColor color, Vector2 position, string behavior)
        {
            Monster monster = new Monster();
            monster.name = name;
            monster.hitpoints = hitpoints;
            monster.symbol = symbol;
            monster.color = color;
            monster.position = position;
            monster.behavior = behavior;
            return monster;
        }

        public static Monster CreateRandomMonster(Random random, Vector2 position)
        {
            int type = random.Next(6);
            return type switch
            {
                0 => CreateMonster("Goblin", 15, 'g', ConsoleColor.Green, position, "melee"),
                1 => CreateMonster("Bat Man", 9, 'M', ConsoleColor.Magenta, position, "ranged"),
                2 => CreateMonster("Orc", 25, 'o', ConsoleColor.Red, position, "charge"),
                3 => CreateMonster("Bunny", 1, 'B', ConsoleColor.Yellow, position, "flee"),
                4 => CreateMonster("Skeleton", 12, 'S', ConsoleColor.Gray, position, "summon"),
                5 => CreateMonster("Dragon", 50, 'D', ConsoleColor.Red, position, "fire_breath"),
                _ => CreateMonster("Zombie", 10, 'Z', ConsoleColor.Green, position, "melee")
            };
        }


        public static void ProcessEnemies(List<Monster> enemies, Map level, PlayerCharacter character, List<int> dirtyTiles, List<string> messages)
        {
            foreach (Monster enemy in enemies)
            {
                if (GetDistanceBetween(enemy.position, character.position) < 5)
                {
                    Vector2 enemyMove = new Vector2(0, 0);

                    // Determine movement direction
                    if (character.position.X < enemy.position.X)
                    {
                        enemyMove.X = -1;
                    }
                    else if (character.position.X > enemy.position.X)
                    {
                        enemyMove.X = 1;
                    }
                    else if (character.position.Y > enemy.position.Y)
                    {
                        enemyMove.Y = 1;
                    }
                    else if (character.position.Y < enemy.position.Y)
                    {
                        enemyMove.Y = -1;
                    }

                    int startTile = PositionToTileIndex(enemy.position, level);
                    Vector2 destinationPlace = enemy.position + enemyMove;

                    // Check if the enemy attacks the player
                    if (destinationPlace == character.position)
                    {
                        int damage = 1;
                        damage -= GetCharacterDefense(character);
                        if (damage <= 0)
                        {
                            damage = 1;
                        }

                        // Add attack description based on behavior
                        string attackDescription = enemy.behavior switch
                        {
                            "melee" => "slashes you with a weapon",
                            "ranged" => "shoots an arrow at you",
                            "charge" => "charges into you",
                            "flee" => "panics and flails at you",
                            "summon" => "summons dark energy to strike you",
                            "fire_breath" => "breathes fire at you",
                            _ => "attacks you"
                        };

                        character.hitpoints -= damage;
                        messages.Add($" {enemy.name} {attackDescription}, dealing {damage} damage!");
                    }
                    else
                    {
                        // Move the enemy if the destination is valid
                        Map.Tile destination = GetTileAtMap(level, destinationPlace);
                        if (destination == Map.Tile.Floor)
                        {
                            enemy.position = destinationPlace;
                            dirtyTiles.Add(startTile);
                        }
                        else if (destination == Map.Tile.Door)
                        {
                            enemy.position = destinationPlace;
                            dirtyTiles.Add(startTile);
                        }
                    }
                }
            }
        }



        private static int GetDistanceBetween(Vector2 A, Vector2 B)
        {
            return (int)Vector2.Distance(A, B);
        }

        private static int PositionToTileIndex(Vector2 position, Map level)
        {
            return (int)position.X + (int)position.Y * level.width;
        }

        private static int GetCharacterDefense(PlayerCharacter character)
        {
            if (character.armor != null)
            {
                return character.armor.quality;
            }
            else
            {
                return 0;
            }
        }

        private static Map.Tile GetTileAtMap(Map level, Vector2 position)
        {
            if (position.X >= 0 && position.X < level.width)
            {
                if (position.Y >= 0 && position.Y < level.height)
                {
                    int ti = (int)position.Y * level.width + (int)position.X;
                    return (Map.Tile)level.Tiles[ti];
                }
            }
            return Map.Tile.Wall;
        }
    }
}
