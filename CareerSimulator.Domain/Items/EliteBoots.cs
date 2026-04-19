using CareerSimulator.Domain.Items;
using CareerSimulator.Domain.Models;
using System.Xml.Linq;

namespace CareerSimulator.Domain.Items
{
    public class EliteBoots : Item
    {
        public EliteBoots(int currentLevel)
        {
            Name = $"Елітні бутси (Рівень {currentLevel + 1}) (+2 до рейтингу)";
            Price = 500m + (currentLevel * 300m);
        }

        public override void Apply(Player player)
        {
            player.ChangeRating(2);
            player.UpgradeBoots();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nВи оновили бутси! Тепер у вас рівень {player.BootsLevel}.");
            Console.ResetColor();
        }
    }
}