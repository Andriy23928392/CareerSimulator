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
        // Базові характеристики
        public int Reputation { get; private set; } = 0;
        public int Energy { get; private set; }
        public Club CurrentClub { get; private set; }
        public decimal Money { get; private set; }
        public int BootsLevel { get; private set; } = 0;
        public int OverallRating { get; private set; }
        public int Age { get; private set; }
        public int SponsorIncome { get; private set; } = 0;
        public string SponsorName { get; private set; } = "Немає";
        // Прокачуванні характеристики
        public int MaxEnergy { get; private set; } = 100;
        public int TrainingChanceBonus { get; private set; } = 0;
        public int TrainingDiscount { get; private set; } = 0;
        public int MatchDiscount { get; private set; } = 0;
        public int WinChanceBonus { get; private set; } = 0;
        // Рівні Модифікаторів
        public int GymLevel { get; private set; } = 0;
        public int VillaLevel { get; private set; } = 0;
        public int GearLevel { get; private set; } = 0; 
        public int CryoLevel { get; private set; } = 0;
        public int MentalLevel { get; private set; } = 0;
        // Методи прокачки
        public void UpgradeGym() { GymLevel++; TrainingChanceBonus += 2; }
        public void UpgradeVilla() { VillaLevel++; MaxEnergy += 20; RestoreEnergy(MaxEnergy); }
        public void UpgradeGear() { GearLevel++; TrainingDiscount += 2; }
        public void UpgradeCryo() { CryoLevel++; MatchDiscount += 2; }
        public void UpgradeMental() { MentalLevel++; WinChanceBonus += 2; }

        public Player(string name, Position position)
        {
            Name = name;
            PlayerPosition = position;
            Energy = 100;
            Money = 500m;
            OverallRating = 40;
            Age = 18;
            CurrentClub = new Club("ФК Збірна Університету", 0m, 10);
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
        public void LoadState(int age, int energy, decimal money, int rating, string clubName, decimal clubSalary,
                              int gymLvl, int villaLvl, int gearLvl, int cryoLvl, int mentalLvl)
        {
            Age = age;
            Energy = energy;
            Money = money;
            OverallRating = rating;
            CurrentClub = new Club(clubName, clubSalary, 0);

            GymLevel = gymLvl; TrainingChanceBonus = gymLvl * 2;
            VillaLevel = villaLvl; MaxEnergy = 100 + (villaLvl * 20);
            GearLevel = gearLvl; TrainingDiscount = gearLvl * 2;
            CryoLevel = cryoLvl; MatchDiscount = cryoLvl * 2;
            MentalLevel = mentalLvl; WinChanceBonus = mentalLvl * 2;
        }
        public void RestoreEnergy(int amount)
        {
            Energy += amount;
            if (Energy > MaxEnergy)
                Energy = MaxEnergy; 
        }

        public void LoadState(int age, int energy, decimal money, int rating, string clubName, decimal clubSalary,
                              int gymLvl, int villaLvl, int gearLvl, int cryoLvl, int mentalLvl, int reputation)
        {
            Age = age;
            Energy = energy;
            Money = money;
            OverallRating = rating;
            CurrentClub = new Club(clubName, clubSalary, 0);

            // Відновлюємо Модифікатори та їхні бонуси
            GymLevel = gymLvl; TrainingChanceBonus = gymLvl * 2;
            VillaLevel = villaLvl; MaxEnergy = 100 + (villaLvl * 20);
            GearLevel = gearLvl; TrainingDiscount = gearLvl * 2;
            CryoLevel = cryoLvl; MatchDiscount = cryoLvl * 2;
            MentalLevel = mentalLvl; WinChanceBonus = mentalLvl * 2;
            // Репутація
            Reputation = reputation;
        }
        public void DecreaseEnergy(int amount)
        {
            Energy -= amount;
            if (Energy < 0) Energy = 0;
        }
        public void SignContract(Club newClub)
        {
            CurrentClub = newClub;
        }
        public void HaveBirthday()
        {
            Age++;
        }
        public void ChangeReputation(int amount)
        {
            Reputation += amount;
            if (Reputation < 0) Reputation = 0;
        }
        public void SignSponsorship(string brand, int weeklyIncome)
        {
            SponsorName = brand;
            SponsorIncome = weeklyIncome;
        }
    }
}