using System;
using System.Collections.Generic;
using System.Numerics;

namespace DungeonCrawl 
{
    internal class Quest
    {
        public string Description { get; set; }
        public int Goal { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
        public Action<PlayerCharacter> Reward { get; set; } // Reward logic

        public Quest(string description, int goal, Action<PlayerCharacter> reward)
        {
            Description = description;
            Goal = goal;
            Progress = 0;
            IsCompleted = false;
            Reward = reward;
        }

        public void UpdateProgress(int amount)
        {
            if (!IsCompleted)
            {
                Progress += amount;
                if (Progress >= Goal)
                {
                    IsCompleted = true;
                }
            }
        }

        public void GiveReward(PlayerCharacter player)
        {
            if (IsCompleted)
            {
                Reward(player);
            }
        }
    }

}
