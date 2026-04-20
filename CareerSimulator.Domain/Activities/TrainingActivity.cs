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
            if (player.TrainingsThisWeek >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[!] Ліміт вичерпано! Тренер забороняє тренуватися більше 5 разів на тиждень через ризик травм. Йдіть на Відпочинок.");
                Console.ResetColor();
                return;
            }
            int trainingCost = Math.Max(5, 30 - player.TrainingDiscount);
            try { player.SpendEnergy(trainingCost); }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }
            player.AddTrainingThisWeek();

            Console.Write($"\nВи успішно провели інтенсивне тренування (-{trainingCost} енергії).");

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