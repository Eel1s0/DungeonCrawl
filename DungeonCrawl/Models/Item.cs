using System;
using System.Numerics;

namespace DungeonCrawl
{
    internal class Item
    {
        public string name;
        public int quality; // means different things depending on the type
        public Vector2 position;
        public ItemType type;
    }
}