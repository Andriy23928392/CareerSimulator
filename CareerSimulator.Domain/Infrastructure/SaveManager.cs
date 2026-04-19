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
                Energy = player.Energy,
                Money = player.Money,
                OverallRating = player.OverallRating,
                CurrentWeek = timeManager.CurrentWeek
            };

            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(SaveFilePath, json);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[Гру успішно збережено у файл savegame.json!]");
            Console.ResetColor();
        }

        public static (Player, TimeManager) LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                throw new FileNotFoundException("Файл збереження не знайдено!");
            }

            string json = File.ReadAllText(SaveFilePath);
            var state = JsonSerializer.Deserialize<GameState>(json);

            Player loadedPlayer = new Player(state.PlayerName, state.PlayerPosition);
            loadedPlayer.LoadState(state.Energy, state.Money, state.OverallRating);

            TimeManager loadedTime = new TimeManager(loadedPlayer);
            loadedTime.SetWeek(state.CurrentWeek);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[Гру успішно завантажено!]");
            Console.ResetColor();

            return (loadedPlayer, loadedTime);
        }
    }
}