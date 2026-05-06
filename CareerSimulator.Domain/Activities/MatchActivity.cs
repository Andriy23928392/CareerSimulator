using CareerSimulator.Domain.Core;
using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Services;
using System;

namespace CareerSimulator.Domain.Activities
{
    public enum MatchType
    {
        League,          
        ChampionsLeague, 
        Cup,
        NationalTeam
        
    }

    public enum MatchLocation
    {
        Home,    
        Away,    
        Neutral  
    }
    public class MatchActivity : IActivity
    {
        public string Name => "Зіграти матч";
        public string Description => "Провести офіційний матч за свій клуб";

        private static Random _random = new Random();
        public void Execute(Player player)
        {
        }

        public void Execute(Player player, TimeManager timeManager, MatchType matchType, MatchLocation location)
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

            int baseEnergyCost = Math.Max(10, 40 - player.MatchDiscount);
            int actualEnergyCost = player.CalculateEnergyCost(baseEnergyCost);

            int winChance = 35 + (player.OverallRating / 4);
            Console.Clear();
            Console.WriteLine($"\n=== МАТЧ: {player.CurrentClub.Name} ({matchType.ToString().ToUpper()}) ===");

            if (location == MatchLocation.Home)
            {
                winChance += 5;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("🏟 Домашній матч: Рідні трибуни женуть вас вперед! (+10% до перемоги)");
                Console.ResetColor();
            }
            else if (location == MatchLocation.Away)
            {
                winChance -= 5; 
                actualEnergyCost += 10;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("🚌 Виїзний матч: Важкий переїзд і тиск чужих фанатів! (-5% шанс, +10 витрата енергії)");
                Console.ResetColor();
            }
            else if (location == MatchLocation.Neutral)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("🏟 Нейтральне поле: Рівні умови для обох команд.");
                Console.ResetColor();
            }

            try { player.SpendEnergy(actualEnergyCost); }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[МЕДИЧНИЙ ШТАБ] Ви занадто виснажені для цього матчу! (Потрібно: {actualEnergyCost}, Є: {player.Energy})");
                Console.ResetColor();
                return; 
            }

            player.AddMatchThisWeek();

            if (player.Morale >= 80) winChance += 5;
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
                        if (player.Attributes.Shooting > 85) goals = _random.Next(2, 5);
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
                player.ChangeCoachTrust(5);
            }
            else if (isDraw)
            {
                player.ChangeMorale(-5);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"НІЧИЯ. {positionText}");
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

            Random random = new Random();
            if (random.Next(1, 101) <= 25 && (player.PlayerPosition == Position.Forward || player.PlayerPosition == Position.Midfielder || player.PlayerPosition == Position.Goalkeeper))
            {
                int matchMinute = random.Next(70, 96);
                string minuteText = matchMinute > 90 ? $"90+{matchMinute - 90}" : matchMinute.ToString();

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n==================================================");
                Console.WriteLine("ПЕНАЛЬТІ!");
                Console.WriteLine("==================================================");
                Console.ResetColor();

                Console.WriteLine($"\n{minuteText}-а хвилина матчу! Рахунок рівний. Суддя вказує на позначку!");

                if (player.PlayerPosition != Position.Goalkeeper)
                {
                    Console.WriteLine($"{player.Name} бере м'яч. Стадіон завмер...\n");
                    Console.WriteLine("Куди будете бити?");
                    Console.WriteLine("1. На силу в лівий кут");
                    Console.WriteLine("2. На точність у правий кут");
                    Console.WriteLine("3. 'Паненка' по центру");
                    Console.Write("Ваш вибір (1/2/3): ");

                    string penaltyChoice = Console.ReadLine() ?? "";
                    bool isGoal = false;
                    int gkDive = random.Next(1, 4);

                    Console.WriteLine("\nРозбіг... Удар!");
                    System.Threading.Thread.Sleep(1500);

                    if (penaltyChoice == "1")
                    {
                        if (gkDive == 1) { isGoal = player.Attributes.Physical > random.Next(50, 95); Console.WriteLine(isGoal ? "Голкіпер вгадав кут, але удар був занадто потужним! ГОЛ!" : "Голкіпер вгадав кут і відбив цей потужний удар!"); }
                        else { isGoal = true; Console.WriteLine("Голкіпер стрибнув в інший кут! Впевнений ГОЛ!"); }
                    }
                    else if (penaltyChoice == "2")
                    {
                        if (gkDive == 2) { isGoal = player.Attributes.Shooting > random.Next(60, 95); Console.WriteLine(isGoal ? "Ідеальна точність! М'яч від стійки залітає у ворота! ГОЛ!" : "Воротар дотягнувся кінчиками пальців! Сейв!"); }
                        else { isGoal = player.Attributes.Shooting > 50; Console.WriteLine(isGoal ? "Воротар навіть не поворухнувся. ГОЛ!" : "Ой-ой... Ви перехвилювалися і не влучили по воротах!"); }
                    }
                    else if (penaltyChoice == "3")
                    {
                        if (gkDive == 3) { isGoal = false; Console.WriteLine("Який сором! Воротар залишився по центру і просто забрав м'яч до рук..."); }
                        else { isGoal = true; Console.WriteLine("Шедевр! Воротар полетів у кут, а м'яч елегантно опустився по центру! ГООООЛ!"); }
                    }
                    else { Console.WriteLine("Ви занадто довго думали, і суддя дав жовту картку за затягування часу."); }

                    if (isGoal)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n[+] Неймовірні емоції! Ви приносите команді користь.");
                        player.ChangeMorale(15); player.ChangeCoachTrust(10);
                        player.Stats.SeasonGoals++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[-] Фанати розчаровані. Ви підвели команду...");
                        player.ChangeMorale(-15); player.ChangeCoachTrust(-10);
                    }
                }
                else
                {
                    Console.WriteLine("Опонент встановлює м'яч на позначку. Ви нервово стрибаєте на лінії воріт...\n");
                    Console.WriteLine("Куди будете стрибати?");
                    Console.WriteLine("1. У лівий кут (Залежить від GK Diving)");
                    Console.WriteLine("2. У правий кут (Залежить від GK Diving)");
                    Console.WriteLine("3. Залишитись по центру (Залежить від GK Reflexes)");
                    Console.Write("Ваш вибір (1/2/3): ");

                    string gkChoice = Console.ReadLine() ?? "";
                    bool isSaved = false;

                    int strikerShot = random.Next(1, 101) <= 15 ? 4 : random.Next(1, 4);

                    Console.WriteLine("\nРозбіг... Удар!");
                    System.Threading.Thread.Sleep(1500);

                    if (strikerShot == 4)
                    {
                        Console.WriteLine("Гравець опонента не витримав тиску і пробив вище воріт! Вам навіть не довелося вступати в гру!");
                        isSaved = true;
                    }
                    else if (gkChoice == "1" || gkChoice == "2" || gkChoice == "3")
                    {
                        if (gkChoice == strikerShot.ToString())
                        {
                            int requiredStat = gkChoice == "3" ? player.Attributes.GK_Reflexes : player.Attributes.GK_Diving;
                            if (requiredStat > random.Next(50, 95))
                            {
                                isSaved = true;
                                Console.WriteLine("ФАНТАСТИЧНИЙ СЕЙВ! Ви витягуєте мертвого м'яча кінчиками рукавиць!");
                            }
                            else
                            {
                                Console.WriteLine("Ви вгадали напрямок, але удар був просто ідеальним... ГОЛ.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ви кинулися в інший кут, а м'яч спокійно закотився у сітку... ГОЛ.");
                        }
                    }
                    else { Console.WriteLine("Ви розгубилися на лінії і просто подивилися, як залітає гол."); }

                    if (isSaved)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n[+] ВИ ГЕРОЙ МАТЧУ! Трибуни скандують ваше ім'я!");
                        player.ChangeMorale(20); player.ChangeCoachTrust(15);
                        player.Stats.SeasonCleanSheets++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[-] Прикрий пропущений м'яч. Мораль команди падає...");
                        player.ChangeMorale(-10); player.ChangeCoachTrust(-5);
                    }
                }

                Console.ResetColor();
                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
            
            }
        }
    }
