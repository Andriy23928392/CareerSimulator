using System;
using System.Linq;
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
            Console.WriteLine("                 ПРОФІЛЬ ГРАВЦЯ");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            Console.WriteLine($"Ім'я: {player.Name}");
            Console.WriteLine($"Національність: {player.Nationality}");
            Console.WriteLine($"Вік: {player.Age} років (Пенсія у {player.Stats.RetirementAge})");
            Console.WriteLine($"Позиція: {player.PlayerPosition}");
            Console.WriteLine($"Поточний клуб: {player.CurrentClub.Name}");
            Console.WriteLine($"Зарплата: {player.CurrentClub.WeeklySalary}$ | Баланс: {player.Money}$");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"      ЗАГАЛЬНИЙ РЕЙТИНГ (OVR): {player.OverallRating}");
            Console.WriteLine("--------------------------------------------------");
            Console.ResetColor();

            Console.WriteLine("\nХАРАКТЕРИСТИКИ:");
            if (player.PlayerPosition == Position.Goalkeeper)
            {
                Console.WriteLine($"DIV (Стрибучість): {player.Attributes.GK_Diving,-3} | REF (Рефлекси): {player.Attributes.GK_Reflexes,-3}");
                Console.WriteLine($"HAN (Гра руками):  {player.Attributes.GK_Handling,-3} | SPD (Швидкість):  {player.Attributes.GK_Speed,-3}");
                Console.WriteLine($"KIC (Удари від воріт): {player.Attributes.GK_Kicking,-3} | POS (Вибір позиції): {player.Attributes.GK_Positioning,-3}");
            }
            else
            {
                Console.WriteLine($"PAC (Швидкість): {player.Attributes.Pace,-3} | DRI (Дриблінг): {player.Attributes.Dribbling,-3}");
                Console.WriteLine($"SHO (Удари):     {player.Attributes.Shooting,-3} | DEF (Захист):   {player.Attributes.Defending,-3}");
                Console.WriteLine($"PAS (Паси):      {player.Attributes.Passing,-3} | PHY (Фізика):   {player.Attributes.Physical,-3}");
            }

            Console.WriteLine("\n----------------- ПСИХОЛОГІЯ ---------------------");
            Console.WriteLine($"Мораль: {player.Morale}/100");
            Console.WriteLine($"Довіра тренера: {player.CoachTrust}/100");

            if (player.IsInjured)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ЗДОРОВ'Я] Травма: {player.InjuryName} ({player.WeeksInjured} тижнів)");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[ЗДОРОВ'Я] Гравець у відмінній формі.");
                Console.ResetColor();
            }

            Console.WriteLine("\n----------------- СТАТИСТИКА ---------------------");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[ПОТОЧНИЙ СЕЗОН]");
            Console.ResetColor();
            Console.WriteLine($"Матчів у лізі: {player.Stats.SeasonLeagueMatches}");
            Console.WriteLine($"Голи: {player.Stats.SeasonGoals} | Асисти: {player.Stats.SeasonAssists}");
            Console.WriteLine($"Сухі матчі: {player.Stats.SeasonCleanSheets}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[ЗА ВСЮ КАР'ЄРУ]");
            Console.ResetColor();
            Console.WriteLine($"Зіграно матчів: {player.Stats.TotalMatches}");
            Console.WriteLine($"Голи: {player.Stats.TotalGoals} | Асисти: {player.Stats.TotalAssists}");
            Console.WriteLine($"Сухі матчі: {player.Stats.TotalCleanSheets}");

            if (player.Stats.BallonDorAwards > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Золоті м'ячі: {player.Stats.BallonDorAwards} 🏆");
                Console.ResetColor();
            }

            Console.WriteLine("\n---------------- КІМНАТА ТРОФЕЇВ -----------------");
            if (player.Stats.Trophies == null || player.Stats.Trophies.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Поки що порожньо. Час вигравати кубки!");
                Console.ResetColor();
            }
            else
            {
                int clCount = player.Stats.Trophies.Count(t => t.Contains("Ліга Чемпіонів"));
                int elCount = player.Stats.Trophies.Count(t => t.Contains("Ліга Європи"));
                int wcCount = player.Stats.Trophies.Count(t => t.Contains("Світу"));
                int euroCount = player.Stats.Trophies.Count(t => t.Contains("Євро"));
                int nlCount = player.Stats.Trophies.Count(t => t.Contains("Ліга Націй"));
                int leagueCount = player.Stats.Trophies.Count(t => t.Contains("Чемпіон Ліги"));
                int cupCount = player.Stats.Trophies.Count(t => t.Contains("Національний Кубок"));

                Console.ForegroundColor = ConsoleColor.Yellow;
                if (wcCount > 0) Console.WriteLine($"🌍 Чемпіонат Світу: {wcCount} шт.");
                if (euroCount > 0) Console.WriteLine($"🇪🇺 Чемпіонат Європи: {euroCount} шт.");
                if (nlCount > 0) Console.WriteLine($"🌐 Ліга Націй: {nlCount} шт.");
                if (clCount > 0) Console.WriteLine($"⭐ Ліга Чемпіонів: {clCount} шт.");
                if (elCount > 0) Console.WriteLine($"🟠 Ліга Європи: {elCount} шт.");
                if (leagueCount > 0) Console.WriteLine($"🥇 Національні Ліги: {leagueCount} шт.");
                if (cupCount > 0) Console.WriteLine($"🏆 Національні Кубки: {cupCount} шт.");
                Console.ResetColor();
            }

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Макс. Енергія: {player.MaxEnergy} | Слава: {player.Reputation}");
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}