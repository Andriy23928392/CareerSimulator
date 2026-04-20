using System;
using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Core
{
    public static class MedicalCenter
    {
        private static Random _random = new Random();

        public static void CheckForInjury(Player player, bool isMatch)
        {
            if (player.IsInjured) return;

            int chance = isMatch ? 4 : 2;

            if (_random.Next(1, 101) <= chance)
            {
                int severityRoll = _random.Next(1, 101);

                if (severityRoll <= 70)
                {
                    player.SufferInjury("Перенавантаження м'язів", 2);
                }
                else if (severityRoll <= 95)
                {
                    player.SufferInjury("Травма коліна", _random.Next(8, 13));
                }
                else
                {
                    player.SufferInjury("Розрив хрестоподібних зв'язок", _random.Next(28, 41));
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n==================================================");
                Console.WriteLine($"🚑 ЖАХЛИВІ НОВИНИ! Ви отримали травму: {player.InjuryName}!");
                Console.WriteLine($"Термін відновлення: {player.WeeksInjured} тижнів.");
                Console.WriteLine("==================================================");
                Console.ResetColor();
            }
        }
    }
}