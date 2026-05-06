using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Items
{
    public abstract class Item
    {
        public abstract string Name { get; }
        public abstract decimal Price { get; }
        public bool IsConsumable { get; protected set; } = true;
        public abstract void Apply(Player player);
    }
}