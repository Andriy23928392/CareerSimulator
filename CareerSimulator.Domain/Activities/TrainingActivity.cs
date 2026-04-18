using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using System;

namespace CareerSimulator.Domain.Activities
{
    public class TrainingActivity : IActivity
    {
        public string Name => "Інтенсивне тренування";
        public string Description => "Витрачає 30 енергії, але покращує форму.";

        public void Execute(Player player)
        {
            player.SpendEnergy(30);

            Console.WriteLine($"{player.Name} успішно провів тренування! Енергія зменшилась.");
        }
    }
}