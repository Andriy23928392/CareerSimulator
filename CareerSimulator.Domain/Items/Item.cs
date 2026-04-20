using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Items
{
    public abstract class Item
    {
        public abstract string Name { get; }
        public abstract decimal Price { get; }
        public abstract void Apply(Player player);
    }
}