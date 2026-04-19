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

        private void AdvanceTime()
        {
            int oldMonth = CurrentDate.Month;
            CurrentDate = CurrentDate.AddDays(7);

            if (oldMonth == 8 && CurrentDate.Month == 9)
            {
                _player.HaveBirthday();
                Console.WriteLine($"\n[ДЕНЬ НАРОДЖЕННЯ!] Вам тепер {_player.Age} років.");
            }

            if (_player.CurrentClub.WeeklySalary > 0)
                _player.EarnMoney(_player.CurrentClub.WeeklySalary);
        }

        public void SimulateYear()
        {
            Console.WriteLine("\n>>> Починаємо симуляцію ігрового року (52 тижні)...");

            decimal moneyAtStart = _player.Money;
            int ratingAtStart = _player.OverallRating;
            int energySpent = 0;

            for (int i = 0; i < 52; i++)
            {
                if (_player.Energy >= 30)
                {
                    _player.SpendEnergy(30);                 
                    energySpent += 30;
                }
                if (_random.Next(1, 101) <= 20)
                {
                    _player.ChangeRating(1);
                }

                else
                {
                    _player.RestoreEnergy(100);
                }

                AdvanceTime();
            }

            Infrastructure.SaveManager.SaveGame(_player, this);

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