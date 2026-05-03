using System;

namespace CareerSimulator.Domain.Models
{
    public class Player
    {
        public string Name { get; private set; }
        public Position PlayerPosition { get; private set; }

        public PlayerAttributes Attributes { get; private set; } = new PlayerAttributes();

        public int OverallRating
        {
            get
            {
                if (PlayerPosition == Position.Goalkeeper)
                {
                    return (int)(Attributes.GK_Diving * 0.2 + Attributes.GK_Reflexes * 0.2 +
                                 Attributes.GK_Handling * 0.2 + Attributes.GK_Positioning * 0.2 +
                                 Attributes.GK_Kicking * 0.1 + Attributes.GK_Speed * 0.1);
                }

                return PlayerPosition switch
                {
                    Position.Forward => (int)(Attributes.Shooting * 0.45 + Attributes.Pace * 0.2 + Attributes.Dribbling * 0.15 + Attributes.Passing * 0.1 + Attributes.Physical * 0.1),
                    Position.Midfielder => (int)(Attributes.Passing * 0.4 + Attributes.Dribbling * 0.25 + Attributes.Shooting * 0.15 + Attributes.Pace * 0.1 + Attributes.Physical * 0.1),
                    Position.Defender => (int)(Attributes.Defending * 0.5 + Attributes.Physical * 0.25 + Attributes.Pace * 0.15 + Attributes.Passing * 0.1),
                    _ => 50
                };
            }
        }

        public int Reputation { get; private set; } = 0;
        public int Energy { get; private set; }
        public Club CurrentClub { get; private set; }
        public decimal Money { get; private set; }
        public int Age { get; private set; }
        public int MatchesThisWeek { get; private set; } = 0;
        public int TrainingsThisWeek { get; private set; } = 0;
        public int Morale { get; private set; } = 60;

        public int SponsorIncome { get; private set; } = 0;
        public string Nationality { get; set; } = "Україна";
        public string SponsorName { get; private set; } = "Немає";
        public PlayerStats Stats { get; private set; } = new PlayerStats();
        public PlayerUpgrades Upgrades { get; private set; } = new PlayerUpgrades();
        public DateTime BirthDate { get; set; }
        public int MaxEnergy => 100 + Upgrades.MaxEnergyBonus;

        public int TrainingDiscount => Upgrades.TrainingDiscount;
        public int MatchDiscount => Upgrades.MatchDiscount;
        public int WinChanceBonus => Upgrades.WinChanceBonus;

        public int VillaLevel => Upgrades.VillaLevel;
        public int GearLevel => Upgrades.GearLevel;
        public int CryoLevel => Upgrades.CryoLevel;
        public int CoachLevel { get; set; }
        public int TrainingBonus { get; set; }
        public int CoachTrust { get; private set; } = 50;

        public bool IsInjured => WeeksInjured > 0;
        public int WeeksInjured { get; private set; } = 0;
        public string InjuryName { get; private set; } = "";

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
            Random rnd = new Random();
            DateTime start = new DateTime(2008, 9, 1);
            int range = (new DateTime(2009, 8, 31) - start).Days;
            BirthDate = start.AddDays(rnd.Next(range));

            DateTime gameStart = new DateTime(2025, 9, 1);
            int calculatedAge = gameStart.Year - BirthDate.Year;
            if (gameStart < BirthDate.AddYears(calculatedAge)) calculatedAge--;

            Age = calculatedAge;
            CurrentClub = new Club("ФК Збірна Університету", 0m, 10);

        }
        public int CalculateEnergyCost(int baseCost)
        {
            if (Morale < 30)
            {
                return (int)(baseCost * 1.5);
            }
            return baseCost;
        }

        public void SpendEnergy(int baseCost)
        {
            int amount = CalculateEnergyCost(baseCost);

            if (Energy >= amount)
            {
                Energy -= amount;
            }
            else
            {
                throw new Exception($"Недостатньо енергії! (Потрібно: {amount}, є: {Energy})");
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
            Attributes.Pace += delta;
            Attributes.Shooting += delta;
            Attributes.Passing += delta;
            Attributes.Dribbling += delta;
            Attributes.Defending += delta;
            Attributes.Physical += delta;

            Attributes.GK_Diving += delta;
            Attributes.GK_Handling += delta;
            Attributes.GK_Kicking += delta;
            Attributes.GK_Reflexes += delta;
            Attributes.GK_Speed += delta;
            Attributes.GK_Positioning += delta;
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

            Attributes.Pace = 0; Attributes.Shooting = 0; Attributes.Passing = 0;
            Attributes.Dribbling = 0; Attributes.Defending = 0; Attributes.Physical = 0;
            Attributes.GK_Diving = 0; Attributes.GK_Handling = 0; Attributes.GK_Kicking = 0;
            Attributes.GK_Reflexes = 0; Attributes.GK_Speed = 0; Attributes.GK_Positioning = 0;
            ChangeRating(rating);

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
        public void ChangeCoachTrust(int amount)
        {
            CoachTrust += amount;
            if (CoachTrust > 100) CoachTrust = 100;
            if (CoachTrust < 0) CoachTrust = 0;
        }

    }
}