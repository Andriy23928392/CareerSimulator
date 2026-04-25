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
            Console.WriteLine($"Поточний клуб: {player.CurrentClub.Name}");
            Console.WriteLine($"Зарплата: {player.CurrentClub.WeeklySalary}$ | Баланс: {player.Money}$");

            Console.WriteLine("\n--------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ЗАГАЛЬНИЙ РЕЙТИНГ (OVR): {player.OverallRating}");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            Console.WriteLine("\nХАРАКТЕРИСТИКИ:");
            if (player.PlayerPosition == Position.Goalkeeper)
            {
                Console.WriteLine($"DIV (Стрибки):   {player.Attributes.GK_Diving,-3} | REF (Рефлекси): {player.Attributes.GK_Reflexes}");
                Console.WriteLine($"HAN (Руки):      {player.Attributes.GK_Handling,-3} | POS (Позиція):  {player.Attributes.GK_Positioning}");
                Console.WriteLine($"KIC (Вибивання): {player.Attributes.GK_Kicking,-3} | SPD (Швидкість): {player.Attributes.GK_Speed}");
            }
            else
            {
                Console.WriteLine($"PAC (Швидкість): {player.Attributes.Pace,-3} | DRI (Дриблінг): {player.Attributes.Dribbling}");
                Console.WriteLine($"SHO (Удари):     {player.Attributes.Shooting,-3} | DEF (Захист):   {player.Attributes.Defending}");
                Console.WriteLine($"PAS (Паси):      {player.Attributes.Passing,-3} | PHY (Фізика):   {player.Attributes.Physical}");
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

            Console.Write($"Довіра тренера: {player.CoachTrust}/100 ");
            if (player.CoachTrust < 30)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[КРИТИЧНО: Вас можуть вигнати!]");
            }
            else if (player.CoachTrust >= 80)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[УЛЮБЛЕНЕЦЬ: Ви лідер команди]");
            }
            else
            {
                Console.WriteLine("[СТАБІЛЬНО]");
            }
            Console.ResetColor();

            if (player.IsInjured)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ТРАВМА] {player.InjuryName} (Залишилось: {player.WeeksInjured} тижнів)");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[ЗДОРОВ'Я] Гравець у відмінній формі.");
                Console.ResetColor();
            }

            Console.WriteLine("\n---------------- СТАТИСТИКА ----------------------");
            Console.WriteLine($"Зіграно матчів: {player.Stats.TotalMatches}");
            Console.WriteLine($"Голи: {player.Stats.TotalGoals} | Асисти: {player.Stats.TotalAssists}");
            if (player.PlayerPosition == Position.Goalkeeper || player.PlayerPosition == Position.Defender)
            {
                Console.WriteLine($"Сухі матчі: {player.Stats.TotalCleanSheets}");
            }

            Console.WriteLine("\n---------------- МОДИФІКАТОРИ --------------------");
            Console.WriteLine($"Макс. Енергія: {player.MaxEnergy} | Слава: {player.Reputation}");
            if (player.WinChanceBonus > 0) Console.WriteLine($"[+] Менталітет: +{player.WinChanceBonus}% до шансу перемоги");

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}