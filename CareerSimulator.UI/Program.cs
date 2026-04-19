using System;
using CareerSimulator.Domain.Activities;
using CareerSimulator.Domain.Core;
using CareerSimulator.Domain.Models;

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
                myPlayer = new Player(playerName, Position.Defender);
                gameTime = new TimeManager(myPlayer);
            }

            var training = new TrainingActivity();
            var rest = new RestActivity();
            var match = new MatchActivity();

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine($"Статус гравця: {myPlayer.Name} ({myPlayer.Age} років) | Клуб: {myPlayer.CurrentClub.Name}");
                Console.WriteLine($"Енергія: {myPlayer.Energy} | Гроші: {myPlayer.Money}$ | Рейтинг: {myPlayer.OverallRating}");
                Console.WriteLine($"Дата: {gameTime.CurrentDate.ToString("dd.MM.yyyy")}");
                Console.WriteLine("==============================");

                Console.WriteLine("Оберіть дію:");
                Console.WriteLine($"1. {training.Name} (-30 енергії)");
                Console.WriteLine($"2. {rest.Name} (Відновлює енергію)");
                Console.WriteLine($"3. {match.Name} (-40 енергії)");
                Console.WriteLine("4. Відвідати магазин");
                Console.WriteLine("5. Просимулювати РІК");
                Console.WriteLine("8. ЗБЕРЕГТИ ГРУ");
                Console.WriteLine("0. Вийти з гри");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        gameTime.ExecuteActivity(training);
                        break;
                    case "2":
                        gameTime.ExecuteActivity(rest);
                        break;
                    case "3":
                        gameTime.ExecuteActivity(match);
                        break;
                    case "4":
                        OpenStore(myPlayer); 
                        break;
                    case "5":
                        gameTime.SimulateYear();
                        break;
                    case "8":
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
        static void OpenStore(Player player)
        {
            var storeItems = new CareerSimulator.Domain.Items.Item[]
            {
             new CareerSimulator.Domain.Items.EnergyDrink(),
             new CareerSimulator.Domain.Items.EliteBoots(player.BootsLevel)
            };

            Console.WriteLine("\n=== СПОРТИВНИЙ МАГАЗИН ===");
            Console.WriteLine($"Ваш баланс: {player.Money}$");

            for (int i = 0; i < storeItems.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {storeItems[i].Name} - Ціна: {storeItems[i].Price}$");
            }
            Console.WriteLine("0. Вийти з магазину");
            Console.Write("Що бажаєте придбати? Ваш вибір: ");

            string choice = Console.ReadLine() ?? "";

            if (int.TryParse(choice, out int itemIndex) && itemIndex > 0 && itemIndex <= storeItems.Length)
            {
                var selectedItem = storeItems[itemIndex - 1];

                try
                {
                    player.SpendMoney(selectedItem.Price);
                    selectedItem.Apply(player);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}