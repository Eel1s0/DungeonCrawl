using System;
using System.Numerics;
using DungeonCrawl;

namespace DungeonCrawl
{
	internal class Program
	{
        public const int INFO_HEIGHT = 6;
        public const int COMMANDS_WIDTH = 12;
        public const int ENEMY_CHANCE = 3;
        public const int ITEM_CHANCE = 4;

        // Room generation 
        public const int ROOM_AMOUNT = 12;
        public const int ROOM_MIN_W = 4;
        public const int ROOM_MAX_W = 12;
        public const int ROOM_MIN_H = 4;
        public const int ROOM_MAX_H = 8;

		static void Main(string[] args)
		{
            List<Monster> monsters = null;
			List<Item> items = null;
			PlayerCharacter player = null;
			Map currentLevel = null;
			Random random = new Random();

			List<int> dirtyTiles = new List<int>();
			List<string> messages = new List<string>();

            Trader trader = new Trader();
            int currentLevelNumber = 1;

            // Initialize the trader's inventory for the first level
            trader.RestockInventory(random, currentLevelNumber);

            // Main loop
            GameState state = GameState.CharacterCreation;
			while (state != GameState.Quit)
			{
				switch (state)
				{
					case GameState.CharacterCreation:
                        // Character creation screen
                        player = PlayerCharacter.CreateCharacter();
                        Console.CursorVisible = false;
						Console.Clear();

                        // Map Creation 
                        currentLevel = Map.CreateMap(random);

                        // Enemy init
                        monsters = CreateEnemies(currentLevel, random);
						// Item init
						items = CreateItems(currentLevel, random);
						// Player init
						Map.PlacePlayerToMap(player, currentLevel);
						Map.PlaceStairsToMap(currentLevel);
						state = GameState.GameLoop;
						break;
					case GameState.GameLoop:
						Map.DrawMap(currentLevel, dirtyTiles);
						dirtyTiles.Clear();
						DrawEnemies(monsters);
						DrawItems(items);

						DrawPlayer(player);
						DrawCommands();
						DrawInfo(player, monsters, items, messages);
                        // Draw map
                        // Draw information
                        // Wait for player command
                        // Process player command

                        if (player.gold >= 400)
                        {
                            state = GameState.Victory;
                            break;
                        }
                        while (true)
						{
							messages.Clear();
							PlayerTurnResult result = DoPlayerTurn(currentLevel, player, monsters, items, dirtyTiles, messages);
							DrawInfo(player, monsters, items, messages);
							if (result == PlayerTurnResult.TurnOver)
							{
								break;
							}
							else if (result == PlayerTurnResult.OpenInventory)
							{
								Console.Clear();
								state = GameState.Inventory;
								break;
							}
                            else if (result == PlayerTurnResult.OpenTrader)
                            {
                                Console.Clear();
                                state = GameState.Trader;
                                break;
                            }
                            else if (result == PlayerTurnResult.NextLevel)
							{
                                currentLevelNumber++;
                                currentLevel = Map.CreateMap(random);
								monsters = CreateEnemies(currentLevel, random);
								items = CreateItems(currentLevel, random);
								Map.PlacePlayerToMap(player, currentLevel);
								Map.PlaceStairsToMap(currentLevel);
                                trader.RestockInventory(random, currentLevelNumber);
                                Console.Clear();
								break;
							}


                        }
                        // Either do computer turn or wait command again
                        // Do computer turn
                        // Process enemies
                        Monster.ProcessEnemies(monsters, currentLevel, player, dirtyTiles, messages);

                        DrawInfo(player, monsters, items, messages);

						// Is player dead?
						if (player.hitpoints < 0)
						{
							state = GameState.DeathScreen;
						}

						break;
					case GameState.Inventory:
						// Draw inventory 
						PlayerTurnResult inventoryResult = DrawInventory(player, messages);
						if (inventoryResult == PlayerTurnResult.BackToGame)
						{
							state = GameState.GameLoop;
							Map.DrawMapAll(currentLevel);
							DrawInfo(player, monsters, items, messages);
						}
						// Read player command
						// Change back to game loop
						break;
					case GameState.DeathScreen:
						DrawEndScreen(random);
						// Animation is over
						Console.SetCursorPosition(Console.WindowWidth/2 - 4, Console.WindowHeight / 2);
						Print("YOU DIED", ConsoleColor.Yellow);
						Console.SetCursorPosition(Console.WindowWidth/2 - 4, Console.WindowHeight / 2 + 1);
						while(true)
						{ 
							Print("Play again (y/n)", ConsoleColor.Gray);
							ConsoleKeyInfo answer = Console.ReadKey();
							if (answer.Key == ConsoleKey.Y)
							{
								state = GameState.CharacterCreation;
                                currentLevelNumber = 1; // Reset the level number
                                trader.RestockInventory(random, currentLevelNumber); // Reset trader inventory
                                break;
							}
							else if (answer.Key == ConsoleKey.N)
							{
								state = GameState.Quit;
								break;
							}
						}
						break;
                    case GameState.Trader:
                        Console.Clear();
                        Console.WriteLine("Welcome to the Trader!");
                        Console.WriteLine("1. Buy items");
                        Console.WriteLine("2. Sell items");
                        Console.WriteLine("Press 'B' to go back.");
                        string traderInput = Console.ReadLine();

                        if (traderInput.ToLower() == "b")
                        {
                            state = GameState.GameLoop;
                            break;
                        }

                        if (traderInput == "1")
                        {
                            trader.DisplayInventory();
                            Console.WriteLine("Enter the number of the item you want to buy, or press 'B' to go back.");
                            string input = Console.ReadLine();

                            if (input.ToLower() == "b")
                            {
                                break;
                            }

                            if (int.TryParse(input, out int itemIndex))
                            {
                                trader.BuyItem(player, itemIndex - 1, messages);
                            }
                            else
                            {
                                messages.Add("Invalid input.");
                            }
                        }
                        else if (traderInput == "2")
                        {
                            Console.Clear();
                            Console.WriteLine("Your Inventory:");
                            for (int i = 0; i < player.inventory.Count; i++)
                            {
                                Item item = player.inventory[i];
                                Console.WriteLine($"{i + 1}. {item.name} ({item.type}) - Sell for {item.quality / 2} gold");
                            }
                            Console.WriteLine("Enter the number of the item you want to sell, or press 'B' to go back.");
                            string input = Console.ReadLine();

                            if (input.ToLower() == "b")
                            {
                                break;
                            }

                            if (int.TryParse(input, out int itemIndex))
                            {
                                trader.SellItem(player, itemIndex - 1, messages);
                            }
                            else
                            {
                                messages.Add("Invalid input.");
                            }
                        }

                        DrawInfo(player, monsters, items, messages);
                        break;
                    case GameState.Victory:
                        Console.Clear();
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2);
                        Print("CONGRATULATIONS!", ConsoleColor.Green);
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 15, Console.WindowHeight / 2 + 1);
                        Print($"You have gathered {player.gold} gold and won the game!", ConsoleColor.Yellow);
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 + 2);
                        Print("Press any key to exit.", ConsoleColor.Gray);
                        Console.ReadKey();
                        state = GameState.Quit;
                        break;



                }
                ;
			}
			Console.ResetColor();
			Console.Clear();
			Console.CursorVisible = true;
		}

		static void PrintLine(string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text);
		}
		public static void Print(string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(text);
		}
		static void PrintLine(string text)
		{
			Console.WriteLine(text);
		}
		static void Print(string text)
		{
			Console.Write(text);
		}

		static void Print(char symbol, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(symbol);
		}

		public static void DrawBrickBg()
		{
			// Draw tiles
			Console.BackgroundColor = ConsoleColor.DarkGray;
			for (int y = 0; y < Console.WindowHeight; y++)
			{
				Console.SetCursorPosition(0, y);
				for (int x = 0; x < Console.WindowWidth; x++)
				{
					if ((x + y) % 3 == 0)
					{
						Print("|", ConsoleColor.Black);
					}
					else
					{
						Print(" ", ConsoleColor.DarkGray);
					}
				}
			}
		}

		public static void DrawRectangle(int x, int y, int width, int height, ConsoleColor color)
		{
			Console.BackgroundColor = color;
			for (int dy = y; dy < y + height; dy++)
			{
				Console.SetCursorPosition(x, dy);
				for (int dx = x; dx < x + width; dx++)
				{
					Print(" ");
				}
			}
		}

		public static void DrawRectangleBorders(int x, int y, int width, int height, ConsoleColor color, string symbol)
		{
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = color;
			for (int dx = x; dx < x + width; dx++)
			{
				Print(symbol);
			}

			for (int dy = y; dy < y + height; dy++)
			{
				Console.SetCursorPosition(x, dy);

				Print(symbol);
				Console.SetCursorPosition(x + width - 1, dy);
				Print(symbol);
			}
		}
		static void DrawEndScreen(Random random)
		{
			// Run death animation: blood flowing down the screen in columns
			// Wait until keypress
			byte[] speeds = new byte[Console.WindowWidth];
			byte[] ends = new byte[Console.WindowWidth];
			for (int i = 0; i < speeds.Length; i++)
			{
				speeds[i] = (byte)random.Next(1, 4);
				ends[i] = 0;
			}
			Console.BackgroundColor = ConsoleColor.DarkRed;
			Console.ForegroundColor = ConsoleColor.White;
			
			
			for (int row = 0; row < Console.WindowHeight - 2; row++)
			{
				Console.SetCursorPosition(0, row);
				for (int i = 0; i < Console.WindowWidth; i++)
				{
					Console.Write(" ");
				}
				Thread.Sleep(100);
			}
			
		}
		static List<Monster> CreateEnemies(Map level, Random random)
		{
			List<Monster> monsters = new List<Monster>();

			for (int y = 0; y < level.height; y++)
			{
				for (int x = 0; x < level.width; x++)
				{
					int ti = y * level.width + x;
					if (level.Tiles[ti] == Map.Tile.Monster)
					{
                        Monster m = Monster.CreateRandomMonster(random, new Vector2(x, y));
                        monsters.Add(m);
						level.Tiles[ti] = (sbyte)Map.Tile.Floor;
					}
				}
			}
			return monsters;
		}

		
		static List<Item> CreateItems(Map level, Random random)
		{
			List<Item> items = new List<Item>();

			for (int y = 0; y < level.height; y++)
			{
				for (int x = 0; x < level.width; x++)
				{
					int ti = y * level.width + x;
					if (level.Tiles[ti] == Map.Tile.Item)
					{
                        Item m = Item.CreateRandomItem(random, new Vector2(x, y));
                        items.Add(m);
						level.Tiles[ti] = (sbyte)Map.Tile.Floor;
					}
				}
			}
			return items;
		}


		
		static void DrawEnemies(List<Monster> enemies)
		{
			foreach (Monster m in enemies)
			{
				Console.SetCursorPosition((int)m.position.X, (int)m.position.Y);
				Print(m.symbol, m.color);
			}
		}

		static void DrawItems(List<Item> items)
		{
			foreach (Item m in items)
			{
				Console.SetCursorPosition((int)m.position.X, (int)m.position.Y);
				char symbol = '$';
				ConsoleColor color = ConsoleColor.Yellow;
				switch (m.type)
				{
					case ItemType.Armor:
						symbol = '[';
						color = ConsoleColor.White;
						break;
					case ItemType.Weapon:
						symbol = '}';
						color = ConsoleColor.Cyan;
						break;
					case ItemType.Treasure:
						symbol = '$';
						color = ConsoleColor.Yellow;
						break;
					case ItemType.Potion:
						symbol = '!';
						color = ConsoleColor.Red;
						break;
				}
				Print(symbol, color);
			}
		}

		static void DrawPlayer(PlayerCharacter character)
		{
			Console.SetCursorPosition((int)character.position.X, (int)character.position.Y);
			Print("@", ConsoleColor.White);
		}

        static void DrawCommands()
        {
            int cx = Console.WindowWidth - COMMANDS_WIDTH + 1;
            int ln = 1;
            Console.SetCursorPosition(cx, ln); ln++;
            Print(":Commands:", ConsoleColor.Yellow);
            Console.SetCursorPosition(cx, ln); ln++;
            Print("I", ConsoleColor.Cyan); Print("nventory", ConsoleColor.White);
            Console.SetCursorPosition(cx, ln); ln++;
            Print("T", ConsoleColor.Cyan); Print("rader", ConsoleColor.White);
        }


        static void DrawInfo(PlayerCharacter player, List<Monster> enemies, List<Item> items, List<string> messages)
		{
			int infoLine1 = Console.WindowHeight - INFO_HEIGHT;
			Console.SetCursorPosition(0, infoLine1);
			Print($"{player.name}: hp ({player.hitpoints}/{player.maxHitpoints}) gold ({player.gold}) ", ConsoleColor.White);
			int damage = 1;
			if (player.weapon != null)
			{
				damage = player.weapon.quality;
			}
			Print($"Weapon damage: {damage} ");
			int armor = 0;
			if (player.armor != null)
			{
				armor = player.armor.quality;
			}
			Print($"Armor: {armor} ");



			// Print last INFO_HEIGHT -1 messages
			DrawRectangle(0, infoLine1 + 1, Console.WindowWidth, INFO_HEIGHT - 2, ConsoleColor.Black);
			Console.SetCursorPosition(0, infoLine1 + 1);
			int firstMessage = 0;
			if (messages.Count > (INFO_HEIGHT - 1))
			{
				firstMessage = messages.Count - (INFO_HEIGHT - 1);
			}
			for (int i = firstMessage; i < messages.Count; i++)
			{
				Print(messages[i], ConsoleColor.Yellow);
			}
		}

		static PlayerTurnResult DrawInventory(PlayerCharacter character, List<string> messages)
		{
			Console.SetCursorPosition(1, 1);
			PrintLine("Inventory. Select item by inputting the number next to it. Invalid input closes inventory");
			ItemType currentType = ItemType.Weapon;
			PrintLine("Weapons", ConsoleColor.DarkCyan);
			for (int i = 0; i < character.inventory.Count; i++)
			{
				Item it = character.inventory[i];
				if (currentType == ItemType.Weapon && it.type == ItemType.Armor)
				{
					currentType = ItemType.Armor;
					PrintLine("Armors", ConsoleColor.DarkRed);
				}
				else if (currentType == ItemType.Armor && it.type == ItemType.Potion)
				{
					currentType = ItemType.Potion;
					PrintLine("Potions", ConsoleColor.DarkMagenta);
				}
				Print($"{i} ", ConsoleColor.Cyan);
				PrintLine($"{it.name} ({it.quality})", ConsoleColor.White);
			}
			while (true)
			{
				Print("Choose item: ", ConsoleColor.Yellow);
				string choiceStr = Console.ReadLine();
				int selectionindex = 0;
				if (int.TryParse(choiceStr, out selectionindex))
				{
					if (selectionindex >= 0 && selectionindex < character.inventory.Count)
					{
                        Item.UseItem(character, character.inventory[selectionindex], messages);
                        break;
					}
				}
				else
				{
					messages.Add("No such item");
				}
			};
			return PlayerTurnResult.BackToGame;
		}



        static bool DoPlayerTurnVsEnemies(PlayerCharacter character, List<Monster> enemies, Vector2 destinationPlace, List<string> messages)
        {
            // Check enemies
            bool hitEnemy = false;
            Monster toRemoveMonster = null;
            foreach (Monster enemy in enemies)
            {
                if (enemy.position == destinationPlace)
                {
                    int damage = PlayerCharacter.GetCharacterDamage(character);
                    messages.Add($"You hit {enemy.name} for {damage} damage!");
                    enemy.hitpoints -= damage;
                    hitEnemy = true;

                    if (enemy.hitpoints > 0)
                    {
                        // Add a message showing the enemy's remaining health
                        messages.Add($"{enemy.name} has {enemy.hitpoints} HP left.");
                    }
                    else
                    {
                        toRemoveMonster = enemy;
                        messages.Add($"You killed {enemy.name}!");
                    }
                }
            }
            if (toRemoveMonster != null)
            {
                enemies.Remove(toRemoveMonster);
            }
            return hitEnemy;
        }

        static bool DoPlayerTurnVsItems(PlayerCharacter character, List<Item> items, Vector2 destinationPlace, List<string> messages)
		{
			// Check items
			Item toRemoveItem = null;
			foreach (Item item in items)
			{
				if (item.position == destinationPlace)
				{
					string itemMessage = $"You find a ";
					switch (item.type)
					{
						case ItemType.Armor:
							itemMessage += $"{item.name}, it fits you well";
							break;
						case ItemType.Weapon:
							itemMessage += $"{item.name} to use in battle";
							break;
						case ItemType.Potion:
							itemMessage += $"potion of {item.name}";
							break;
						case ItemType.Treasure:
							itemMessage += $"valuable {item.name} and get {item.quality} gold!";
							break;
					};
					messages.Add(itemMessage);
					toRemoveItem = item;
                    Item.GiveItem(character, item);
                    break;
				}
			}
			if (toRemoveItem != null)
			{
				items.Remove(toRemoveItem);
			}
			return false;
		}

		static PlayerTurnResult DoPlayerTurn(Map level, PlayerCharacter character, List<Monster> enemies, List<Item> items, List<int> dirtyTiles, List<string> messages)
		{
			Vector2 playerMove = new Vector2(0, 0);
			while (true)
			{
				ConsoleKeyInfo key = Console.ReadKey();
				if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow)
				{
					playerMove.Y = -1;
					break;
				}
				else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow)
				{
					playerMove.Y = 1;
					break;
				}
				else if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow)
				{
					playerMove.X = -1;
					break;
				}
				else if (key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow)
				{
					playerMove.X = 1;
					break;
				}
				// Other commands
				else if (key.Key == ConsoleKey.I)
				{
					return PlayerTurnResult.OpenInventory;
				}
                else if (key.Key == ConsoleKey.T)
                {
                    return PlayerTurnResult.OpenTrader;
                }

            }

            int startTile = Map.PositionToTileIndex(character.position, level);
			Vector2 destinationPlace = character.position + playerMove;

			if (DoPlayerTurnVsEnemies(character, enemies, destinationPlace, messages))
			{
				return PlayerTurnResult.TurnOver;
			}

			if (DoPlayerTurnVsItems(character, items, destinationPlace, messages))
			{
				return PlayerTurnResult.TurnOver;
			}

			// Check movement
			Map.Tile destination = Map.GetTileAtMap(level, destinationPlace);
			if (destination == Map.Tile.Floor)
			{
				character.position = destinationPlace;
				dirtyTiles.Add(startTile);
			}
			else if (destination == Map.Tile.Door)
			{
				messages.Add("You open a door");
				character.position = destinationPlace;
				dirtyTiles.Add(startTile);
			}
			else if (destination == Map.Tile.Wall)
			{
				messages.Add("You hit a wall");
			}
			else if (destination == Map.Tile.Stairs)
			{
				messages.Add("You find stairs leading down");
				return PlayerTurnResult.NextLevel;
			}

			return PlayerTurnResult.TurnOver;
		}

		

		public static int GetDistanceBetween(Vector2 A, Vector2 B)
		{
			return (int)Vector2.Distance(A, B);
		}
	}
}
