using System;
using System.Collections.Generic;
using System.Numerics;

namespace DungeonCrawl
{
    internal class PlayerCharacter
    {
        public string name;
        public int hitpoints;
        public int maxHitpoints;
        public Item weapon;
        public Item armor;
        public int gold;
        public Vector2 position;
        public List<Item> inventory;

        public static PlayerCharacter CreateCharacter()
        {
            // Create a new PlayerCharacter instance to hold character data.
            PlayerCharacter character = new PlayerCharacter();

            // Initialize character properties.
            character.name = "";                  // Character name, to be set by user input.
            character.hitpoints = 50;             // Starting health points.
            character.maxHitpoints = character.hitpoints; // Maximum HP set to the same as starting HP.
            character.gold = 0;                  // Start with no gold.
            character.weapon = null;             // No weapon equipped initially.
            character.armor = null;              // No armor equipped initially.
            character.inventory = new List<Item>(); // Empty inventory for items.

            // Clear the console for a clean character creation screen.
            Console.Clear();
            Program.DrawBrickBg(); // Draws a decorative brick background.

            // Draw entrance-like door on the screen for aesthetic purposes.
            Console.BackgroundColor = ConsoleColor.Black; // Background color for the door.

            // Calculate the size and position of the door.
            int doorHeight = (int)(Console.WindowHeight * (3.0f / 4.0f)); // Door height is 75% of window height.
            int doorY = Console.WindowHeight - doorHeight;                // Door starts from the bottom up.
            int doorWidth = (int)(Console.WindowWidth * (3.0f / 5.0f));   // Door width is 60% of window width.
            int doorX = Console.WindowWidth / 2 - doorWidth / 2;          // Center the door horizontally.

            // Draw the door background and its decorative borders.
            Program.DrawRectangle(doorX, doorY, doorWidth, doorHeight, ConsoleColor.Black);        // Main door area.
            Program.DrawRectangleBorders(doorX + 1, doorY + 1, doorWidth - 2, doorHeight - 2, ConsoleColor.Blue, "|"); // Outer border.
            Program.DrawRectangleBorders(doorX + 3, doorY + 3, doorWidth - 6, doorHeight - 6, ConsoleColor.DarkBlue, "|"); // Inner border.

            // Display the character creation prompt.
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2); // Center the welcome message.
            Program.Print("Welcome Brave Adventurer!", ConsoleColor.Yellow); // Greet the player.

            // Prompt for character name.
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2 + 1);
            Program.Print("What is your name?", ConsoleColor.Yellow);

            // Keep asking for the name until a non-empty input is provided.
            while (string.IsNullOrEmpty(character.name))
            {
                character.name = Console.ReadLine(); // Read user input for the name.
            }

            // Greet the player by their chosen name.
            Program.Print($"Welcome {character.name}!", ConsoleColor.Yellow);

            // Return the fully initialized character.
            return character;
        }

        public static int GetCharacterDamage(PlayerCharacter character)
        {
            if (character.weapon != null)
            {
                return character.weapon.quality;
            }
            else
            {
                return 1;
            }
        }

        public static int GetCharacterDefense(PlayerCharacter character)
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
    }
}
