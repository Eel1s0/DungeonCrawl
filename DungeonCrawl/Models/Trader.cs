using System;
using System.Collections.Generic;
using System.Numerics;

namespace DungeonCrawl
{
    internal class Trader
    {
        public List<Item> Inventory { get; private set; }

        public Trader()
        {
            Inventory = new List<Item>();
        }

        public void RestockInventory(Random random, int level)
        {
            Inventory.Clear();
            int numberOfItems = random.Next(3, 6);

            for (int i = 0; i < numberOfItems; i++)
            {
                Item item = Item.CreateRandomItem(random, new Vector2(0, 0));
                item.quality += level; // Increase item quality based on the level
                Inventory.Add(item);
            }
        }


        public void DisplayInventory()
        {
            Console.WriteLine("Welcome to the Trader! Here are the items available for purchase:");
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
    }
}
