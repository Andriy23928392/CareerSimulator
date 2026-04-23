using System;
using CareerSimulator.Domain.Models;

namespace CareerSimulator.UI.Menus
{
    public static class ProfileMenu
    {
        public static void OpenProfile(Player player)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("               ПРОФІЛЬ ГРАВЦЯ                     ");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            Console.WriteLine($"Ім'я: {player.Name}");
            Console.WriteLine($"Вік: {player.Age} років (Пенсія очікується у {player.Stats.RetirementAge})");
            Console.WriteLine($"Позиція: {player.PlayerPosition}");
            Console.WriteLine($"Поточний клуб: {player.CurrentClub.Name} (Зарплата: {player.CurrentClub.WeeklySalary}$)");
            Console.WriteLine($"Рейтинг: {player.OverallRating} | Слава: {player.Reputation}");
            Console.WriteLine($"Баланс: {player.Money}$");

            if (player.IsInjured)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ТРАВМА] {player.InjuryName} (Залишилось лікуватися: {player.WeeksInjured} тижнів)");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[ЗДОРОВ'Я] Гравець повністю здоровий.");
                Console.ResetColor();
            }

            Console.WriteLine("\n---------------- ПСИХОЛОГІЯ ----------------------");
            Console.Write($"Мораль: {player.Morale}/100 ");
            if (player.Morale >= 80)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[КУРАЖ: +10% до перемоги]");
            }
            else if (player.Morale < 30)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ДЕПРЕСІЯ: -10% до перемоги, енергія падає швидше!]");
            }
            else
            {
                Console.WriteLine("[СТАБІЛЬНО]");
            }
            Console.ResetColor();

            Console.WriteLine("\n---------------- СТАТИСТИКА ----------------------");
            Console.WriteLine($"Зіграно матчів: {player.Stats.TotalMatches}");

            if (player.PlayerPosition == Position.Goalkeeper)
            {
                Console.WriteLine($"Сухі матчі (Кліншити): {player.Stats.TotalCleanSheets}");
                Console.WriteLine($"Відбиті пенальті: {player.Stats.TotalPenaltiesSaved}");
            }
            else if (player.PlayerPosition == Position.Defender)
            {
                Console.WriteLine($"Голи: {player.Stats.TotalGoals}");
                Console.WriteLine($"Асисти: {player.Stats.TotalAssists}");
                Console.WriteLine($"Сухі матчі: {player.Stats.TotalCleanSheets}");
            }
            else
            {
                Console.WriteLine($"Голи: {player.Stats.TotalGoals}");
                Console.WriteLine($"Асисти: {player.Stats.TotalAssists}");
            }

            Console.WriteLine("\n---------------- АКТИВНІ БОНУСИ ------------------");
            Console.WriteLine($"Макс. Енергія: {player.MaxEnergy}");
            Console.WriteLine($"Шанс до рейтингу (Тренування): +{player.TrainingChanceBonus}%");
            Console.WriteLine($"Шанс на перемогу (Матч): +{player.WinChanceBonus}%");

            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися...");
            Console.ReadKey();
        }
    }
}