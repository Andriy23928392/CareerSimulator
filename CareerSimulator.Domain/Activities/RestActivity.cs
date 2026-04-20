using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using System;

namespace CareerSimulator.Domain.Activities
{
    public class RestActivity : IActivity
    {
        public string Name => "Відпочинок вдома";
        public string Description => "Відновлює енергію до максимуму.";

        public void Execute(Player player)
        {
            player.RestoreEnergy(player.MaxEnergy);
            Console.WriteLine($"\nВи добре відпочили вдома. Енергія повністю відновлена (до {player.MaxEnergy})!");
        }
    }
}