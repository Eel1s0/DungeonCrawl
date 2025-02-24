using System;

namespace DungeonCrawl
{
    internal class Map
    {
        public enum Tile : sbyte
        {
            Floor,
            Wall,
            Door,
            Monster,
            Item,
            Player,
            Stairs
        }
        public int width;
        public int height;
        public Tile[] Tiles;
    }
}