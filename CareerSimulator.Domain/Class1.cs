namespace CareerSimulator.Domain.Models
{
    public enum Position
    {
        Goalkeeper,
        Defender,
        Midfielder,
        Forward
    }

    public class Player
    {
        public string Name { get; private set; }
        public Position PlayerPosition { get; private set; }

        public int Energy { get; private set; }
        public decimal Money { get; private set; }

        public int OverallRating { get; private set; }

        public Player(string name, Position position)
        {
            Name = name;
            PlayerPosition = position;


            Energy = 100;
            Money = 500m;
            OverallRating = 40;
        }

        public void SpendEnergy(int amount)
        {
            if (Energy - amount < 0)
            {
                throw new Exception("Недостатньо енергії для цієї дії!");
            }
            Energy -= amount;
        }

        public void Rest()
        {
            Energy = 100;
        }
    }
}