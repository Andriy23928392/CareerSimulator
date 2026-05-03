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

        public (CareerSimulator.Domain.Activities.MatchType, CareerSimulator.Domain.Activities.MatchLocation) GetNextMatchDetails()
        {
            int month = CurrentDate.Month;
            int playerRating = _player.OverallRating;

            if (playerRating >= 75)
            {
                if (CurrentDate.Year % 2 == 0 && ((month == 6 && CurrentDate.Day >= 15) || (month == 7 && CurrentDate.Day <= 15)))
                {
                    if (_random.Next(1, 101) <= 50)
                        return (CareerSimulator.Domain.Activities.MatchType.NationalTeam, CareerSimulator.Domain.Activities.MatchLocation.Neutral);
                }
                else if (month == 9 || month == 10 || month == 11 || month == 3)
                {
                    if (_random.Next(1, 101) <= 30)
                    {
                        CareerSimulator.Domain.Activities.MatchLocation loc = (CareerSimulator.Domain.Activities.MatchLocation)_random.Next(0, 2);
                        return (CareerSimulator.Domain.Activities.MatchType.NationalTeam, loc);
                    }
                }
            }

            if (_player.Stats.SeasonLeagueMatches >= 38)
                return (CareerSimulator.Domain.Activities.MatchType.Cup, CareerSimulator.Domain.Activities.MatchLocation.Neutral);

            if (playerRating >= 75 && month != 1 && month != 6 && month != 7 && month != 8)
            {
                if (_random.Next(1, 101) <= 25)
                {
                    CareerSimulator.Domain.Activities.MatchLocation loc = (CareerSimulator.Domain.Activities.MatchLocation)_random.Next(0, 2);
                    return (CareerSimulator.Domain.Activities.MatchType.ChampionsLeague, loc);
                }
            }

            if (month == 1 || month == 2 || month == 5)
            {
                if (_random.Next(1, 101) <= 20)
                    return (CareerSimulator.Domain.Activities.MatchType.Cup, CareerSimulator.Domain.Activities.MatchLocation.Neutral);
            }

            CareerSimulator.Domain.Activities.MatchLocation defaultLoc = (CareerSimulator.Domain.Activities.MatchLocation)_random.Next(0, 2);
            return (CareerSimulator.Domain.Activities.MatchType.League, defaultLoc);
        }

        public void AdvanceTime()
        {
            DateTime oldDate = CurrentDate;
            CurrentDate = CurrentDate.AddDays(7);

            if (_player.TrainingsThisWeek == 0 && !_player.IsInjured)
            {
                _player.ChangeCoachTrust(-10);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[ТРЕНЕР] Ви не з'явилися на жодне тренування цього тижня!");
                Console.WriteLine("Довіра тренера падає (-10).");
                Console.ResetColor();
            }

            _player.ResetWeeklyLimits();
            _player.HealOneWeek();

            if (_player.CurrentClub.WeeklySalary > 0)
                _player.EarnMoney(_player.CurrentClub.WeeklySalary);
            if (_player.SponsorIncome > 0)
                _player.EarnMoney(_player.SponsorIncome);

            if (_player.CoachTrust == 0)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n[СКАДАЛ!] Тренеру увірвався терпець! Ваш контракт розірвано.");
                Console.ResetColor();

                var newClub = CareerSimulator.Domain.Logic.TransferManager.GetEmergencyTransfer(_player);
                _player.SignContract(newClub);
                _player.ChangeCoachTrust(50);
                _player.ChangeMorale(-30);

                Console.WriteLine($"Ваш агент терміново знайшов вам нову команду: {newClub.Name}.");
                Console.WriteLine($"Зарплата тепер: {newClub.WeeklySalary}$. Мораль різко впала.");
                Console.WriteLine("Натисніть будь-яку клавішу...");
                Console.ReadKey();
            }

            int expectedAge = CurrentDate.Year - _player.BirthDate.Year;
            if (CurrentDate < _player.BirthDate.AddYears(expectedAge)) expectedAge--;

            if (expectedAge > _player.Age)
            {
                _player.SetAge(expectedAge);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n🎂 З ДНЕМ НАРОДЖЕННЯ!");
                Console.WriteLine($"Сьогодні {_player.BirthDate:dd.MM}, вам виповнюється {_player.Age} років.");
                Console.ResetColor();

                if (_player.Age >= _player.Stats.RetirementAge)
                {
                    EndCareer();
                    return;
                }
            }

            if (CurrentDate.Month == 8 && oldDate.Month == 7)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n==================================================");
                Console.WriteLine($"🏆 СЕЗОН {CurrentDate.Year - 1}/{CurrentDate.Year} ЗАВЕРШЕНО! ПІДБИТТЯ ПІДСУМКІВ.");
                Console.WriteLine("==================================================");

                int seasonScore = 0;
                if (_player.PlayerPosition == Position.Forward || _player.PlayerPosition == Position.Midfielder)
                {
                    seasonScore = (_player.Stats.SeasonGoals * 2) + _player.Stats.SeasonAssists + _player.OverallRating;
                }
                else
                {
                    seasonScore = (_player.Stats.SeasonCleanSheets * 3) + _player.OverallRating;
                }

                int clubModifier = _player.OverallRating >= 85 ? 20 : 0;
                seasonScore += clubModifier;

                if (seasonScore >= 180)
                {
                    _player.Stats.BallonDorAwards++;
                    _player.ChangeMorale(50);
                    _player.ChangeCoachTrust(50);
                    _player.EarnMoney(100000);
                    _player.ChangeReputation(1000);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n🌟🌟🌟 УВАГА! ВЕСЬ ФУТБОЛЬНИЙ СВІТ ЗАМЕР... 🌟🌟🌟");
                    Console.WriteLine($"За феноменальну гру в цьому сезоні...");
                    Console.WriteLine($"ГРАВЕЦЬ {_player.Name.ToUpper()} ОТРИМУЄ ЗОЛОТИЙ М'ЯЧ!");
                    Console.WriteLine($"Це ваш {_player.Stats.BallonDorAwards}-й Золотий м'яч у кар'єрі!");
                    Console.WriteLine("Бонус: 100,000$ | +1000 Слави | Мораль і Довіра на максимумі!");
                    Console.ResetColor();
                }
                else if (seasonScore >= 130)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n👏 Ви провели блискучий сезон і потрапили в топ-10 номінантів на Золотий м'яч!");
                    Console.WriteLine("Але нагороду цього року забрав інший гравець. Працюйте далі!");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("\n📊 Сезон завершено. До Золотого м'яча ще треба рости, але все попереду.");
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу, щоб перейти до нового сезону...");
                Console.ReadKey();

                _player.Stats.ResetSeasonStats();
                Console.WriteLine("\n[!] Сезонну статистику обнулено. Починаємо з чистого аркуша!");
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
                HandleTransferOffer(offer);
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
                    _player.AddTrainingThisWeek();

                    int baseMatchCost = Math.Max(10, 40 - _player.MatchDiscount);
                    int actualMatchCost = _player.CalculateEnergyCost(baseMatchCost);

                    if (_player.Energy >= actualMatchCost)
                    {
                        _player.SpendEnergy(baseMatchCost);
                        _player.AddMatchThisWeek();
                        totalMatchesSimulated++;

                        int winChance = 40 + (_player.OverallRating / 2);
                        if (_player.Morale >= 80) winChance += 5;
                        else if (_player.Morale < 30) winChance -= 10;

                        int roll = _random.Next(1, 101);
                        bool isWin = roll <= winChance;
                        bool isDraw = !isWin && roll <= winChance + 20;

                        int goals = 0, assists = 0, cleanSheets = 0;
                        if (isWin)
                        {
                            if (_player.PlayerPosition == Position.Forward)
                            {
                                goals = _random.Next(0, 100) < 60 ? 1 : (_random.Next(0, 100) < 20 ? 2 : 0);
                                if (_random.Next(0, 100) < 30) assists = 1;
                            }
                            if (_player.PlayerPosition == Position.Midfielder)
                            {
                                if (_random.Next(0, 100) < 30) goals = 1;
                                assists = _random.Next(0, 100) < 50 ? 1 : (_random.Next(0, 100) < 15 ? 2 : 0);
                            }
                            if (_player.PlayerPosition == Position.Defender || _player.PlayerPosition == Position.Goalkeeper)
                            {
                                if (_random.Next(0, 100) < 40) cleanSheets = 1;
                            }
                        }

                        _player.Stats.RecordMatchStats(goals, assists, cleanSheets, 0);

                        if (isWin) { _player.ChangeMorale(10); }
                        else if (isDraw) { _player.ChangeMorale(-5); }
                        else { _player.ChangeMorale(-15); }

                        if (_random.Next(1, 101) <= 4)
                        {
                            _player.SufferInjury("Мікротравма (Симуляція)", _random.Next(1, 4));
                        }
                    }

                    int baseTrainCost = Math.Max(5, 30 - _player.TrainingDiscount);
                    int actualTrainCost = _player.CalculateEnergyCost(baseTrainCost);

                    if (_player.Energy >= actualTrainCost && !_player.IsInjured)
                    {
                        _player.SpendEnergy(baseTrainCost);
                        _player.AddTrainingThisWeek();

                        if (_player.PlayerPosition == Position.Goalkeeper)
                        {
                            int statToTrain = _random.Next(1, 7);
                            if (statToTrain == 1) _player.Attributes.TryImprove(ref _player.Attributes.GK_Diving, bonus: _player.TrainingBonus);
                            else if (statToTrain == 2) _player.Attributes.TryImprove(ref _player.Attributes.GK_Reflexes, bonus: _player.TrainingBonus);
                            else if (statToTrain == 3) _player.Attributes.TryImprove(ref _player.Attributes.GK_Positioning, bonus: _player.TrainingBonus);
                            else if (statToTrain == 4) _player.Attributes.TryImprove(ref _player.Attributes.GK_Handling, bonus: _player.TrainingBonus);
                            else if (statToTrain == 5) _player.Attributes.TryImprove(ref _player.Attributes.GK_Kicking, bonus: _player.TrainingBonus);
                            else _player.Attributes.TryImprove(ref _player.Attributes.GK_Speed, bonus: _player.TrainingBonus);
                        }
                        else
                        {
                            int statToTrain = _random.Next(1, 7);
                            if (statToTrain == 1) _player.Attributes.TryImprove(ref _player.Attributes.Pace, bonus: _player.TrainingBonus);
                            else if (statToTrain == 2) _player.Attributes.TryImprove(ref _player.Attributes.Shooting, bonus: _player.TrainingBonus);
                            else if (statToTrain == 3) _player.Attributes.TryImprove(ref _player.Attributes.Passing, bonus: _player.TrainingBonus);
                            else if (statToTrain == 4) _player.Attributes.TryImprove(ref _player.Attributes.Physical, bonus: _player.TrainingBonus);
                            else if (statToTrain == 5) _player.Attributes.TryImprove(ref _player.Attributes.Dribbling, bonus: _player.TrainingBonus);
                            else
                            {
                                int defLimit = _player.PlayerPosition == Position.Forward ? 50 : (_player.PlayerPosition == Position.Midfielder ? 70 : 99);
                                _player.Attributes.TryImprove(ref _player.Attributes.Defending, defLimit, bonus: _player.TrainingBonus);
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

        private void HandleTransferOffer(Club offer)
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
                _lastTransferDate = CurrentDate;
                Console.WriteLine($"\nВітаємо! Ви офіційно стали гравцем {offer.Name}!");
                CareerSimulator.Domain.Infrastructure.SaveManager.SaveGame(_player, this);
            }
            Console.ResetColor();
        }

        public void SetDate(DateTime date) { CurrentDate = date; }
    }
}