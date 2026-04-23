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

        public void ExecuteActivity(CareerSimulator.Domain.Interfaces.IActivity activity)
        {
            activity.Execute(_player);

            if (activity is CareerSimulator.Domain.Activities.RestActivity)
            {
                AdvanceTime();
            }
        }

        public void AdvanceTime()
        {
            DateTime oldDate = CurrentDate;
            CurrentDate = CurrentDate.AddDays(7);

            // 1. ОНОВЛЕННЯ ЛІМІТІВ ТА ЗДОРОВ'Я
            _player.ResetWeeklyLimits();
            _player.HealOneWeek();

            // 2. ФІНАНСИ
            if (_player.CurrentClub.WeeklySalary > 0)
                _player.EarnMoney(_player.CurrentClub.WeeklySalary);
            if (_player.SponsorIncome > 0)
                _player.EarnMoney(_player.SponsorIncome);

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

            if (_player.Age >= 32 && CurrentDate.Month != oldDate.Month)
            {
                int degradeChance = 30 + ((_player.Age - 32) * 10);

                if (new Random().Next(1, 101) <= degradeChance)
                {
                    _player.Attributes.Pace -= 2;
                    _player.Attributes.Physical -= 1;
                    _player.Attributes.GK_Speed -= 2;

                    if (new Random().Next(1, 100) <= 50) _player.Attributes.Dribbling -= 1;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[📉 ВІКОВІ ЗМІНИ] Ваш вік ({_player.Age}) дається взнаки. Ви втрачаєте швидкість та витривалість.");
                    Console.ResetColor();
                }
            }

            var offer = CareerSimulator.Domain.Logic.TransferManager.CheckForTransferOffers(_player, CurrentDate);
            if (offer != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n=============================================");
                Console.WriteLine($"🚨 ТРАНСФЕРНА ПРОПОЗИЦІЯ! Клуб {offer.Name} зацікавився вами!");
                Console.WriteLine($"Пропонована зарплата: {offer.WeeklySalary}$ на тиждень.");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. Підписати контракт");
                Console.WriteLine("2. Відмовитись і залишитись");
                Console.Write("Ваш вибір: ");

                string transferChoice = Console.ReadLine() ?? "";
                if (transferChoice == "1")
                {
                    _player.SignContract(offer);
                    Console.WriteLine($"\nВітаємо! Ви офіційно стали гравцем {offer.Name}!");
                }
                Console.ResetColor();
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
            int totalMatchesSimulated = 0;

            for (int i = 0; i < 52; i++)
            {
                if (!_player.IsInjured)
                {
                    int matchCost = Math.Max(10, 40 - _player.MatchDiscount);
                    if (_player.Energy >= matchCost)
                    {
                        _player.SpendEnergy(matchCost);
                        _player.AddMatchThisWeek();
                        totalMatchesSimulated++;

                        int winChance = 40 + (_player.OverallRating / 2) + _player.WinChanceBonus;
                        if (_player.Morale >= 80) winChance += 10;
                        else if (_player.Morale < 30) winChance -= 10;

                        int roll = _random.Next(1, 101);
                        bool isWin = roll <= winChance;
                        bool isDraw = !isWin && roll <= winChance + 20;

                        int goals = 0, assists = 0, cleanSheets = 0;
                        if (isWin)
                        {
                            if (_player.PlayerPosition == Position.Forward) goals = _random.Next(1, 3);
                            if (_player.PlayerPosition == Position.Midfielder) { goals = _random.Next(0, 2); assists = _random.Next(1, 3); }
                            if (_player.PlayerPosition == Position.Defender || _player.PlayerPosition == Position.Goalkeeper) cleanSheets = 1;
                        }

                        _player.Stats.RecordMatchStats(goals, assists, cleanSheets, 0);

                        if (isWin) { _player.ChangeMorale(10); _player.EarnMoney(100); }
                        else if (isDraw) { _player.ChangeMorale(-5); _player.EarnMoney(50); }
                        else { _player.ChangeMorale(-15); }

                        if (_random.Next(1, 101) <= 4)
                        {
                            _player.SufferInjury("Мікротравма (Симуляція)", _random.Next(1, 4));
                        }
                    }

                    int trainCost = Math.Max(5, 30 - _player.TrainingDiscount);
                    if (_player.Energy >= trainCost && !_player.IsInjured)
                    {
                        _player.SpendEnergy(trainCost);
                        _player.AddTrainingThisWeek();

                        if (_player.PlayerPosition == Position.Goalkeeper)
                        {
                            int statToTrain = _random.Next(1, 7);
                            if (statToTrain == 1) _player.Attributes.TryImprove(ref _player.Attributes.GK_Diving);
                            else if (statToTrain == 2) _player.Attributes.TryImprove(ref _player.Attributes.GK_Reflexes);
                            else if (statToTrain == 3) _player.Attributes.TryImprove(ref _player.Attributes.GK_Positioning);
                            else if (statToTrain == 4) _player.Attributes.TryImprove(ref _player.Attributes.GK_Handling);
                            else if (statToTrain == 5) _player.Attributes.TryImprove(ref _player.Attributes.GK_Kicking);
                            else _player.Attributes.TryImprove(ref _player.Attributes.GK_Speed);
                        }
                        else
                        {
                            int statToTrain = _random.Next(1, 7);
                            if (statToTrain == 1) _player.Attributes.TryImprove(ref _player.Attributes.Pace);
                            else if (statToTrain == 2) _player.Attributes.TryImprove(ref _player.Attributes.Shooting);
                            else if (statToTrain == 3) _player.Attributes.TryImprove(ref _player.Attributes.Passing);
                            else if (statToTrain == 4) _player.Attributes.TryImprove(ref _player.Attributes.Physical);
                            else if (statToTrain == 5) _player.Attributes.TryImprove(ref _player.Attributes.Dribbling);
                            else
                            {
                                int defLimit = _player.PlayerPosition == Position.Forward ? 50 : (_player.PlayerPosition == Position.Midfielder ? 70 : 99);
                                _player.Attributes.TryImprove(ref _player.Attributes.Defending, defLimit);
                            }
                        }
                    }
                }

                if (_player.Energy < 30 && !_player.IsInjured)
                {
                    _player.RestoreEnergy(100);
                }

                AdvanceTime();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[+] Симуляцію завершено! Зіграно матчів за рік: {totalMatchesSimulated}");
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