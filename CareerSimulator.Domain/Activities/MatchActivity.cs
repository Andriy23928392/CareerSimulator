using System;
using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Core;

namespace CareerSimulator.Domain.Activities
{
    public class MatchActivity : IActivity
    {
        public string Name => "Зіграти матч";
        public string Description => "Провести офіційний матч за свій клуб";

        private static Random _random = new Random();

        public void Execute(Player player)
        {
            if (player.MatchesThisWeek >= 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[!] Ліміт матчів! Ви вже зіграли 2 гри цього тижня. Гравцю потрібен Відпочинок.");
                Console.ResetColor();
                return;
            }

            if (_random.Next(1, 101) <= 20)
            {
                CareerSimulator.Domain.Events.EventManager.TriggerPreMatchInterview(player);
            }

            int matchCost = Math.Max(10, 40 - player.MatchDiscount);
            try { player.SpendEnergy(matchCost); }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }

            player.AddMatchThisWeek();

            Console.Clear();
            Console.WriteLine($"\n=== МАТЧ: {player.CurrentClub.Name} ===");

            int winChance = 40 + (player.OverallRating / 2) + player.WinChanceBonus;
            int roll = _random.Next(1, 101);

            bool isWin = roll <= winChance;
            bool isDraw = !isWin && roll <= winChance + 20;
            bool isLoss = !isWin && !isDraw;

            int goals = 0, assists = 0, cleanSheets = 0, penaltiesSaved = 0;
            string positionText = "";

            switch (player.PlayerPosition)
            {
                case Position.Goalkeeper:
                    if (isWin || isDraw)
                    {
                        if (_random.Next(1, 100) <= 60) cleanSheets = 1;
                        if (_random.Next(1, 100) <= 15) penaltiesSaved = 1;
                    }
                    positionText = isWin ? "Ви здійснили кілька неймовірних сейвів!" :
                                   isDraw ? "Надійна гра на лінії, але напад підвів." :
                                            "Ви пропустили кілька прикрих голів. Захист не допоміг.";
                    break;

                case Position.Defender:
                    if (isWin && _random.Next(1, 100) <= 5) goals = 1;
                    if (_random.Next(1, 100) <= 10) assists = 1;
                    if (isWin && _random.Next(1, 100) <= 50) cleanSheets = 1;

                    positionText = isWin ? "Ви забетонували свій фланг і не дали супернику шансів." :
                                   isDraw ? "Напружена гра в обороні без помилок." :
                                            "Нападники суперника розірвали вашу зону захисту.";
                    break;

                case Position.Midfielder:
                    if (_random.Next(1, 100) <= 25) goals = _random.Next(0, 2);
                    if (_random.Next(1, 100) <= 40) assists = _random.Next(0, 2);

                    positionText = isWin ? "Ви домінували в центрі поля і диктували темп гри." :
                                   isDraw ? "Багато боротьби в центрі, але без результату." :
                                            "Центр поля було програно, ви не змогли зв'язати гру.";
                    break;

                case Position.Forward:
                    if (isWin) goals = _random.Next(1, 4);
                    else if (isDraw) goals = _random.Next(0, 2);
                    if (_random.Next(1, 100) <= 20) assists = 1;

                    positionText = isWin ? "Ваш гольовий інстинкт приніс команді важливі очки!" :
                                   isDraw ? "Ви мали моменти, але м'яч вперто не йшов у ворота." :
                                            "Захисники суперника повністю виключили вас з гри.";
                    break;
            }

            if (isWin)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"ПЕРЕМОГА! {positionText}");
                int earned = _random.Next(80, 150);
                player.EarnMoney(earned);
                Console.WriteLine($"💰 Зароблено за матч: {earned}$");

                if (_random.Next(1, 100) <= 35)
                {
                    player.ChangeRating(1);
                    Console.WriteLine("⬆️ Відмінна гра! Ви отримали досвід і ваш рейтинг зріс на +1!");
                }
                else
                {
                    Console.WriteLine("➡️ Ви зіграли круто, але досвіду для підвищення рейтингу поки замало.");
                }
            }
            else if (isDraw)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"НІЧИЯ. {positionText}");
                int earned = _random.Next(30, 80);
                player.EarnMoney(earned);
                Console.WriteLine($"💰 Зароблено за матч: {earned}$");

                if (_random.Next(1, 100) <= 15)
                {
                    player.ChangeRating(1);
                    Console.WriteLine("⬆️ Важка гра загартувала вас. Рейтинг зріс на +1!");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ПОРАЗКА. {positionText}");
                if (_random.Next(1, 100) <= 5)
                {
                    player.ChangeRating(-1);
                    Console.WriteLine("⬇️ Через жахливу гру та критику фанатів ваш рейтинг впав на 1.");
                }
            }
            Console.ResetColor();

            if (goals > 0 || assists > 0 || cleanSheets > 0 || penaltiesSaved > 0)
            {
                Console.WriteLine("\nВаша статистика в матчі:");
                if (goals > 0) Console.WriteLine($"- Голи: {goals}");
                if (assists > 0) Console.WriteLine($"- Асисти: {assists}");
                if (cleanSheets > 0) Console.WriteLine($"- Сухий матч (Кліншит)");
                if (penaltiesSaved > 0) Console.WriteLine($"- Відбито пенальті: {penaltiesSaved}");
            }

            player.Stats.RecordMatchStats(goals, assists, cleanSheets, penaltiesSaved);
            MedicalCenter.CheckForInjury(player, true);
        }
    }
}