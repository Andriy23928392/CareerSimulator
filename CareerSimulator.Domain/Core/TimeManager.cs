using CareerSimulator.Domain.Interfaces;
using CareerSimulator.Domain.Models;
using System;

namespace CareerSimulator.Domain.Core
{
    public class TimeManager
    {
        public int CurrentWeek { get; private set; }
        private readonly Player _player;

        public TimeManager(Player player)
        {
            _player = player;
            CurrentWeek = 1;
        }
        public void SetWeek(int week)
        {
            CurrentWeek = week;
        }

        public void ExecuteActivity(IActivity activity)
        {
            Console.WriteLine($"\n--- Тиждень {CurrentWeek} ---");
            Console.WriteLine($"Дія: {activity.Name}");

            try
            {
                activity.Execute(_player);

                CurrentWeek++;
            }
            catch (Exceptions.NotEnoughEnergyException ex)
            {
                Console.WriteLine($"ПОМИЛКА: {ex.Message}");
                Console.WriteLine("Тиждень не пропущено, виберіть іншу дію.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Щось пішло не так: {ex.Message}");
            }

        }
    }
}