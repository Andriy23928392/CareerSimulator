using CareerSimulator.Domain.Models;
using CareerSimulator.Domain.Core;
using System.Text.Json;
using System.IO;
using System;

namespace CareerSimulator.Domain.Infrastructure
{
    public class GameState
    {
        public string PlayerName { get; set; } = string.Empty;
        public Position PlayerPosition { get; set; }
        public int Energy { get; set; }
        public decimal Money { get; set; }
        public int OverallRating { get; set; }
        public int CurrentWeek { get; set; }
        public int BootsLevel { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public decimal ClubSalary { get; set; }
        public DateTime CurrentDate { get; set; }
        public int Age { get; set; }
    }

    public static class SaveManager
    {
        private const string SaveFilePath = "savegame.json";

        public static void SaveGame(Player player, TimeManager timeManager)
        {
            var state = new GameState
            {
                PlayerName = player.Name,
                PlayerPosition = player.PlayerPosition,
                Age = player.Age,
                Energy = player.Energy,
                Money = player.Money,
                OverallRating = player.OverallRating,
                CurrentWeek = timeManager.CurrentWeek,
                BootsLevel = player.BootsLevel,
                CurrentDate = timeManager.CurrentDate,
                ClubName = player.CurrentClub.Name,
                ClubSalary = player.CurrentClub.WeeklySalary
            };

            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFilePath, json);

            Console.WriteLine("\n[Гру успішно збережено!]");
        }

        public static (Player, TimeManager) LoadGame()
        {
            if (!File.Exists(SaveFilePath)) throw new FileNotFoundException("Файл не знайдено!");

            string json = File.ReadAllText(SaveFilePath);
            var state = JsonSerializer.Deserialize<GameState>(json);

            if (state == null) throw new Exception("Помилка даних!");

            Player loadedPlayer = new Player(state.PlayerName, state.PlayerPosition);
            loadedPlayer.LoadState(state.Age, state.Energy, state.Money, state.OverallRating, state.BootsLevel, state.ClubName, state.ClubSalary);

            TimeManager loadedTime = new TimeManager(loadedPlayer);
            loadedTime.SetDate(state.CurrentDate);

            return (loadedPlayer, loadedTime);
        }
    }
}