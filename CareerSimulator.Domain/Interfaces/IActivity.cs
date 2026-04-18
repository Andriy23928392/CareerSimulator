using CareerSimulator.Domain.Models;

namespace CareerSimulator.Domain.Interfaces
{
    public interface IActivity
    {
        string Name { get; }
        string Description { get; }

        void Execute(Player player);
    }
}