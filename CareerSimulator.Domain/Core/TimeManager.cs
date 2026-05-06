using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Activities;
using System;
using System.Linq; 

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

            int oldAge = _player.Age;

            // === СТАРІННЯ ТІЛЬКИ РАЗ НА РІК ===
            if (expectedAge > oldAge)
            {
                _player.SetAge(expectedAge);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n🎂 З ДНЕМ НАРОДЖЕННЯ!");
                Console.WriteLine($"Сьогодні {_player.BirthDate:dd.MM}, вам виповнюється {_player.Age} років.");
                Console.ResetColor();

                if (_player.Age >= 31)
                {
                    _player.Attributes.Pace = Math.Max(35, _player.Attributes.Pace - 2);
                    _player.Attributes.Physical = Math.Max(35, _player.Attributes.Physical - 1);
                    _player.Attributes.GK_Speed = Math.Max(35, _player.Attributes.GK_Speed - 2);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[📉 ВІКОВІ ЗМІНИ] Ваш вік ({_player.Age}). Ви втрачаєте швидкість та витривалість.");
                    Console.ResetColor();
                }

                if (_player.Age >= 36)
                {
                    _player.Attributes.Shooting = Math.Max(50, _player.Attributes.Shooting - 1);
                    _player.Attributes.Passing = Math.Max(50, _player.Attributes.Passing - 1);
                    _player.Attributes.Dribbling = Math.Max(50, _player.Attributes.Dribbling - 1);
                    _player.Attributes.Defending = Math.Max(50, _player.Attributes.Defending - 1);

                    _player.Attributes.GK_Diving = Math.Max(50, _player.Attributes.GK_Diving - 1);
                    _player.Attributes.GK_Reflexes = Math.Max(50, _player.Attributes.GK_Reflexes - 1);
                    _player.Attributes.GK_Positioning = Math.Max(50, _player.Attributes.GK_Positioning - 1);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[📉 ВІКОВІ ЗМІНИ] Ваш вік ({_player.Age}) дається взнаки. Ви втрачаєте технічні характеристики.");
                    Console.ResetColor();
                }

                if (_player.Age >= _player.Stats.RetirementAge)
                {
                    EndCareer();
                    return;
                }
            }

            int currentMonth = CurrentDate.Month;
            if ((currentMonth == 1 || currentMonth == 7 || currentMonth == 8) && _player.PreContractClub != null)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n==================================================");
                Console.WriteLine("🔄 ТРАНСФЕРНЕ ВІКНО ВІДКРИТО!");
                Console.WriteLine($"Ваша попередня домовленість вступає в силу.");
                Console.WriteLine($"Ви офіційно переходите до клубу: {_player.PreContractClub.Name}");
                Console.WriteLine("==================================================");
                Console.ResetColor();

                _player.SignContract(_player.PreContractClub);
                _player.PreContractClub = null;

                Console.WriteLine("\nНатисніть будь-яку клавішу...");
                Console.ReadKey();
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

            if (_player.PlayerPosition == Position.Goalkeeper)
            {
                Console.WriteLine($"Матчів на нуль (Clean Sheets): {_player.Stats.TotalCleanSheets}");
            }
            else
            {
                Console.WriteLine($"Забито голів: {_player.Stats.TotalGoals}");
                Console.WriteLine($"Зроблено асистів: {_player.Stats.TotalAssists}");
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Виграно Золотих м'ячів: {_player.Stats.BallonDorAwards} 🏆");
            Console.ResetColor();

            Console.WriteLine($"Зароблено грошей: {_player.Money}$");
            Console.WriteLine($"Фінальна Слава: {_player.Reputation}");
            Console.WriteLine($"Фінальний Рейтинг: {_player.OverallRating}");
            Console.WriteLine("--------------------------------------------------");

            Console.WriteLine("\n🏆 ГОЛОВНІ ЗДОБУТКИ:");
            int clCount = _player.Stats.Trophies.Count(t => t.Contains("Ліга Чемпіонів"));
            int wcCount = _player.Stats.Trophies.Count(t => t.Contains("Світу"));
            int euroCount = _player.Stats.Trophies.Count(t => t.Contains("Євро"));
            int leagueCount = _player.Stats.Trophies.Count(t => t.Contains("Чемпіон Ліги"));

            if (wcCount > 0) Console.WriteLine($"🌍 Чемпіонат Світу: {wcCount} шт.");
            if (euroCount > 0) Console.WriteLine($"🇪🇺 Чемпіонат Європи: {euroCount} шт.");
            if (clCount > 0) Console.WriteLine($"⭐ Ліга Чемпіонів: {clCount} шт.");
            if (leagueCount > 0) Console.WriteLine($"🥇 Національні Ліги: {leagueCount} шт.");
            if (wcCount == 0 && euroCount == 0 && clCount == 0 && leagueCount == 0) Console.WriteLine("На жаль, кабінет трофеїв порожній.");
            Console.WriteLine("--------------------------------------------------");

            string title;
            if (_player.Stats.BallonDorAwards >= 3) title = "ЛЕГЕНДА СВІТОВОГО ФУТБОЛУ 🐐";
            else if (_player.Stats.BallonDorAwards >= 1 || _player.Reputation > 20000) title = "ІСТОРИЧНА ЗІРКА 🌟 ";
            else if (_player.Reputation > 10000 || _player.OverallRating >= 85) title = "ВИДАТНИЙ ПРОФЕСІОНАЛ 🏅";
            else if (_player.Stats.TotalMatches > 100 || _player.Reputation > 3000) title = "ВЕТЕРАН ТА УЛЮБЛЕНЕЦЬ ФАНАТІВ 👏";
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

                        int winChance = 30 + (_player.CurrentClub.RequiredRating / 2) + (_player.OverallRating / 4);
                        if (_player.Morale >= 80) winChance += 5;
                        else if (_player.Morale < 30) winChance -= 10;

                        winChance = Math.Min(78, winChance); 

                        int roll = _random.Next(1, 101);
                        bool isWin = roll <= winChance;
                        bool isDraw = !isWin && roll <= winChance + 25; 

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
                                if (_random.Next(0, 100) < 45) cleanSheets = 1;
                                if (_random.Next(0, 100) < 8) goals = 1;
                                if (_random.Next(0, 100) < 12) assists = 1;
                            }
                        }

                        _player.Stats.RecordMatchStats(goals, assists, cleanSheets, 0);

                        if (_player.Stats.SeasonLeagueMatches < 38)
                        {
                            _player.Stats.SeasonLeagueMatches++;
                            if (isWin) { _player.ChangeMorale(10); _player.Stats.SeasonWins++; }
                            else if (isDraw) { _player.ChangeMorale(-5); _player.Stats.SeasonDraws++; }
                            else { _player.ChangeMorale(-15); _player.Stats.SeasonLosses++; }
                        }
                        else
                        {
                            if (isWin) _player.ChangeMorale(5);
                            else if (isDraw) _player.ChangeMorale(-2);
                            else _player.ChangeMorale(-10);
                        }
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

            ShowSeasonResults();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[+] Симуляцію завершено! Зіграно матчів за рік: {totalMatchesSimulated}");
            Console.ResetColor();
        }
        public void ShowSeasonResults()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("🏆 ПІДСУМКИ ФУТБОЛЬНОГО СЕЗОНУ 🏆");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            Console.WriteLine("\n📊 ВАША ПЕРСОНАЛЬНА СТАТИСТИКА ЗА РІК:");
            Console.WriteLine("\n📊 ВАША ПЕРСОНАЛЬНА СТАТИСТИКА ЗА РІК:");
            if (_player.PlayerPosition == Position.Goalkeeper)
            {
                Console.WriteLine($"Матчів на нуль: {_player.Stats.SeasonCleanSheets}");
            }
            else if (_player.PlayerPosition == Position.Defender)
            {
                Console.WriteLine($"Матчів на нуль (надійна оборона): {_player.Stats.SeasonCleanSheets}");
                Console.WriteLine($"Забито голів (зі стандартів): {_player.Stats.SeasonGoals}");
                Console.WriteLine($"Асистів: {_player.Stats.SeasonAssists}");
            }
            else
            {
                Console.WriteLine($"Забито голів: {_player.Stats.SeasonGoals}");
                Console.WriteLine($"Асистів: {_player.Stats.SeasonAssists}");
            }
            Console.WriteLine("--------------------------------------------------");

            int points = (_player.Stats.SeasonWins * 3) + (_player.Stats.SeasonDraws * 1);
            Console.WriteLine($"\n📊 Ваш клуб '{_player.CurrentClub.Name}' набрав {points} очок у лізі.");

            bool wonLeague = false;
            int place = _random.Next(2, 5);

            if (points >= 95) wonLeague = true;
            else if (points >= 90) wonLeague = _random.Next(1, 101) <= 80;
            else if (points >= 85) wonLeague = _random.Next(1, 101) <= 60;

            if (wonLeague)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[+] ВІТАЄМО! Ви стали ЧЕМПІОНАМИ ЛІГИ ({points} очок)! 🥇");
                _player.Stats.Trophies.Add($"🏆 Чемпіон Ліги ({CurrentDate.Year})");
                _player.ChangeReputation(500);
                _player.ChangeMorale(40);
                Console.ResetColor();
            }
            else
            {
                if (points >= 70) place = _random.Next(2, 4);
                else place = _random.Next(5, 12);
                Console.WriteLine($"[-] Ви не виграли лігу. Клуб посів {place}-е місце в чемпіонаті.");
            }

            bool inCL = points >= 75 || _player.CurrentClub.RequiredRating >= 84;
            bool inEL = !inCL && (points >= 60 || _player.CurrentClub.RequiredRating >= 70);

            if (inCL)
            {
                Console.WriteLine("\n⭐ ШЛЯХ У ЛІЗІ ЧЕМПІОНІВ:");
                SimulateKnockoutTournament("Ліга Чемпіонів", "CL");
            }
            else if (inEL)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\n🟠 ШЛЯХ У ЛІЗІ ЄВРОПИ:");
                Console.ResetColor();
                SimulateKnockoutTournament("Ліга Європи", "EL");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n📺 Цього сезону ви не кваліфікувалися в Єврокубки.");
                Console.ResetColor();
            }

            Console.WriteLine("\n🏆 НАЦІОНАЛЬНИЙ КУБОК:");
            SimulateKnockoutTournament("Національний Кубок", "Cup");

            bool isWorldCupYear = CurrentDate.Year % 4 == 2;
            bool isEuroYear = CurrentDate.Year % 4 == 0;
            bool isNationsLeagueYear = CurrentDate.Year % 2 != 0;

            if (isWorldCupYear || isEuroYear || isNationsLeagueYear)
            {
                string tournamentName = isWorldCupYear ? "Чемпіонат Світу" :
                                        isEuroYear ? "Євро" : "Ліга Націй";
                string type = isWorldCupYear ? "WC" : isEuroYear ? "Euro" : "NL";

                Console.WriteLine($"\n🌍 {tournamentName.ToUpper()} {CurrentDate.Year}:");

                if (_player.OverallRating >= 70 || _player.Reputation >= 1000)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("✅ Ви отримали офіційний виклик до Національної Збірної України!");
                    Console.ResetColor();

                    SimulateKnockoutTournament(tournamentName, type);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("❌ Тренер збірної не включив вас у заявку.");
                    Console.ResetColor();
                }
            }

            bool isSuperstar = _player.OverallRating >= 90;

            bool hasGoodStats = _player.PlayerPosition == Position.Goalkeeper
                ? _player.Stats.SeasonCleanSheets >= 15
                : (_player.Stats.SeasonGoals >= 20 || _player.Stats.SeasonAssists >= 15);

            bool hasInsaneStats = _player.PlayerPosition == Position.Goalkeeper
                ? _player.Stats.SeasonCleanSheets >= 25
                : _player.Stats.SeasonGoals >= 35;

            if ((isSuperstar && hasGoodStats) || hasInsaneStats)
            {
                _player.Stats.BallonDorAwards++;
                _player.ChangeMorale(50);
                _player.ChangeCoachTrust(50);
                _player.EarnMoney(100000);
                _player.ChangeReputation(1000);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n🌟🌟🌟 УВАГА! ВЕСЬ ФУТБОЛЬНИЙ СВІТ ЗАМЕР... 🌟🌟🌟");
                Console.WriteLine($"За феноменальну індивідуальну гру в цьому сезоні...");
                Console.WriteLine($"ГРАВЕЦЬ {_player.Name.ToUpper()} ОТРИМУЄ ЗОЛОТИЙ М'ЯЧ! 🏆");
                Console.WriteLine($"Це ваш {_player.Stats.BallonDorAwards}-й Золотий м'яч у кар'єрі!");
                Console.ResetColor();
            }
            else if (hasGoodStats || _player.OverallRating >= 85)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n👏 Ви провели блискучий сезон і були в номінації на Золотий м'яч!");
                Console.WriteLine("Але нагороду забрав інший гравець.");
                Console.ResetColor();
            }


            _player.Stats.SeasonWins = 0;
            _player.Stats.SeasonDraws = 0;
            _player.Stats.SeasonLosses = 0;
            _player.Stats.SeasonGoals = 0;
            _player.Stats.SeasonAssists = 0;
            _player.Stats.SeasonCleanSheets = 0;
            _player.Stats.SeasonLeagueMatches = 0;

            Console.WriteLine("\nНатисніть будь-яку клавішу для продовження кар'єри...");
            Console.ReadKey();
        }

        private void SimulateKnockoutTournament(string tournamentName, string type)
        {
            string[] stages;
            int tournamentDifficulty = 0;
            int teamRating = _player.CurrentClub.RequiredRating;

            switch (type)
            {
                case "CL": stages = new[] { "Груповий етап", "1/8 фіналу", "1/4 фіналу", "Півфінал", "ФІНАЛ" }; tournamentDifficulty = 88; break;
                case "EL": stages = new[] { "Груповий етап", "1/16 фіналу", "1/8 фіналу", "1/4 фіналу", "Півфінал", "ФІНАЛ" }; tournamentDifficulty = 78; break;
                case "Cup": stages = new[] { "1/16 фіналу", "1/8 фіналу", "1/4 фіналу", "Півфінал", "ФІНАЛ" }; tournamentDifficulty = teamRating - 5; break;
                case "WC": stages = new[] { "Груповий етап", "1/8 фіналу", "1/4 фіналу", "Півфінал", "ФІНАЛ" }; tournamentDifficulty = 85; teamRating = 76; break;
                case "Euro": stages = new[] { "Груповий етап", "1/8 фіналу", "1/4 фіналу", "Півфінал", "ФІНАЛ" }; tournamentDifficulty = 82; teamRating = 76; break;
                case "NL": stages = new[] { "Груповий етап", "Півфінал", "ФІНАЛ" }; tournamentDifficulty = 78; teamRating = 76; break;
                default: stages = new[] { "Півфінал", "ФІНАЛ" }; tournamentDifficulty = 70; break;
            }

            bool eliminated = false;

            for (int i = 0; i < stages.Length; i++)
            {
                System.Threading.Thread.Sleep(800);

                int chanceToPass = 50 + ((teamRating - tournamentDifficulty) * 4) + ((_player.OverallRating - 70) / 2);

                if (i == 0 && stages[i] == "Груповий етап") chanceToPass += 15;

                if (i == stages.Length - 1) chanceToPass -= 10; 

                chanceToPass = Math.Max(5, Math.Min(95, chanceToPass)); 

                if (_random.Next(1, 101) <= chanceToPass)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"   ✔️ {stages[i]}: Пройдено!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   ❌ {stages[i]}: Виліт з турніру...");
                    Console.ResetColor();
                    eliminated = true;
                    break;
                }
            }

            if (!eliminated)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   [+] ВИ ВИГРАЛИ {tournamentName.ToUpper()}! 🏆");
                _player.Stats.Trophies.Add($"🏆 {tournamentName} ({CurrentDate.Year})");

                int repBonus = type == "CL" || type == "WC" ? 1000 : 500;
                _player.ChangeReputation(repBonus);
                _player.ChangeMorale(50);
                Console.ResetColor();
            }
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