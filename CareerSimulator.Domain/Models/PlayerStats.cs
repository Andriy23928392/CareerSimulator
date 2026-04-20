using System;

namespace CareerSimulator.Domain.Models
{
    public class PlayerStats
    {
        public int TotalMatches { get; private set; } = 0;
        public int RetirementAge { get; private set; }

        public PlayerStats()
        {
            RetirementAge = new Random().Next(38, 42);
        }

        public void AddMatchPlayed()
        {
            TotalMatches++;
        }

        public void LoadStats(int matches, int retirementAge)
        {
            TotalMatches = matches;
            RetirementAge = retirementAge;
        }
    }
}