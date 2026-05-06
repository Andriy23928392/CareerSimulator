using CareerSimulator.Domain.Core;
using CareerSimulator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CareerSimulator.Domain.Logic
{
    public static class TransferManager
    {
        private static readonly Random _random = new Random();

        private static readonly List<Club> _standardClubs = new List<Club>
        {
            new Club("Нива (Тернопіль)", 50m, 25),
            new Club("Оболонь (Київ)", 180m, 45, 25),
            new Club("Полісся (Житомир)", 500m, 60, 50),
            new Club("Динамо (Київ)", 1000m, 70, 150),
            new Club("Шахтар (Донецьк)", 1200m, 73, 150),
            new Club("Жирона (Іспанія)", 2000m, 76, 300),
            new Club("Галатасарай (Туреччина)", 2500m, 78, 500),
            new Club("Бенфіка (Португалія)", 2500m, 78, 500),
            new Club("Марсель (Франція)", 2500m, 78, 500),
            new Club("Боруссія Дортмунд (Німеччина)", 3000m, 80, 750),
            new Club("Баєр Леверкузен (Німеччина)", 4000m, 82, 1000),
            new Club("Мілан (Італія)", 4000m, 82, 1000),
            new Club("Ювентус (Італія)", 4000m, 83, 1000),
            new Club("Атлетіко Мадрид", 5000m, 86, 1000),
            new Club("Челсі (Лондон)", 5000m, 86, 1000),
            new Club("Манчестер Юнайтед", 6000m, 87, 1000),
            new Club("Інтер Мілан (Італія)", 6000m, 87, 1000),
            new Club("Баварія (Мюнхен)", 8000m, 89, 2000),
            new Club("Ліверпуль", 8000m, 89, 2000),
            new Club("Манчестер Сіті", 8000m, 89, 2000),
            new Club("ПСЖ", 9000m, 90, 2000),
            new Club("Барселона", 10000m, 91, 2000)
        };

        private static readonly List<Club> _veteranClubs = new List<Club>
        {
            new Club("Лос-Анджелес ФК (США)", 15000m, 75, 5000),
            new Club("Інтер Маямі (США)", 18000m, 78, 5000),
            new Club("Аль-Іттіхад (СА)", 25000m, 80, 5000),
            new Club("Аль-Наср (СА)", 35000m, 82, 5000),
            new Club("Аль-Хіляль (СА)", 40000m, 83, 5000)
        };

        public static void OpenAgency(Player player, TimeManager timeManager)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("=== 🕴️ ВАШЕ ФУТБОЛЬНЕ АГЕНТСТВО ===");
                Console.ResetColor();

                string agentName = player.AgentLevel == 0 ? "Базовий агент (Доступ лише до клубів України)" :
                                   player.AgentLevel == 1 ? "Європейський агент (Зарплата +20%, доступ до середньої Європи)" :
                                   "Світова Акула (Зарплата +50%, доступ до ТОП-клубів)";

                Console.WriteLine($"Ваш поточний агент: {agentName}");
                Console.WriteLine($"Ваш Рейтинг: {player.OverallRating} | Слава: {player.Reputation}");

                if (player.PreContractClub != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n[📄 СТАТУС] У вас підписана попередня угода з: {player.PreContractClub.Name}");
                    Console.WriteLine("Очікуйте відкриття трансферного вікна!");
                    Console.ResetColor();
                }

                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("1. Попросити агента знайти новий клуб (Трансфер)");
                Console.WriteLine("2. Найняти кращого агента");
                Console.WriteLine("0. Повернутися назад");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                if (choice == "2")
                {
                    if (player.AgentLevel >= 2)
                    {
                        Console.WriteLine("\n[!] У вас вже найкращий агент у світі!");
                    }
                    else
                    {
                        decimal cost = player.AgentLevel == 0 ? 5000 : 30000;
                        string nextLevelName = player.AgentLevel == 0 ? "Європейського агента" : "Світову Акулу";

                        Console.WriteLine($"\nНайняти {nextLevelName} коштує {cost}$.");
                        Console.Write("Оплатити послуги? (1 - Так, 0 - Ні): ");
                        if (Console.ReadLine() == "1")
                        {
                            if (player.Money >= cost) { player.SpendMoney(cost); player.AgentLevel++; Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\n[+] Успішно! Тепер у вас набагато більше зв'язків."); Console.ResetColor(); }
                            else Console.WriteLine("\n[-] Недостатньо грошей!");
                        }
                    }
                    Console.ReadKey();
                }
                else if (choice == "1")
                {
                    if (player.PreContractClub != null)
                    {
                        Console.WriteLine($"\n[!] Агент каже: 'Друже, ми вже підписали папери з {player.PreContractClub.Name}! Сиди тихо до вікна.'");
                        Console.ReadKey();
                        continue;
                    }

                    Console.WriteLine("\nАгент підняв свої зв'язки і почав обдзвонювати клуби...");
                    System.Threading.Thread.Sleep(1500);

                    Club? offer = GenerateAgentOffer(player);

                    if (offer != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n🚨 Є ПРОПОЗИЦІЯ! Клуб {offer.Name} готовий підписати вас!");
                        Console.WriteLine($"Пропонована зарплата: {offer.WeeklySalary}$ на тиждень.");
                        Console.ResetColor();
                        Console.Write("Підписати угоду? (1 - Так, 0 - Ні): ");

                        if (Console.ReadLine() == "1")
                        {
                            int month = timeManager.CurrentDate.Month;
                            bool isWindowOpen = month == 1 || month == 7 || month == 8; 

                            if (isWindowOpen)
                            {
                                player.SignContract(offer);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"\n[+] Вітаємо! Вікно відкрите, ви офіційно перейшли в {offer.Name}!");
                            }
                            else
                            {
                                player.PreContractClub = offer;
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"\n[+] Ви підписали ПОПЕРЕДНЮ УГОДУ з {offer.Name}!");
                                Console.WriteLine("Ви перейдете туди автоматично, щойно відкриється найближче трансферне вікно.");
                            }
                            Console.ResetColor();
                        }
                        else Console.WriteLine("\nВи відхилили пропозицію.");
                    }
                    else
                    {
                        Console.WriteLine("\n[-] На жаль, зараз немає клубів вашого рівня, які б хотіли вас підписати.");
                    }
                    Console.ReadKey();
                }
            }
        }

        private static Club? GenerateAgentOffer(Player player)
        {
            int ratingBonus = player.AgentLevel == 2 ? 5 : (player.AgentLevel == 1 ? 2 : 0);
            int repBonus = player.AgentLevel == 2 ? 200 : (player.AgentLevel == 1 ? 50 : 0);

            var availableClubs = _standardClubs
                .Where(c => (player.OverallRating + ratingBonus) >= c.RequiredRating
                         && (player.Reputation + repBonus) >= c.RequiredReputation
                         && c.Name != player.CurrentClub.Name)
                .ToList();

            if (player.AgentLevel == 0) availableClubs = availableClubs.Where(c => c.RequiredRating <= 75).ToList();
            else if (player.AgentLevel == 1) availableClubs = availableClubs.Where(c => c.RequiredRating <= 85).ToList();

            if (player.Age >= 34 && player.AgentLevel == 2)
            {
                var veteranOffers = _veteranClubs
                    .Where(c => (player.OverallRating + ratingBonus) >= c.RequiredRating
                             && (player.Reputation + repBonus) >= c.RequiredReputation
                             && c.Name != player.CurrentClub.Name)
                    .ToList();
                availableClubs.AddRange(veteranOffers);
            }

            if (availableClubs.Count == 0) return null;

            var realisticClubs = availableClubs.Where(c => c.RequiredRating >= player.OverallRating - 8).ToList();

            if (realisticClubs.Count == 0)
            {
                realisticClubs = availableClubs.OrderByDescending(c => c.RequiredRating).Take(5).ToList();
            }

            Club selectedClub = realisticClubs[_random.Next(realisticClubs.Count)];

            decimal salaryMultiplier = player.AgentLevel == 0 ? 1.0m :
                                       player.AgentLevel == 1 ? 1.20m : 1.50m;

            decimal finalSalary = Math.Round(selectedClub.WeeklySalary * salaryMultiplier);

            return new Club(selectedClub.Name, finalSalary, selectedClub.RequiredRating, selectedClub.RequiredReputation);
        }

        public static Club GetEmergencyTransfer(Player player)
        {
            var available = _standardClubs
                .Where(c => c.RequiredRating <= player.OverallRating &&
                            c.RequiredRating >= player.OverallRating - 10 &&
                            c.Name != player.CurrentClub.Name)
                .OrderByDescending(c => c.RequiredRating)
                .ToList();

            if (available.Count > 0)
                return available[0];

            return _standardClubs[0];
        }
    }
}