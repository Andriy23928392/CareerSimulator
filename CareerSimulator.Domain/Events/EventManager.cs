using System;
using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Events
{
    public static class EventManager
    {
        private static Random _random = new Random();

        public static void TriggerPostMatchInterview(Player player, bool isWin, bool isDraw)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("🎤 ПІСЛЯМАТЧЕВЕ ІНТЕРВ'Ю");
            Console.ResetColor();

            int outcomeRoll = _random.Next(1, 101);

            if (isWin)
            {
                Console.WriteLine("Журналіст: 'Вітаємо з перемогою! Команда виглядала чудово. Як прокоментуєте?'");
                Console.WriteLine("1. (Скромно) Перемога — це заслуга всієї команди.");
                Console.WriteLine("2. (Зухвало) Я розірвав їхній захист. Я найкращий!");
                Console.WriteLine("3. (Відповідально) Ми виграли, але я бачу помилки. Треба працювати далі.");
                Console.Write("\nВаша відповідь: ");

                string choice = Console.ReadLine() ?? "1";
                if (choice == "1") { player.ChangeReputation(15); Console.WriteLine("\n✅ Скромність цінується. Слава +15."); }
                else if (choice == "2")
                {
                    if (outcomeRoll <= 50) { player.ChangeReputation(50); Console.WriteLine("\n🔥 Ви зірка! Слава +50."); }
                    else { player.ChangeReputation(-30); Console.WriteLine("\n❌ Вас назвали егоїстом. Слава -30."); }
                }
                else { player.ChangeReputation(25); Console.WriteLine("\n🛡️ Ви справжній лідер. Слава +25."); }
            }
            else
            {
                Console.WriteLine("Журналіст: 'Результат сьогодні залишає бажати кращого. Що пішло не так?'");
                Console.WriteLine("1. (Відповідально) Це моя провина. Я маю грати краще.");
                Console.WriteLine("2. (Скандально) Суддя був сліпий, а наш захист просто спав!");
                Console.WriteLine("3. (Спокійно) Це футбол. Зробимо висновки і підемо далі.");
                Console.Write("\nВаша відповідь: ");

                string choice = Console.ReadLine() ?? "3";
                if (choice == "1") { player.ChangeReputation(25); Console.WriteLine("\n🛡️ Фанати цінують вашу чесність. Слава +25."); }
                else if (choice == "2")
                {
                    if (outcomeRoll <= 50) { player.ChangeReputation(40); Console.WriteLine("\n🔥 Скандал привернув увагу! Слава +40."); }
                    else { player.ChangeReputation(-40); player.ChangeRating(-1); Console.WriteLine("\n❌ Тренер лютує через ваші слова. Слава -40, Рейтинг -1."); }
                }
                else { player.ChangeReputation(15); Console.WriteLine("\n✅ Спокійна реакція. Слава +15."); }
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        public static void TriggerPreMatchInterview(Player player)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("📸 ПЕРЕДМАТЧЕВА ПРЕСКОНФЕРЕНЦІЯ");
            Console.WriteLine("Зал повен журналістів. Спалахи камер. До вас звертаються з питаннями...");
            Console.ResetColor();

            Console.WriteLine("\n[Питання 1/3] Журналіст: 'Що ви думаєте про сьогоднішнього суперника?'");
            Console.WriteLine("1. (З повагою) Це сильна команда, але ми добре підготувалися.");
            Console.WriteLine("2. (Зухвало) Ми їх просто знищимо. Навіть не сумнівайтесь.");
            Console.WriteLine("3. (Відповідально) Головне — це наша гра. Якщо ми виконаємо план тренера, все буде добре.");
            Console.Write("Ваша відповідь: ");
            string ans1 = Console.ReadLine() ?? "1";
            if (ans1 == "1") { player.ChangeReputation(10); Console.WriteLine("✅ Спокійна та поважна відповідь. (Слава +10)"); }
            else if (ans1 == "2")
            {
                if (_random.Next(1, 101) <= 50) { player.ChangeReputation(30); Console.WriteLine("🔥 Фанатам подобається ця зухвалість! (Слава +30)"); }
                else { player.ChangeReputation(-20); Console.WriteLine("❌ Суперник отримав додаткову мотивацію. (Слава -20)"); }
            }
            else { player.ChangeReputation(15); Console.WriteLine("🛡️ Професійний підхід справжнього лідера. (Слава +15)"); }

            Console.WriteLine("\n[Питання 2/3] Журналіст: 'Від вас сьогодні багато очікують. Чи відчуваєте ви тиск?'");
            Console.WriteLine("1. (Відповідально) Тиск — це частина футболу. Я беру цю відповідальність на себе.");
            Console.WriteLine("2. (Впевнено) Який тиск? Я народжений для таких матчів!");
            Console.WriteLine("3. (Скромно) Я просто вийду і зроблю все можливе для команди.");
            Console.Write("Ваша відповідь: ");
            string ans2 = Console.ReadLine() ?? "1";
            if (ans2 == "1") { player.ChangeReputation(20); Console.WriteLine("🛡️ Вболівальники бачать у вас надійного гравця. (Слава +20)"); }
            else if (ans2 == "2")
            {
                if (_random.Next(1, 101) <= 50) { player.ChangeReputation(40); Console.WriteLine("🔥 Медіа обожнюють ваші гучні заяви! (Слава +40)"); }
                else { player.ChangeReputation(-25); Console.WriteLine("❌ Преса вважає вас занадто пихатим. (Слава -25)"); }
            }
            else { player.ChangeReputation(10); Console.WriteLine("✅ Гарна, командна відповідь. (Слава +10)"); }

            Console.WriteLine("\n[Питання 3/3] Журналіст: 'І наостанок, що б ви хотіли передати вболівальникам перед грою?'");
            Console.WriteLine("1. (Класика) Дякуємо за підтримку, ми граємо для вас!");
            Console.WriteLine("2. (Інтелектуально) Ми проаналізували минулі помилки. Сьогодні ви побачите зовсім іншу тактику.");
            Console.WriteLine("3. (Агресивно) Готуйтеся святкувати, ми рознесемо їх вщент!");
            Console.Write("Ваша відповідь: ");
            string ans3 = Console.ReadLine() ?? "1";
            if (ans3 == "1") { player.ChangeReputation(15); Console.WriteLine("✅ Фанати задоволені. (Слава +15)"); }
            else if (ans3 == "2") { player.ChangeReputation(25); Console.WriteLine("🧠 Журналісти відзначили ваш високий футбольний IQ. (Слава +25)"); }
            else if (ans3 == "3")
            {
                if (_random.Next(1, 101) <= 50) { player.ChangeReputation(35); Console.WriteLine("🔥 Стадіон буде ревіти від захвату! (Слава +35)"); }
                else { player.ChangeReputation(-30); Console.WriteLine("❌ Експерти радять вам менше говорити і більше грати. (Слава -30)"); }
            }

            Console.WriteLine("\nПрес-аташе: 'На цьому все, дякуємо! Час готуватися до матчу.'");
            Console.WriteLine("Натисніть будь-яку клавішу, щоб вийти на поле...");
            Console.ReadKey();

        }
    }
}