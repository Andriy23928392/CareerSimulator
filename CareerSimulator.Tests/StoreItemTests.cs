using System;
using Xunit;
using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Items;

namespace CareerSimulator.Tests
{
    public class StoreItemTests
    {
        [Fact]
        public void ProteinShake_ShouldRestore50Energy()
        {
            var player = new Player("Мудрик", Position.Forward);
            player.DecreaseEnergy(70); 
            var shake = new ProteinShake();

            shake.Apply(player);

            Assert.Equal(80, player.Energy); 
        }

        [Fact]
        public void PsychologistSession_ShouldThrowException_WhenMoraleIsMax()
        {
            var player = new Player("Зінченко", Position.Midfielder);
            player.Morale = 100;
            var session = new PsychologistSession();

            var exception = Assert.Throws<Exception>(() => session.Apply(player));
            Assert.Contains("психолог не потрібен", exception.Message);
        }

        [Fact]
        public void LuxuryVilla_ShouldIncreaseMaxEnergy()
        {
            var player = new Player("Довбик", Position.Forward);
            int initialMaxEnergy = player.MaxEnergy;
            var villa = new LuxuryVilla();

            villa.Apply(player);

            Assert.Equal(initialMaxEnergy + 20, player.MaxEnergy);
        }
    }
}