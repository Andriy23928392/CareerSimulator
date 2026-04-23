using System;

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
        public int OverallRating { get; private set; }
        public int Age { get; private set; }
        public int MatchesThisWeek { get; private set; } = 0;
        public int TrainingsThisWeek { get; private set; } = 0;
        public int Morale { get; private set; } = 60;

        public int SponsorIncome { get; private set; } = 0;
        public string SponsorName { get; private set; } = "Немає";
        public PlayerStats Stats { get; private set; } = new PlayerStats();
        public PlayerUpgrades Upgrades { get; private set; } = new PlayerUpgrades();

        // Динамічна властивість (рахується автоматично)
        public int MaxEnergy => 100 + Upgrades.MaxEnergyBonus;

        public int TrainingChanceBonus => Upgrades.TrainingChanceBonus;
        public int TrainingDiscount => Upgrades.TrainingDiscount;
        public int MatchDiscount => Upgrades.MatchDiscount;
        public int WinChanceBonus => Upgrades.WinChanceBonus;

        public int GymLevel => Upgrades.GymLevel;
        public int VillaLevel => Upgrades.VillaLevel;
        public int GearLevel => Upgrades.GearLevel;
        public int CryoLevel => Upgrades.CryoLevel;
        public int MentalLevel => Upgrades.MentalLevel;
        // --- ТРАВМИ ---
        public bool IsInjured => WeeksInjured > 0;
        public int WeeksInjured { get; private set; } = 0;
        public string InjuryName { get; private set; } = "";

        public void UpgradeGym() => Upgrades.UpgradeGym();
        public void UpgradeVilla() { Upgrades.UpgradeVilla(); RestoreEnergy(MaxEnergy); }
        public void UpgradeGear() => Upgrades.UpgradeGear();
        public void UpgradeCryo() => Upgrades.UpgradeCryo();
        public void UpgradeMental() => Upgrades.UpgradeMental();
        public void AddMatchThisWeek() => MatchesThisWeek++;
        public void AddTrainingThisWeek() => TrainingsThisWeek++;


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
            if (Morale < 30)
            {
                amount = (int)(amount * 1.5);
            }

            if (Energy >= amount)
            {
                Energy -= amount;
            }
            else
            {
                throw new Exception("Недостатньо енергії!");
            }
        }

        public void DecreaseEnergy(int amount)
        {
            Energy -= amount;
            if (Energy < 0) Energy = 0;
        }

        public void RestoreEnergy(int amount)
        {
            Energy += amount;
            if (Energy > MaxEnergy)
                Energy = MaxEnergy;
        }

        public void EarnMoney(decimal amount) { Money += amount; }

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

        public void ChangeReputation(int amount)
        {
            Reputation += amount;
            if (Reputation < 0) Reputation = 0;
        }

        public void SignContract(Club newClub) { CurrentClub = newClub; }

        public void SignSponsorship(string brand, int weeklyIncome)
        {
            SponsorName = brand;
            SponsorIncome = weeklyIncome;
        }

        public void SetAge(int newAge) { Age = newAge; }

        public void LoadState(int age, int energy, decimal money, int rating, string clubName, decimal clubSalary,
                              int gymLvl, int villaLvl, int gearLvl, int cryoLvl, int mentalLvl, int reputation,
                              int totalMatches, int retirementAge)
        {
            Age = age;
            Energy = energy;
            Money = money;
            OverallRating = rating;
            CurrentClub = new Club(clubName, clubSalary, 0);
            Reputation = reputation;

            Stats.LoadStats(totalMatches, retirementAge);
            Upgrades.LoadUpgrades(gymLvl, villaLvl, gearLvl, cryoLvl, mentalLvl);
        }
        public void SufferInjury(string name, int durationInWeeks)
        {
            WeeksInjured = durationInWeeks;
            InjuryName = name;
            Energy = 30;
        }

        public void HealOneWeek()
        {
            if (WeeksInjured > 0)
            {
                WeeksInjured--;
                if (WeeksInjured == 0) InjuryName = "";
            }
        }
        public void ResetWeeklyLimits()
        {
            MatchesThisWeek = 0;
            TrainingsThisWeek = 0;
        }
        public void ChangeMorale(int amount)
        {
            Morale += amount;
            if (Morale > 100) Morale = 100;
            if (Morale < 0) Morale = 0;
        }
    }
}