using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Activities;
using System;

namespace CareerSimulator.Domain.Core
{
    public class TimeManager
    {
        public DateTime CurrentDate { get; private set; }
        private readonly Player _player;
        private DateTime _lastTransferDate = DateTime.MinValue;
        private static readonly Random _random = new Random();

        public TimeManager(Player player)
        {
            _player = player;
            CurrentDate = new DateTime(2025, 9, 1);
        }

        public int CurrentWeek => (CurrentDate - new DateTime(2025, 9, 1)).Days / 7 + 1;

        public void ExecuteActivity(IActivity activity)
        {
            Console.WriteLine($"\n--- {CurrentDate.ToString("d MMMM yyyy")} (Тиждень {CurrentWeek}) ---");
            Console.WriteLine($"Дія: {activity.Name}");

            try
            {
                activity.Execute(_player);

                AdvanceTime();

                if ((CurrentDate - _lastTransferDate).TotalDays > 150)
                {
                    var offer = Logic.TransferManager.CheckForTransferOffers(_player, CurrentDate);
                    if (offer != null) HandleTransferOffer(offer);
                }

                Events.EventManager.TriggerRandomEvent(_player);
            }
            catch (Exceptions.NotEnoughEnergyException ex)
            {
                Console.WriteLine($"ПОМИЛКА: {ex.Message}");
            }
        }

        public void AdvanceTime()
        {
            DateTime oldDate = CurrentDate;
            CurrentDate = CurrentDate.AddDays(7);

            if (_player.CurrentClub.WeeklySalary > 0)
            {
                _player.EarnMoney(_player.CurrentClub.WeeklySalary);
            }
            if (_player.SponsorIncome > 0)
            {
                _player.EarnMoney(_player.SponsorIncome);
            }

            if (CurrentDate.Year > oldDate.Year)
            {
                _player.SetAge(_player.Age + 1);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n🎂 З днем народження! Вам тепер {_player.Age} років.");
                Console.ResetColor();

                if (_player.Age >= _player.Stats.RetirementAge)
                {
                    EndCareer();
                    return;
                }
            }

            if (_player.Age >= 30)
            {
                if (CurrentDate.Month != oldDate.Month)
                {
                    int degradeChance = 30 + ((_player.Age - 30) * 5);
                    if (new Random().Next(1, 101) <= degradeChance)
                    {
                        _player.ChangeRating(-1);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[📉 СТАРІННЯ] Роки беруть своє... Ваш загальний рейтинг впав на 1.");
                        Console.ResetColor();
                    }
                }
            }
        }

        private void EndCareer()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("==================================================");
            Console.WriteLine("        🏁 КАР'ЄРА ОФІЦІЙНО ЗАВЕРШЕНА 🏁        ");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            Console.WriteLine($"\nГравець: {_player.Name}");
            Console.WriteLine($"Фінальний вік: {_player.Age} років");
            Console.WriteLine($"Останній клуб: {_player.CurrentClub.Name}");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Зіграно матчів: {_player.Stats.TotalMatches}");
            Console.WriteLine($"Зароблено грошей: {_player.Money}$");
            Console.WriteLine($"Фінальна Слава: {_player.Reputation}");
            Console.WriteLine($"Фінальний Рейтинг: {_player.OverallRating}");
            Console.WriteLine("--------------------------------------------------");

            string title;
            if (_player.Reputation >= 2000 && _player.OverallRating >= 85) title = "ЛЕГЕНДА СВІТОВОГО ФУТБОЛУ 👑";
            else if (_player.Reputation >= 500 || _player.OverallRating >= 75) title = "ВИДАТНИЙ ПРОФЕСІОНАЛ 🌟";
            else if (_player.Stats.TotalMatches > 100) title = "ВЕТЕРАН ТА УЛЮБЛЕНЕЦЬ ФАНАТІВ 👏";
            else title = "ДОБРОТНИЙ ГРАВЕЦЬ 👍";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"ВАШ СТАТУС В ІСТОРІЇ: {title}");
            Console.ResetColor();

            Console.WriteLine("\nДякуємо за цю неймовірну подорож!");
            Console.WriteLine("Натисніть будь-яку клавішу для виходу з гри...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public void SimulateYear()
        {
            Console.WriteLine("\n>>> Починаємо симуляцію ігрового року (52 тижні)...");

            decimal moneyAtStart = _player.Money;
            int ratingAtStart = _player.OverallRating;
            int energySpent = 0;

            for (int i = 0; i < 52; i++)
            {
                int energyCost = Math.Max(5, 30 - _player.TrainingDiscount);
                if (_player.Energy >= energyCost)
                {
                    _player.SpendEnergy(energyCost);
                    int totalChance = 15 + (_player.TrainingChanceBonus / 2);
                    if (_random.Next(1, 101) <= totalChance) { _player.ChangeRating(1); }
                    energySpent += energyCost;
                }
                else
                {
                    _player.RestoreEnergy(_player.MaxEnergy);
                }

                AdvanceTime();
            }

            CareerSimulator.Domain.Infrastructure.SaveManager.SaveGame(_player, this);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n======================================");
            Console.WriteLine("       РІЧНИЙ ЗВІТ СИМУЛЯЦІЇ         ");
            Console.WriteLine("======================================");
            Console.WriteLine($"Рейтинг: {ratingAtStart} -> {_player.OverallRating} (Ріст: +{_player.OverallRating - ratingAtStart})");
            Console.WriteLine($"Баланс: {moneyAtStart}$ -> {_player.Money}$");
            Console.WriteLine($"Чистий прибуток: {_player.Money - moneyAtStart}$");
            Console.WriteLine($"Витрачено енергії на тренуваннях: {energySpent}");
            Console.WriteLine($"Нова дата: {CurrentDate.ToString("dd MMMM yyyy")}");
            Console.WriteLine("======================================\n");
            Console.ResetColor();
        }

        private void HandleTransferOffer(Club newClub)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n[ТРАНСФЕРНА ПРОПОЗИЦІЯ!]");
            Console.ResetColor();

            Console.WriteLine($"Клуб {newClub.Name} хоче підписати вас!");
            Console.WriteLine($"Запропонована зарплата: {newClub.WeeklySalary}$ на тиждень.");
            Console.Write("Прийняти пропозицію? (введіть 'так', '1' або '+'): ");

            string response = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (response == "так" || response == "1" || response == "+" || response.StartsWith("т") || response.StartsWith("y"))
            {
                _player.SignContract(newClub);
                _lastTransferDate = CurrentDate;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nГРАЦІЯ! Ви підписали контракт із клубом {newClub.Name}!");
                Console.ResetColor();

                CareerSimulator.Domain.Infrastructure.SaveManager.SaveGame(_player, this);
            }
            else
            {
                Console.WriteLine("Ви відхилили пропозицію.");
            }
        }

        public void SetDate(DateTime date) { CurrentDate = date; }
    }
}