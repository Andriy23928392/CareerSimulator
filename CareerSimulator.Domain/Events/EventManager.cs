using CareerSimulator.Domain.Models;
using System;

namespace CareerSimulator.Domain.Events
{
    public static class EventManager
    {
        private static readonly Random _random = new Random();

        public static void TriggerRandomEvent(Player player)
        {
            if (_random.Next(1, 101) > 30) return;

            int eventType = _random.Next(1, 5);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n[РАПТОВА ПОДІЯ!]");

            switch (eventType)
            {
                case 1:
                    Console.WriteLine("Ви підписали рекламний контракт із місцевим брендом! (+150$)");
                    player.EarnMoney(150);
                    break;
                case 2:
                    Console.WriteLine("Ви з'їли несвіжу шаурму біля стадіону. Легке отруєння: -20 енергії.");
                    player.DecreaseEnergy(20);
                    break;
                case 3:
                    Console.WriteLine("Тренер похвалив вас перед усією командою. Мораль на висоті! (+30 енергії)");
                    player.RestoreEnergy(30);
                    break;
                case 4:
                    Console.WriteLine("Ви дали невдале інтерв'ю журналістам. Вболівальники розчаровані. (-1 до рейтингу)");
                    player.ChangeRating(-1);
                    break;
            }
            Console.ResetColor();
        }
    }
}