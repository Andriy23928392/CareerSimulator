using System;
using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Items
{
    public class ProteinShake : Item
    {
        public override string Name => "Протеїновий коктейль (Відновлює 50 Енергії)";
        public override decimal Price => 150m;

        public override void Apply(Player player)
        {
            player.RestoreEnergy(50);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[+] Випили протеїн! Енергію відновлено на 50.");
            Console.ResetColor();
        }
    }

    public class PrCampaign : Item
    {
        public override string Name => "Замовна стаття в ЗМІ (+50 Слави)";
        public override decimal Price => 1000m;

        public override void Apply(Player player)
        {
            player.ChangeReputation(50);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[+] Про вас говорять усі! Ваша Слава зросла на 50.");
            Console.ResetColor();
        }
    }

    public class PremiumRehab : Item
    {
        public override string Name => "Сеанс у приватній клініці (-2 тижні травми)";
        public override decimal Price => 2000m;

        public override void Apply(Player player)
        {
            if (!player.IsInjured)
            {
                throw new Exception("Ви повністю здорові! Клініка вам не потрібна.");
            }

            player.HealOneWeek();
            player.HealOneWeek();

            Console.ForegroundColor = ConsoleColor.Green;
            if (player.IsInjured)
                Console.WriteLine($"\n[+] Лікування допомогло! Залишилося хворіти: {player.WeeksInjured} тижнів.");
            else
                Console.WriteLine("\n[+] Дивовижно! Ви повністю вилікували травму!");
            Console.ResetColor();
        }
    }

    public class PsychologistSession : Item
    {
        public override string Name => "Сеанс у спортивного психолога (+30 Моралі)";
        public override decimal Price => 500m;

        public override void Apply(Player player)
        {
            if (player.Morale >= 100)
            {
                throw new Exception("У вас ідеальний настрій, психолог не потрібен!");
            }

            player.ChangeMorale(30);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[+] Психолог допоміг розібратися з думками! Мораль зросла до {player.Morale}.");
            Console.ResetColor();
        }
    }
}