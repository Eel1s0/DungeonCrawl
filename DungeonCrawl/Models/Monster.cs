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

        public static Monster CreateMonster(string name, int hitpoints, char symbol, ConsoleColor color, Vector2 position)
        {
            Monster monster = new Monster();
            monster.name = name;
            monster.hitpoints = hitpoints;
            monster.symbol = symbol;
            monster.color = color;
            monster.position = position;
            return monster;
        }

        public static Monster CreateRandomMonster(Random random, Vector2 position)
        {
            int type = random.Next(4);
            return type switch
            {
                0 => CreateMonster("Goblin", 5, 'g', ConsoleColor.Green, position),
                1 => CreateMonster("Bat Man", 2, 'M', ConsoleColor.Magenta, position),
                2 => CreateMonster("Orc", 15, 'o', ConsoleColor.Red, position),
                3 => CreateMonster("Bunny", 1, 'B', ConsoleColor.Yellow, position)
            };
        }

        public static void ProcessEnemies(List<Monster> enemies, Map level, PlayerCharacter character, List<int> dirtyTiles, List<string> messages)
        {
            foreach (Monster enemy in enemies)
            {
                if (GetDistanceBetween(enemy.position, character.position) < 5)
                {
                    Vector2 enemyMove = new Vector2(0, 0);

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
                    if (destinationPlace == character.position)
                    {
                        int damage = 1;
                        damage -= GetCharacterDefense(character);
                        if (damage <= 0)
                        {
                            damage = 1;
                        }
                        character.hitpoints -= damage;
                        messages.Add($"{enemy.name} hits you for {damage} damage!");
                    }
                    else
                    {
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
