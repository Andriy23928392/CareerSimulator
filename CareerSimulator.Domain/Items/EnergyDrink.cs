using CareerSimulator.Domain.Models;
using System;

namespace CareerSimulator.Domain.Items
{
    public class EnergyDrink : Item
    {
        public EnergyDrink()
        {
            Name = "Спортивний енергетик (+30 енергії)";
            Price = 50m;
        }

        public override void Apply(Player player)
        {
            player.RestoreEnergy(30);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nВи випили енергетик. Енергія збільшилась!");
            Console.ResetColor();
        }
    }
}