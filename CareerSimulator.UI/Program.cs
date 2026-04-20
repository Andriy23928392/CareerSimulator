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
                int actualTrainingCost = Math.Max(5, 30 - myPlayer.TrainingDiscount);
                int actualMatchCost = Math.Max(10, 40 - myPlayer.MatchDiscount);

                Console.WriteLine("\n==============================");
                Console.WriteLine($"Статус гравця: {myPlayer.Name} ({myPlayer.Age} років) | Клуб: {myPlayer.CurrentClub.Name}");
                Console.WriteLine($"Енергія: {myPlayer.Energy} | Гроші: {myPlayer.Money}$ | Рейтинг: {myPlayer.OverallRating}");
                Console.WriteLine($"Дата: {gameTime.CurrentDate.ToString("dd.MM.yyyy")}");
                Console.WriteLine("==============================");

                Console.WriteLine("Оберіть дію:");
                Console.WriteLine($"1. Інтенсивне тренування (-{actualTrainingCost} енергії)");
                Console.WriteLine($"2. Відпочинок вдома (Відновлює до {myPlayer.MaxEnergy})");
                Console.WriteLine($"3. Зіграти матч (-{actualMatchCost} енергії)");
                Console.WriteLine("4. Відвідати магазин");
                Console.WriteLine("5. Модифікатори");
                Console.WriteLine("6. БЛАГОДІЙНИЙ ФОНД (Підвищення слави)");
                Console.WriteLine("7. РЕКЛАМНІ КОНТРАКТИ (Пасивний дохід)");
                Console.WriteLine("8. Просимулювати РІК");
                Console.WriteLine("9. ЗБЕРЕГТИ ГРУ");
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
                        OpenVIPStore(myPlayer);
                        break;
                    case "6":
                        OpenCharity(myPlayer);
                        break;
                    case "7":
                        OpenSponsors(myPlayer);
                        break;
                    case "8":
                        gameTime.SimulateYear(); 
                        break;
                    case "9":
                        CareerSimulator.Domain.Infrastructure.SaveManager.SaveGame(myPlayer, gameTime); // Збереження на 9
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
            };

            Console.WriteLine("\n=== ЗВИЧАЙНИЙ МАГАЗИН ===");
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
        static void OpenVIPStore(Player player)
        {
            while (true)
            {
                Console.WriteLine("\n=== VIP АГЕНТСТВО 'LUXURY LIFE' ===");
                Console.WriteLine($"Ваш баланс: {player.Money}$");
                Console.WriteLine($"Макс. Енергія: {player.MaxEnergy} | Знижка Тренування: -{player.TrainingDiscount} | Знижка Матчу: -{player.MatchDiscount}");
                Console.WriteLine($"Шанс рейтингу: +{player.TrainingChanceBonus}% | Воля до перемоги: +{player.WinChanceBonus}%");
                Console.WriteLine("-----------------------------------");

                decimal gymPrice = 4000m * (player.GymLevel + 1) * (player.GymLevel + 1);
                decimal villaPrice = 5000m * (player.VillaLevel + 1) * (player.VillaLevel + 1);
                decimal gearPrice = 3000m * (player.GearLevel + 1) * (player.GearLevel + 1);
                decimal cryoPrice = 3500m * (player.CryoLevel + 1) * (player.CryoLevel + 1);
                decimal mentalPrice = 6000m * (player.MentalLevel + 1) * (player.MentalLevel + 1);

                string gymStatus = player.GymLevel >= 5 ? "[МАКСИМУМ]" : $"{gymPrice}$";
                string villaStatus = player.VillaLevel >= 5 ? "[МАКСИМУМ]" : $"{villaPrice}$";
                string gearStatus = player.GearLevel >= 5 ? "[МАКСИМУМ]" : $"{gearPrice}$";
                string cryoStatus = player.CryoLevel >= 5 ? "[МАКСИМУМ]" : $"{cryoPrice}$";
                string mentalStatus = player.MentalLevel >= 5 ? "[МАКСИМУМ]" : $"{mentalPrice}$";

                Console.WriteLine($"1. Топ-тренер (Рівень {player.GymLevel}/5) - +2% шансу до рейтингу | Ціна: {gymStatus}");
                Console.WriteLine($"2. Елітна Вілла (Рівень {player.VillaLevel}/5) - +20 Макс. Енергії | Ціна: {villaStatus}");
                Console.WriteLine($"3. VIP-Екіпірування (Рівень {player.GearLevel}/5) - -2 енергії на тренування | Ціна: {gearStatus}");
                Console.WriteLine($"4. Домашня Кріокамера (Рівень {player.CryoLevel}/5) - -2 енергії на матч | Ціна: {cryoStatus}");
                Console.WriteLine($"5. Спортивний психолог (Рівень {player.MentalLevel}/5) - +2% шансу перемоги | Ціна: {mentalStatus}");
                Console.WriteLine("0. Повернутися в меню");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                try
                {
                    if (choice == "1")
                    {
                        if (player.GymLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(gymPrice); player.UpgradeGym(); Console.WriteLine("\n[+] Успішно! Ви найняли топ-тренера.");
                    }
                    else if (choice == "2")
                    {
                        if (player.VillaLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(villaPrice); player.UpgradeVilla(); Console.WriteLine("\n[+] Успішно! Ви розширили Віллу.");
                    }
                    else if (choice == "3")
                    {
                        if (player.GearLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(gearPrice); player.UpgradeGear(); Console.WriteLine("\n[+] Успішно! Ви оновили екіпірування.");
                    }
                    else if (choice == "4")
                    {
                        if (player.CryoLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(cryoPrice); player.UpgradeCryo(); Console.WriteLine("\n[+] Успішно! Ви купили кріокамеру.");
                    }
                    else if (choice == "5")
                    {
                        if (player.MentalLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(mentalPrice); player.UpgradeMental(); Console.WriteLine("\n[+] Успішно! Ви найняли психолога.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message} (Недостатньо коштів)");
                    Console.ResetColor();
                }
            }
        }
        static void OpenCharity(Player player)
        {
            while (true)
            {
                Console.WriteLine("\n=== БЛАГОДІЙНИЙ ФОНД ===");
                Console.WriteLine($"Ваш баланс: {player.Money}$ | Ваша Слава: {player.Reputation}");
                Console.WriteLine("Допомагаючи іншим, ви підвищуєте свою впізнаваність у світі футболу.");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1. Закупити м'ячі для місцевої школи (1,000$) -> +10 Слави");
                Console.WriteLine("2. Реконструкція футбольного поля (5,000$) -> +60 Слави");
                Console.WriteLine("3. Внесок у глобальний екологічний фонд (20,000$) -> +300 Слави");
                Console.WriteLine("0. Повернутися");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                try
                {
                    if (choice == "1") { player.SpendMoney(1000m); player.ChangeReputation(10); Console.WriteLine("\nДіти щасливі! Ваша слава зросла."); }
                    else if (choice == "2") { player.SpendMoney(5000m); player.ChangeReputation(60); Console.WriteLine("\nСтуденти вам вдячні! Ваша слава значно зросла."); }
                    else if (choice == "3") { player.SpendMoney(20000m); player.ChangeReputation(300); Console.WriteLine("\nПро вас пишуть у всіх світових ЗМІ! Величезний приріст слави."); }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
        static void OpenSponsors(Player player)
        {
            Console.WriteLine("\n=== РЕКЛАМНІ КОНТРАКТИ ===");
            Console.WriteLine($"Ваша Слава: {player.Reputation} | Поточний спонсор: {player.SponsorName} (+{player.SponsorIncome}$/тижд.)");
            Console.WriteLine("Бренди готові платити вам за популярність!");
            Console.WriteLine("-----------------------------------");

            string sponsor1 = player.Reputation >= 100 ? "1. Місцевий бренд одягу (+50$/тижд.)" : "[Закрито] Потрібно 100 Слави";
            string sponsor2 = player.Reputation >= 500 ? "2. Національна мережа піцерій (+250$/тижд.)" : "[Закрито] Потрібно 500 Слави";
            string sponsor3 = player.Reputation >= 2000 ? "3. Глобальний контракт з Nike (+2000$/тижд.)" : "[Закрито] Потрібно 2000 Слави";

            Console.WriteLine(sponsor1);
            Console.WriteLine(sponsor2);
            Console.WriteLine(sponsor3);
            Console.WriteLine("0. Повернутися");
            Console.Write("Оберіть контракт: ");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1" && player.Reputation >= 100) { player.SignSponsorship("Місцевий бренд", 50); Console.WriteLine("\n[+] Контракт підписано!"); }
            else if (choice == "2" && player.Reputation >= 500) { player.SignSponsorship("Мережа піцерій", 250); Console.WriteLine("\n[+] Ви тепер обличчя бренду!"); }
            else if (choice == "3" && player.Reputation >= 2000) { player.SignSponsorship("Nike/Adidas", 2000); Console.WriteLine("\n[+] Світовий ексклюзив! Величезний дохід."); }
            else if (choice != "0") { Console.WriteLine("\n[-] Недостатньо слави або невірний вибір."); }
        }
    }
}