using System;
using System.Numerics;
using System.Collections.Generic;

namespace DungeonCrawl
{
    internal class Item
    {
        public string name;
        public int quality;
        public Vector2 position;
        public ItemType type;

        public static Item CreateItem(string name, ItemType type, int quality, Vector2 position)
        {
            Item i = new Item();
            i.name = name;
            i.type = type;
            i.quality = quality;
            i.position = position;
            return i;
        }

        public static Item CreateRandomItem(Random random, Vector2 position)
        {
            ItemType type = Enum.GetValues<ItemType>()[random.Next(4)];
            Item i = type switch
            {
                ItemType.Treasure => CreateItem("Book", type, 2, position),
                ItemType.Weapon => CreateItem("Sword", type, 5, position),
                ItemType.Armor => CreateItem("Helmet", type, 1, position),
                ItemType.Potion => CreateItem("Apple Juice", type, 1, position)
            };
            return i;
        }

        public static void GiveItem(PlayerCharacter character, Item item)
        {
            switch (item.type)
            {
                case ItemType.Weapon:
                    if ((character.weapon != null && character.weapon.quality < item.quality)
                        || character.weapon == null)
                    {
                        character.weapon = item;
                    }
                    character.inventory.Insert(0, item);
                    break;
                case ItemType.Armor:
                    if ((character.armor != null && character.armor.quality < item.quality)
                        || character.armor == null)
                    {
                        character.armor = item;
                    }
                    int armorIndex = 0;
                    while (armorIndex < character.inventory.Count && character.inventory[armorIndex].type == ItemType.Weapon)
                    {
                        armorIndex++;
                    }
                    character.inventory.Insert(armorIndex, item);
                    break;
                case ItemType.Potion:
                    character.inventory.Add(item);
                    break;
                case ItemType.Treasure:
                    character.gold += item.quality;
                    break;
            }
        }

        public static void UseItem(PlayerCharacter character, Item item, List<string> messages)
        {
            switch (item.type)
            {
                case ItemType.Weapon:
                    character.weapon = item;
                    messages.Add($"You are now wielding a {item.name}");
                    break;
                case ItemType.Armor:
                    character.armor = item;
                    messages.Add($"You equip {item.name} on yourself.");
                    break;
                case ItemType.Potion:
                    character.hitpoints += item.quality;
                    if (character.hitpoints > character.maxHitpoints)
                    {
                        character.maxHitpoints = character.hitpoints;
                    }
                    messages.Add($"You drink a potion and gain {item.quality} hitpoints");
                    character.inventory.Remove(item);
                    break;
            }
        }
    }
}
