using System;
using CareerSimulator.Domain.Activities;
using CareerSimulator.Domain.Core;
using CareerSimulator.Domain.Models;
using CareerSimulator.UI.Menus;

namespace CareerSimulator.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Вітаємо в симуляторі кар'єри футболіста!");

            Player? myPlayer = null;
            TimeManager? gameTime = null;

            Console.WriteLine("1. Почати нову гру");
            Console.WriteLine("2. Завантажити збереження");
            Console.Write("Ваш вибір: ");

            string startChoice = Console.ReadLine() ?? "1";

            if (startChoice == "2")
            {
                try
                {
                    var loadedData = CareerSimulator.Domain.Infrastructure.SaveManager.LoadGame();
                    myPlayer = loadedData.Item1;
                    gameTime = loadedData.Item2;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка завантаження: {ex.Message}. Починаємо нову гру.");
                    startChoice = "1";
                }
            }

            if (startChoice != "2" || myPlayer == null || gameTime == null)
            {
                Console.Write("\nВведіть ім'я вашого гравця: ");
                string playerName = Console.ReadLine() ?? "Гравець";

                Console.WriteLine("Оберіть позицію на полі:");
                Console.WriteLine("1. Голкіпер");
                Console.WriteLine("2. Захисник");
                Console.WriteLine("3. Півзахисник");
                Console.WriteLine("4. Нападник");
                Console.Write("Ваш вибір: ");
                string posChoice = Console.ReadLine() ?? "2";

                Position startingPos = Position.Defender;
                if (posChoice == "1") startingPos = Position.Goalkeeper;
                else if (posChoice == "3") startingPos = Position.Midfielder;
                else if (posChoice == "4") startingPos = Position.Forward;

                myPlayer = new Player(playerName, startingPos);
                gameTime = new TimeManager(myPlayer);
            }

            var training = new TrainingActivity();
            var rest = new RestActivity();
            var match = new MatchActivity();

            bool isRunning = true;

            while (isRunning)
            {
                int baseTrainingCost = Math.Max(5, 30 - myPlayer.TrainingDiscount);
                int baseMatchCost = Math.Max(10, 40 - myPlayer.MatchDiscount);
                int actualTrainingCost = myPlayer.CalculateEnergyCost(baseTrainingCost);
                int actualMatchCost = myPlayer.CalculateEnergyCost(baseMatchCost);

                Console.WriteLine("\n==============================");
                Console.WriteLine($"Статус: {myPlayer.Name} ({myPlayer.Age} років) | {myPlayer.PlayerPosition} | {myPlayer.CurrentClub.Name}"); Console.WriteLine($"Енергія: {myPlayer.Energy} | Гроші: {myPlayer.Money}$ | Рейтинг: {myPlayer.OverallRating}");
                Console.WriteLine($"Довіра тренера: {myPlayer.CoachTrust}/100");
                Console.WriteLine($"Дата: {gameTime.CurrentDate.ToString("dd.MM.yyyy")}");
                Console.WriteLine("==============================");

                if (myPlayer.Morale < 30)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️ ДЕПРЕСІЯ: Ви виснажені емоційно! Всі дії забирають на 50% більше енергії!");
                    Console.ResetColor();
                }

                if (myPlayer.IsInjured)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"🚑 СТАТУС: Травмований ({myPlayer.InjuryName}). Залишилося: {myPlayer.WeeksInjured} тижнів.");
                    Console.ResetColor();
                }

                Console.WriteLine("Оберіть дію:");
                Console.WriteLine($"1. Інтенсивне тренування (-{actualTrainingCost} енергії)");
                Console.WriteLine($"2. Відпочинок вдома (Відновлює до {myPlayer.MaxEnergy})");
                Console.WriteLine($"3. Зіграти матч (-{actualMatchCost} енергії)");
                Console.WriteLine("4. Відвідати магазин");
                Console.WriteLine("5. Модифікатори");
                Console.WriteLine("6. Благодійний фонд");
                Console.WriteLine("7. Рекламні контракти");
                Console.WriteLine("8. Профіль гравця");
                Console.WriteLine("9. Просимулювати РІК");
                Console.WriteLine("10. Зберегти гру");
                Console.WriteLine("0. Вийти з гри");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        if (myPlayer.IsInjured) { Console.WriteLine("Ви травмовані! Треба відпочивати."); break; }
                        gameTime.ExecuteActivity(training);
                        CareerSimulator.Domain.Services.MedicalCenter.CheckForInjury(myPlayer, false);
                        break;
                    case "2":
                        gameTime.ExecuteActivity(rest);
                        break;
                    case "3":
                        if (myPlayer.IsInjured) { Console.WriteLine("Ви травмовані! Треба відпочивати."); break; }
                        gameTime.ExecuteActivity(match);
                        CareerSimulator.Domain.Services.MedicalCenter.CheckForInjury(myPlayer, false);
                        break;
                    case "4":
                        StoreMenu.OpenStore(myPlayer);
                        break;
                    case "5":
                        StoreMenu.OpenVIPStore(myPlayer); 
                        break;
                    case "6":
                        SocialMenu.OpenCharity(myPlayer);
                        break;
                    case "7":
                        SocialMenu.OpenSponsors(myPlayer);
                        break;
                    case "8":
                        ProfileMenu.OpenProfile(myPlayer);
                        break;
                    case "9":
                        gameTime.SimulateYear();
                        break;
                    case "10":
                        CareerSimulator.Domain.Infrastructure.SaveManager.SaveGame(myPlayer, gameTime);
                        break;
                    case "0":
                        isRunning = false;
                        Console.WriteLine("Дякуємо за гру!");
                        break;
                    default:
                        Console.WriteLine("Невідомий вибір, спробуйте ще раз.");
                        break;
                }
            }
        }
    }
}