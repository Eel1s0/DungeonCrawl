using System;
using System.Collections.Generic;
using System.Numerics;

namespace DungeonCrawl
{
    internal class Trader
    {
        public const int MaxInventorySize = 10; // Maximum number of items the trader can hold
        public List<Item> Inventory { get; private set; }

        public Trader()
        {
            Inventory = new List<Item>();
        }

        public void RestockInventory(Random random, int level)
        {
            // Only restock if there's room in the inventory
            int availableSlots = MaxInventorySize - Inventory.Count;
            if (availableSlots <= 0)
            {
                return; // Inventory is full, no restocking
            }

            int numberOfItems = random.Next(1, Math.Min(availableSlots, 6)); // Add up to the available slots

            for (int i = 0; i < numberOfItems; i++)
            {
                Item item = Item.CreateRandomItem(random, new Vector2(0, 0));
                item.quality += level; // Increase item quality based on the level
                Inventory.Add(item);
            }
        }

        public void DisplayInventory()
        {
            Console.WriteLine("Trader's Inventory:");
            for (int i = 0; i < Inventory.Count; i++)
            {
                Item item = Inventory[i];
                Console.WriteLine($"{i + 1}. {item.name} ({item.type}) - {item.quality} gold");
            }
        }

        public bool BuyItem(PlayerCharacter player, int itemIndex, List<string> messages)
        {
            if (itemIndex < 0 || itemIndex >= Inventory.Count)
            {
                messages.Add("Invalid item selection.");
                return false;
            }

            Item item = Inventory[itemIndex];
            if (player.gold >= item.quality)
            {
                player.gold -= item.quality;
                Item.GiveItem(player, item);
                messages.Add($"You bought {item.name} for {item.quality} gold.");
                Inventory.RemoveAt(itemIndex);
                return true;
            }
            else
            {
                messages.Add("You don't have enough gold to buy this item.");
                return false;
            }
        }

        public bool SellItem(PlayerCharacter player, int itemIndex, List<string> messages)
        {
            if (itemIndex < 0 || itemIndex >= player.inventory.Count)
            {
                messages.Add("Invalid item selection.");
                return false;
            }

            if (Inventory.Count >= MaxInventorySize)
            {
                messages.Add("The trader's inventory is full. You cannot sell this item.");
                return false;
            }

            Item item = player.inventory[itemIndex];
            int sellPrice = item.quality / 2; // Trader buys items for half their quality value
            player.gold += sellPrice;
            Inventory.Add(item); // Add the item to the trader's inventory
            player.inventory.RemoveAt(itemIndex); // Remove the item from the player's inventory
            messages.Add($"You sold {item.name} for {sellPrice} gold.");
            return true;
        }
    }
}
