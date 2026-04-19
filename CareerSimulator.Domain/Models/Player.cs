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
        public int BootsLevel { get; private set; } = 0;

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
                throw new Domain.Exceptions.NotEnoughEnergyException($"Недостатньо енергії! Потрібно {amount}, а є {Energy}.");
            }
            Energy -= amount;
        }

        public void Rest()
        {
            Energy = 100;
        }

        public void EarnMoney(decimal amount)
        {
            Money += amount;
        }

        public void SpendMoney(decimal amount)
        {
            if (Money - amount < 0)
                throw new Exception("Недостатньо грошей!");
            Money -= amount;
        }

        public void ChangeRating(int delta)
        {
            OverallRating += delta;
            if (OverallRating < 1) OverallRating = 1;
        }
        public void LoadState(int energy, decimal money, int rating)
        {
            Energy = energy;
            Money = money;
            OverallRating = rating;
        }
        public void RestoreEnergy(int amount)
        {
            Energy += amount;
            if (Energy > 100)
                Energy = 100;
        }

        public void LoadState(int energy, decimal money, int rating, int bootsLevel)
        {
            Energy = energy;
            Money = money;
            OverallRating = rating;
            BootsLevel = bootsLevel;
        }

        public void UpgradeBoots()
        {
            BootsLevel++;
        }
        public void DecreaseEnergy(int amount)
        {
            Energy -= amount;
            if (Energy < 0) Energy = 0;
        }
    }
}