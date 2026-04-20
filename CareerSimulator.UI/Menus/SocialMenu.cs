using System;
using CareerSimulator.Domain.Models;

namespace CareerSimulator.UI.Menus
{
    public static class SocialMenu
    {
        public static void OpenCharity(Player player)
        {
            while (true)
            {
                Console.WriteLine("\n=== БЛАГОДІЙНИЙ ФОНД ===");
                Console.WriteLine($"Ваш баланс: {player.Money}$ | Ваша Слава: {player.Reputation}");
                Console.WriteLine("Допомагаючи іншим, ви підвищуєте свою впізнаваність у світі футболу.");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1. Закупити м'ячі для місцевої школи (1,000$) -> +10 Слави");
                Console.WriteLine("2. Реконструкція футбольного поля (5,000$) -> +60 Слави");
                Console.WriteLine("3. Внесок у глобальний екологічний фонд (20,000$) -> +300 Слави");
                Console.WriteLine("0. Повернутися");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                if (choice == "0") break;

                try
                {
                    if (choice == "1") { player.SpendMoney(1000m); player.ChangeReputation(10); Console.WriteLine("\nДіти щасливі! Ваша слава зросла."); }
                    else if (choice == "2") { player.SpendMoney(5000m); player.ChangeReputation(60); Console.WriteLine("\nСтуденти вам вдячні! Ваша слава значно зросла."); }
                    else if (choice == "3") { player.SpendMoney(20000m); player.ChangeReputation(300); Console.WriteLine("\nПро вас пишуть у всіх світових ЗМІ! Величезний приріст слави."); }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        public static void OpenSponsors(Player player)
        {
            Console.WriteLine("\n=== РЕКЛАМНІ КОНТРАКТИ ===");
            Console.WriteLine($"Ваша Слава: {player.Reputation} | Поточний спонсор: {player.SponsorName} (+{player.SponsorIncome}$/тижд.)");
            Console.WriteLine("Бренди готові платити вам за популярність!");
            Console.WriteLine("-----------------------------------");

            string sponsor1 = player.Reputation >= 100 ? "1. Місцевий бренд одягу (+50$/тижд.)" : "[Закрито] Потрібно 100 Слави";
            string sponsor2 = player.Reputation >= 500 ? "2. Національна мережа піцерій (+250$/тижд.)" : "[Закрито] Потрібно 500 Слави";
            string sponsor3 = player.Reputation >= 2000 ? "3. Глобальний контракт з Nike (+2000$/тижд.)" : "[Закрито] Потрібно 2000 Слави";

            Console.WriteLine(sponsor1);
            Console.WriteLine(sponsor2);
            Console.WriteLine(sponsor3);
            Console.WriteLine("0. Повернутися");
            Console.Write("Оберіть контракт: ");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1" && player.Reputation >= 100) { player.SignSponsorship("Місцевий бренд", 50); Console.WriteLine("\n[+] Контракт підписано!"); }
            else if (choice == "2" && player.Reputation >= 500) { player.SignSponsorship("Мережа піцерій", 250); Console.WriteLine("\n[+] Ви тепер обличчя бренду!"); }
            else if (choice == "3" && player.Reputation >= 2000) { player.SignSponsorship("Nike", 2000); Console.WriteLine("\n[+] Світовий ексклюзив! Величезний дохід."); }
            else if (choice != "0") { Console.WriteLine("\n[-] Недостатньо слави або невірний вибір."); }
        }
    }
}