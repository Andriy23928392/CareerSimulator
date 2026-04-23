using System;

namespace CareerSimulator.Domain.Models
{
    public class PlayerAttributes
    {
        private static Random _random = new Random();

        public int Pace = 50;
        public int Shooting = 50;
        public int Passing = 50;
        public int Dribbling = 50;
        public int Defending = 50;
        public int Physical = 50;

        public int GK_Diving = 50;
        public int GK_Handling = 50;
        public int GK_Kicking = 50;
        public int GK_Reflexes = 50;
        public int GK_Speed = 50;
        public int GK_Positioning = 50;

        public bool TryImprove(ref int attributeValue, int maxLimit = 99)
        {
            if (attributeValue >= maxLimit) return false;

            int successChance;

            if (attributeValue < 70) successChance = 80;
            else if (attributeValue < 80) successChance = 60;
            else if (attributeValue < 85) successChance = 40;
            else if (attributeValue < 90) successChance = 30;
            else if (attributeValue < 100) successChance = 15;
            else successChance = 0;

            if (_random.Next(1, 101) <= successChance)
            {
                attributeValue += 1;
                return true;
            }
            return false;
        }
    }
}