using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Items
{
    public abstract class Item
    {
        public string Name { get; protected set; } = string.Empty;
        public decimal Price { get; protected set; }

        public abstract void Apply(Player player);
    }
}