using System;
using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Activities
{
    public class TrainingActivity : IActivity
    {
        public string Name => "Тренувальна база";
        public string Description => "Цілеспрямовано покращити конкретні характеристики (витрачає енергію)";

        public void Execute(Player player)
        {
            if (player.TrainingsThisWeek >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[!] Ліміт тренувань! Ви вже виснажені. Потрібен Відпочинок.");
                Console.ResetColor();
                return;
            }

            int energyCost = Math.Max(5, 30 - player.TrainingDiscount);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"=== ТРЕНУВАЛЬНА БАЗА: {player.CurrentClub.Name} ===");
            Console.ResetColor();
            Console.WriteLine($"Енергія: {player.Energy}/{player.MaxEnergy} | Залишилось тренувань: {5 - player.TrainingsThisWeek}");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Над чим будете працювати сьогодні?\n");

            bool isGk = player.PlayerPosition == Position.Goalkeeper;

            if (isGk)
            {
                Console.WriteLine($"1. Стрибки та Рефлекси (Поточні: {player.Attributes.GK_Diving} / {player.Attributes.GK_Reflexes})");
                Console.WriteLine($"2. Гра на виходах та Позиція (Поточні: {player.Attributes.GK_Speed} / {player.Attributes.GK_Positioning})");
                Console.WriteLine($"3. Робота з м'ячем (Поточні: {player.Attributes.GK_Handling} / {player.Attributes.GK_Kicking})");
            }
            else
            {
                Console.WriteLine($"1. Бігова робота та Фізика (Поточні: Швидк. {player.Attributes.Pace} / Фізика {player.Attributes.Physical})");
                Console.WriteLine($"2. Відпрацювання ударів (Поточні: Удари {player.Attributes.Shooting})");
                Console.WriteLine($"3. Контроль м'яча та Паси (Поточні: Дриб. {player.Attributes.Dribbling} / Паси {player.Attributes.Passing})");
                Console.WriteLine($"4. Тактика та Відбір (Поточні: Захист {player.Attributes.Defending})");
            }
            Console.WriteLine("0. Повернутися назад (не витрачати енергію)");
            Console.Write("\nВаш вибір: ");

            string choice = Console.ReadLine() ?? "0";
            if (choice == "0") return;

            try { player.SpendEnergy(energyCost); }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Помилка] {ex.Message}");
                Console.ResetColor();
                return;
            }

            player.AddTrainingThisWeek();
            bool s1 = false, s2 = false;
            string trainedStat = "";

            if (isGk)
            {
                if (choice == "1") { s1 = player.Attributes.TryImprove(ref player.Attributes.GK_Diving); s2 = player.Attributes.TryImprove(ref player.Attributes.GK_Reflexes); trainedStat = "Стрибки та Рефлекси"; }
                else if (choice == "2") { s1 = player.Attributes.TryImprove(ref player.Attributes.GK_Speed); s2 = player.Attributes.TryImprove(ref player.Attributes.GK_Positioning); trainedStat = "Швидкість та Позицію"; }
                else if (choice == "3") { s1 = player.Attributes.TryImprove(ref player.Attributes.GK_Handling); s2 = player.Attributes.TryImprove(ref player.Attributes.GK_Kicking); trainedStat = "Гру руками та Вибивання"; }
                else { player.RestoreEnergy(energyCost); return; }
            }
            else
            {
                if (choice == "1") { s1 = player.Attributes.TryImprove(ref player.Attributes.Pace); s2 = player.Attributes.TryImprove(ref player.Attributes.Physical); trainedStat = "Швидкість та Фізику"; }
                else if (choice == "2") { s1 = player.Attributes.TryImprove(ref player.Attributes.Shooting); trainedStat = "Удари"; }
                else if (choice == "3") { s1 = player.Attributes.TryImprove(ref player.Attributes.Dribbling); s2 = player.Attributes.TryImprove(ref player.Attributes.Passing); trainedStat = "Дриблінг та Паси"; }
                else if (choice == "4")
                {
                    int defLimit = player.PlayerPosition == Position.Forward ? 50 : (player.PlayerPosition == Position.Midfielder ? 70 : 99);

                    s1 = player.Attributes.TryImprove(ref player.Attributes.Defending, defLimit);
                    trainedStat = "Захист";

                    if (player.Attributes.Defending >= defLimit)
                    {
                        Console.WriteLine($"\n[!] Ваша навичка захисту досягла максимуму для цієї позиції ({defLimit}).");
                    }
                }
                else { player.RestoreEnergy(energyCost); return; }
            }

            Console.Clear();
            Console.WriteLine($"=== РЕЗУЛЬТАТ ТРЕНУВАННЯ ({trainedStat}) ===");

            if (s1 || s2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[+] Відмінне тренування! Характеристики зросли.");
                Console.WriteLine($"Поточний Загальний Рейтинг (OVR): {player.OverallRating}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[-] Ви виклались на повну, але сьогодні без помітного прогресу.");
            }
            Console.ResetColor();
        }
    }
}