using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using System;

namespace CareerSimulator.Domain.Activities
{
    public class MatchActivity : IActivity
    {
        public string Name => "Зіграти матч";
        public string Description => "Забирає 40 енергії. Можна заробити гроші та рейтинг.";

        private static readonly Random _random = new Random();

        public void Execute(Player player)
        {
            int currentEnergy = player.Energy;

            int matchCost = Math.Max(10, 40 - player.MatchDiscount);
            player.SpendEnergy(matchCost);

            int opponentRating = _random.Next(Math.Max(10, player.OverallRating - 15), player.OverallRating + 15);

            double energyFactor = 0.5 + 0.5 * (currentEnergy / 100.0);
            double effectivePower = player.OverallRating * energyFactor;

            double winChance = 50 + (effectivePower - opponentRating) * 1.5;
            winChance += player.WinChanceBonus;
            winChance = Math.Clamp(winChance, 5, 99);

            Console.WriteLine($"\nСуперник: Команда з рейтингом {opponentRating}");
            Console.WriteLine($"Ваша ефективна сила з урахуванням втоми: {Math.Round(effectivePower, 1)}");
            Console.WriteLine($"Ймовірність вашої перемоги: {Math.Round(winChance)}%");
            Console.WriteLine("Свисток... Матч починається!\n");

            int roll = _random.Next(1, 101);

            if (roll <= winChance)
            {
                int reward = _random.Next(150, 300);
                player.EarnMoney(reward);
                player.ChangeRating(1);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"ПЕРЕМОГА! Ви відіграли чудово в захисті. Зароблено: {reward}$, Рейтинг +1");
            }
            else if (roll <= winChance + 15)
            {
                int reward = _random.Next(30, 80);
                player.EarnMoney(reward);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"НІЧИЯ. Напружена гра без переможця. Зароблено: {reward}$");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ПОРАЗКА. Нападники суперника знищили ваш захист. Ви нічого не заробили.");

                if (_random.Next(1, 100) <= 15)
                {
                    player.ChangeRating(-1);
                    Console.WriteLine("Через жахливу гру ваш рейтинг впав на 1.");
                }
            }

            Console.ResetColor();
        }
    }
}