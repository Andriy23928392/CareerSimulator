using System;
using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Items;

namespace CareerSimulator.UI.Menus
{
    public static class StoreMenu
    {
        public static void OpenStore(Player player)
        {
            var storeItems = new Item[]
            {
                new ProteinShake(),
                new PrCampaign(),
                new PremiumRehab(),
                new PsychologistSession()
            };

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==================================================");
                Console.WriteLine("                МАГАЗИН ПРЕДМЕТІВ                 ");
                Console.WriteLine("==================================================");
                Console.ResetColor();

                Console.WriteLine($"Ваш баланс: {player.Money}$");
                Console.WriteLine("Оберіть товар для покупки:\n");

                for (int i = 0; i < storeItems.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {storeItems[i].Name} - Ціна: {storeItems[i].Price}$");
                }
                Console.WriteLine("0. Вийти з магазину");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine() ?? "";

                if (choice == "0") break;

                if (int.TryParse(choice, out int itemIndex) && itemIndex > 0 && itemIndex <= storeItems.Length)
                {
                    Item selectedItem = storeItems[itemIndex - 1];

                    if (player.Money >= selectedItem.Price)
                    {
                        try
                        {
                            selectedItem.Apply(player);
                            player.SpendMoney(selectedItem.Price);
                            Console.WriteLine($"Ви успішно придбали: {selectedItem.Name}");
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n[Помилка] {ex.Message}");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[Помилка] Недостатньо коштів для покупки!");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine("\nНевірний вибір.");
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу...");
                Console.ReadKey();
            }
        }

        public static void OpenVIPStore(Player player)
        {
            while (true)
            {
                Console.Clear(); 
                Console.WriteLine("\n=== VIP АГЕНТСТВО 'LUXURY LIFE' ===");
                Console.WriteLine($"Ваш баланс: {player.Money}$");
                Console.WriteLine($"Знижка Тренування: -{player.TrainingDiscount} | Знижка Матчу: -{player.MatchDiscount}");
                Console.WriteLine($"Макс. Енергія: {player.MaxEnergy} | Воля до перемоги: +{player.WinChanceBonus}%");
                Console.WriteLine("-----------------------------------");

                decimal villaPrice = 5000m * (player.VillaLevel + 1) * (player.VillaLevel + 1);
                decimal gearPrice = 3000m * (player.GearLevel + 1) * (player.GearLevel + 1);
                decimal cryoPrice = 3500m * (player.CryoLevel + 1) * (player.CryoLevel + 1);
                decimal mentalPrice = 6000m * (player.MentalLevel + 1) * (player.MentalLevel + 1);

                string villaStatus = player.VillaLevel >= 5 ? "[МАКСИМУМ]" : $"{villaPrice}$";
                string gearStatus = player.GearLevel >= 5 ? "[МАКСИМУМ]" : $"{gearPrice}$";
                string cryoStatus = player.CryoLevel >= 5 ? "[МАКСИМУМ]" : $"{cryoPrice}$";
                string mentalStatus = player.MentalLevel >= 5 ? "[МАКСИМУМ]" : $"{mentalPrice}$";

                Console.WriteLine($"1. Елітна Вілла (Рівень {player.VillaLevel}/5) - +20 Макс. Енергії | Ціна: {villaStatus}");
                Console.WriteLine($"2. VIP-Екіпірування (Рівень {player.GearLevel}/5) - -2 енергії на тренування | Ціна: {gearStatus}");
                Console.WriteLine($"3. Домашня Кріокамера (Рівень {player.CryoLevel}/5) - -2 енергії на матч | Ціна: {cryoStatus}");
                Console.WriteLine($"4. Спортивний психолог (Рівень {player.MentalLevel}/5) - +2% шансу перемоги | Ціна: {mentalStatus}");
                Console.WriteLine("0. Повернутися в меню");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                try
                {
                    if (choice == "1")
                    {
                        if (player.VillaLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(villaPrice); player.UpgradeVilla(); Console.WriteLine("\n[+] Успішно! Ви розширили Віллу.");
                    }
                    else if (choice == "2")
                    {
                        if (player.GearLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(gearPrice); player.UpgradeGear(); Console.WriteLine("\n[+] Успішно! Ви оновили екіпірування.");
                    }
                    else if (choice == "3")
                    {
                        if (player.CryoLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(cryoPrice); player.UpgradeCryo(); Console.WriteLine("\n[+] Успішно! Ви купили кріокамеру.");
                    }
                    else if (choice == "4")
                    {
                        if (player.MentalLevel >= 5) { Console.WriteLine("\n[!] Максимальний рівень досягнуто."); continue; }
                        player.SpendMoney(mentalPrice); player.UpgradeMental(); Console.WriteLine("\n[+] Успішно! Ви найняли психолога.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу...");
                Console.ReadKey();
            }
        }
    }
}