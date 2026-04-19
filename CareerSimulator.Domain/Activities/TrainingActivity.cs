using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using System;

namespace CareerSimulator.Domain.Activities
{
    public class TrainingActivity : IActivity
    {
        public string Name => "Інтенсивне тренування";
        public string Description => "Витрачає 30 енергії, але покращує форму.";

        private static readonly Random _random = new Random();
        public void Execute(Player player)
        {
            player.SpendEnergy(30);

            Console.WriteLine($"{player.Name} успішно провів тренування! Енергія зменшилась.");
            if (_random.Next(1, 101) <= 25)
            {
                player.ChangeRating(1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" Ваша форма покращилась: Рейтинг +1!");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

    }
}