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
    }
}
