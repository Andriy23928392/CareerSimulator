using System;

namespace CareerSimulator.Domain.Models
{
    public class PlayerUpgrades
    {
        public int VillaLevel { get; private set; } = 0;
        public int GearLevel { get; private set; } = 0;
        public int CryoLevel { get; private set; } = 0;
        public int MentalLevel { get; private set; } = 0;

        public int MaxEnergyBonus => VillaLevel * 20;
        public int TrainingDiscount => GearLevel * 2;
        public int MatchDiscount => CryoLevel * 2;
        public int WinChanceBonus => MentalLevel * 2;

        public void UpgradeVilla() => VillaLevel++;
        public void UpgradeGear() => GearLevel++;
        public void UpgradeCryo() => CryoLevel++;
        public void UpgradeMental() => MentalLevel++;

        public void LoadUpgrades(int gym, int villa, int gear, int cryo, int mental)
        {
            VillaLevel = villa;
            GearLevel = gear;
            CryoLevel = cryo;
            MentalLevel = mental;
        }
    }
}