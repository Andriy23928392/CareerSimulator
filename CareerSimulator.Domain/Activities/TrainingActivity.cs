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
            int energyCost = Math.Max(5, 30 - player.TrainingDiscount);
            player.SpendEnergy(energyCost);

            Console.Write($"\nВи успішно провели інтенсивне тренування (-{energyCost} енергії).");

            int totalChance = 30 + player.TrainingChanceBonus;

            if (_random.Next(1, 101) <= totalChance)
            {
                player.ChangeRating(1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" Форма покращилась (Шанс був {totalChance}%): Рейтинг +1!");
                Console.ResetColor();
            }
            Console.WriteLine();
        }

    }
}