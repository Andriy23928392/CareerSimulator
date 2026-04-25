using CareerSimulator.Domain.Core;
using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Services;
using System;

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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n[ПРЕСКОНФЕРЕНЦІЯ] Журналісти запрошують вас на пресконференцію перед матчем.");
                Console.ResetColor();
                Console.WriteLine("1. Піти до преси і відповісти на питання");
                Console.WriteLine("2. (Відмовитись) Зосередитись на розминці");
                Console.Write("Ваш вибір: ");

                string preMatchChoice = Console.ReadLine() ?? "2";
                if (preMatchChoice == "1") CareerSimulator.Domain.Events.EventManager.TriggerPreMatchInterview(player);
                else Console.WriteLine("Ви вирішили уникнути преси і зосередитись на майбутній грі.");
            }
            if (player.CoachTrust < 20)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[РІШЕННЯ ТРЕНЕРА] Тренер не довіряє вам і залишає на лаві запасних на матч.");
                Console.WriteLine("Вам потрібно повернути його довіру на тренуваннях!");
                Console.ResetColor();
                return;
            }

            int matchCost = Math.Max(10, 40 - player.MatchDiscount);
            try { player.SpendEnergy(matchCost); }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }

            player.AddMatchThisWeek();

            Console.Clear();
            Console.WriteLine($"\n=== МАТЧ: {player.CurrentClub.Name} ===");

            int winChance = 40 + (player.OverallRating / 2) + player.WinChanceBonus;

            if (player.Morale >= 80) winChance += 10;
            else if (player.Morale < 30) winChance -= 10;

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
                        int cleanSheetChance = (player.Attributes.GK_Positioning + player.Attributes.GK_Reflexes) / 2;
                        if (_random.Next(1, 101) <= cleanSheetChance) cleanSheets = 1;

                        if (_random.Next(1, 101) <= (player.Attributes.GK_Diving / 3)) penaltiesSaved = 1;
                    }
                    positionText = cleanSheets > 0 ? "Ви відстояли на нуль! Справжня стіна!" :
                                   isWin ? "Команда виграла, хоча ви й пропустили." :
                                   isDraw ? "Надійна гра на лінії, але напад підвів." :
                                            "Ви пропустили кілька прикрих голів. Захист не допоміг.";
                    break;

                case Position.Defender:
                    if (isWin && _random.Next(1, 101) <= (player.Attributes.Physical / 5)) goals = 1;
                    if (_random.Next(1, 101) <= (player.Attributes.Passing / 4)) assists = 1;
                    if (isWin && _random.Next(1, 101) <= player.Attributes.Defending) cleanSheets = 1;

                    positionText = isWin ? "Ви забетонували свій фланг і не дали супернику шансів." :
                                   isDraw ? "Напружена гра в обороні без помилок." :
                                            "Нападники суперника розірвали вашу зону захисту.";
                    break;

                case Position.Midfielder:
                    if (_random.Next(1, 101) <= (player.Attributes.Shooting / 2)) goals = _random.Next(1, 3);
                    if (_random.Next(1, 101) <= (player.Attributes.Passing / 1.5)) assists = _random.Next(1, 3);

                    positionText = isWin ? "Ви домінували в центрі поля і диктували темп гри." :
                                   isDraw ? "Багато боротьби в центрі, але без результату." :
                                            "Центр поля було програно, ви не змогли зв'язати гру.";
                    break;

                case Position.Forward:
                    if (isWin)
                    {
                        if (player.Attributes.Shooting > 85) goals = _random.Next(2, 5); // Хет-трики для топів
                        else if (player.Attributes.Shooting > 70) goals = _random.Next(1, 3);
                        else goals = _random.Next(1, 2);
                    }

                    else if (isDraw && player.Attributes.Shooting > 60) goals = _random.Next(0, 2);

                    if (_random.Next(1, 101) <= (player.Attributes.Passing / 3)) assists = 1;

                    positionText = goals > 1 ? "Ваш гольовий інстинкт приніс команді перемогу!" :
                                   goals == 1 ? "Ви забили важливий м'яч у цій грі." :
                                   isDraw ? "Ви мали моменти, але м'яч вперто не йшов у ворота." :
                                            "Захисники суперника повністю виключили вас з гри.";
                    break;
            }

            if (isWin)
            {
                player.ChangeMorale(10);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"ПЕРЕМОГА! {positionText}");
                int earned = _random.Next(80, 150);
                player.EarnMoney(earned);
                Console.WriteLine($"💰 Зароблено за матч: {earned}$");
                player.ChangeCoachTrust(5);
            }
            else if (isDraw)
            {
                player.ChangeMorale(-5);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"НІЧИЯ. {positionText}");
                int earned = _random.Next(30, 80);
                player.EarnMoney(earned);
                Console.WriteLine($"💰 Зароблено за матч: {earned}$");
                player.ChangeCoachTrust(0);
            }
            else
            {
                player.ChangeMorale(-15);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ПОРАЗКА. {positionText}");
                player.ChangeCoachTrust(-5);
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

            if (_random.Next(1, 101) <= 20)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n[МІКС-ЗОНА] Журналісти з мікрофонами чекають на вас після матчу.");
                Console.ResetColor();
                Console.WriteLine("1. Підійти до преси і дати коментар");
                Console.WriteLine("2. (Проігнорувати) Мовчки піти в роздягальню");
                Console.Write("Ваш вибір: ");

                if ((Console.ReadLine() ?? "2") == "1") CareerSimulator.Domain.Events.EventManager.TriggerPostMatchInterview(player, isWin, isDraw);
            }
            MedicalCenter.CheckForInjury(player, true);
        }
    }
}