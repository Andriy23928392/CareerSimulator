using System;
using CareerSimulator.Domain.Models;


namespace CareerSimulator.Domain.Services
{
    public static class MedicalCenter
    {
        private static Random _random = new Random();

        public static void CheckForInjury(Player player, bool isMatch)
        {
            if (player.IsInjured) return;

            // Шанс травмуватись вищий у матчі, ніж на тренуванні
            int baseChance = isMatch ? 5 : 2; 

            if (_random.Next(1, 101) <= baseChance)
            {
                int severityRoll = _random.Next(1, 101);
                string name;
                int weeks;
                int moraleDrop;

                // Розподіл тяжкості травм
                if (severityRoll <= 60) 
                {
                    // Легка травма (60%)
                    name = "Забій гомілкостопу";
                    weeks = _random.Next(1, 3);
                    moraleDrop = 5;
                }
                else if (severityRoll <= 85) 
                {
                    // Середня травма (25%)
                    name = "Надрив м'язів стегна";
                    weeks = _random.Next(3, 7);
                    moraleDrop = 15;
                }
                else if (severityRoll <= 97) 
                {
                    // Важка травма (12%)
                    name = "Перелом плеснової кістки";
                    weeks = _random.Next(8, 15);
                    moraleDrop = 35;
                }
                else 
                {
                    // КАТАСТРОФА: "Хрести" (3%)
                    name = "Розрив хрестоподібних зв'язок";
                    weeks = _random.Next(24, 36); // Близько 6-8 місяців без футболу
                    moraleDrop = 70; // Миттєва депресія
                }

                player.SufferInjury(name, weeks);
                player.ChangeMorale(-moraleDrop);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[МЕДИЧНИЙ ЦЕНТР] ЖАХЛИВІ НОВИНИ!");
                Console.WriteLine($"Ви отримали серйозну травму: {name}.");
                Console.WriteLine($"Лікарі прогнозують, що ви пропустите {weeks} тижнів.");
                Console.WriteLine($"Психологічний удар: Мораль впала на {moraleDrop} пунктів.");
                Console.ResetColor();
            }
        }
    }
}