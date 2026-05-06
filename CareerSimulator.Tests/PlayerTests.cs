using System;
using Xunit;
using CareerSimulator.Domain.Models;

namespace CareerSimulator.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void SpendEnergy_ShouldDecreaseEnergy_WhenEnoughEnergy()
        {
            var player = new Player("Шевченко", Position.Forward); 
            int initialEnergy = player.Energy;
            int cost = 30;

            player.SpendEnergy(cost);

            Assert.Equal(initialEnergy - cost, player.Energy);
        }

        [Fact]
        public void SpendEnergy_ShouldThrowException_WhenNotEnoughEnergy()
        {
            var player = new Player("Мудрик", Position.Forward);
            player.DecreaseEnergy(90); 

            var exception = Assert.Throws<Exception>(() => player.SpendEnergy(30));
            Assert.Contains("Недостатньо енергії", exception.Message);
        }

        [Fact]
        public void RestoreEnergy_ShouldNotExceedMaxEnergy()
        {
            var player = new Player("Зінченко", Position.Midfielder);
            player.DecreaseEnergy(50);

            player.RestoreEnergy(200); 

            Assert.Equal(player.MaxEnergy, player.Energy);
        }

        [Fact]
        public void RecordMatchStats_ShouldIncrementTotalAndSeasonStats()
        {
            var player = new Player("Довбик", Position.Forward);

            player.Stats.RecordMatchStats(goals: 2, assists: 1, cleanSheets: 0, penaltiesSaved: 0);

            Assert.Equal(1, player.Stats.TotalMatches);
            Assert.Equal(2, player.Stats.TotalGoals);
            Assert.Equal(1, player.Stats.TotalAssists);

            Assert.Equal(1, player.Stats.SeasonMatches);
            Assert.Equal(2, player.Stats.SeasonGoals);
        }

        [Fact]
        public void CalculateEnergyCost_ShouldIncreaseCost_WhenMoraleIsLow()
        {
            var player = new Player("Забарний", Position.Defender);
            player.Morale = 20;
            int baseCost = 20;

            int actualCost = player.CalculateEnergyCost(baseCost);

            Assert.Equal(30, actualCost);
        }
    }
}