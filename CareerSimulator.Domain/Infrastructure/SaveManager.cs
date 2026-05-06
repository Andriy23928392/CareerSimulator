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
        public string ClubName { get; set; } = string.Empty;
        public decimal ClubSalary { get; set; }
        public DateTime CurrentDate { get; set; }
        public int Age { get; set; }

        public int VillaLevel { get; set; }
        public int GearLevel { get; set; }
        public int CryoLevel { get; set; }
        public int CoachLevel { get; set; }
        public int AgentLevel { get; set; }

        public int Reputation { get; set; }
        public int TotalMatches { get; set; }
        public int RetirementAge { get; set; }

        public int TrainingBonus { get; set; }
        public int BallonDorAwards { get; set; }
        public int SeasonLeagueMatches { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }


        public int CoachTrust { get; set; }
        public int Morale { get; set; }
        public int SeasonGoals { get; set; }
        public int TotalGoals { get; set; }
        public int SeasonCleanSheets { get; set; }
        public Club? PreContractClub { get; set; }

        public PlayerAttributes? Attributes { get; set; }
    }

    public static class SaveManager
    {
        private const string SaveFilePath = "savegame.json";

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

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
                CurrentDate = timeManager.CurrentDate,
                ClubName = player.CurrentClub.Name,
                ClubSalary = player.CurrentClub.WeeklySalary,

                VillaLevel = player.VillaLevel,
                GearLevel = player.GearLevel,
                CryoLevel = player.CryoLevel,
                CoachLevel = player.CoachLevel,
                AgentLevel = player.AgentLevel,

                Reputation = player.Reputation,
                TotalMatches = player.Stats.TotalMatches,
                RetirementAge = player.Stats.RetirementAge,

                TrainingBonus = player.TrainingBonus,
                BallonDorAwards = player.Stats.BallonDorAwards,
                SeasonLeagueMatches = player.Stats.SeasonLeagueMatches,
                Nationality = player.Nationality,
                BirthDate = player.BirthDate,

                CoachTrust = player.CoachTrust,
                Morale = player.Morale,
                SeasonGoals = player.Stats.SeasonGoals,
                TotalGoals = player.Stats.TotalGoals,
                SeasonCleanSheets = player.Stats.SeasonCleanSheets,
                PreContractClub = player.PreContractClub,
                Attributes = player.Attributes
            };

            string json = JsonSerializer.Serialize(state, _jsonOptions);
            File.WriteAllText(SaveFilePath, json);

            Console.WriteLine("\n[Гру успішно збережено!]");
        }

        public static (Player, TimeManager) LoadGame()
        {
            if (!File.Exists(SaveFilePath)) throw new FileNotFoundException("Файл не знайдено!");

            string json = File.ReadAllText(SaveFilePath);
            var state = JsonSerializer.Deserialize<GameState>(json, _jsonOptions);

            if (state == null) throw new Exception("Помилка даних!");

            Player loadedPlayer = new Player(state.PlayerName, state.PlayerPosition);

            loadedPlayer.LoadState(
                state.Age, state.Energy, state.Money, state.OverallRating, state.ClubName, state.ClubSalary,
                state.VillaLevel, state.GearLevel, state.CryoLevel, state.Reputation, state.TotalMatches, state.RetirementAge
            );

            loadedPlayer.CoachLevel = state.CoachLevel;
            loadedPlayer.AgentLevel = state.AgentLevel;
            loadedPlayer.TrainingBonus = state.TrainingBonus;

            if (!string.IsNullOrEmpty(state.Nationality)) loadedPlayer.Nationality = state.Nationality;
            if (state.BirthDate != DateTime.MinValue) loadedPlayer.BirthDate = state.BirthDate;

            loadedPlayer.CoachTrust = state.CoachTrust == 0 ? 50 : state.CoachTrust;
            loadedPlayer.Morale = state.Morale == 0 ? 50 : state.Morale;

            loadedPlayer.Stats.BallonDorAwards = state.BallonDorAwards;
            loadedPlayer.Stats.SeasonLeagueMatches = state.SeasonLeagueMatches;
            loadedPlayer.Stats.SeasonGoals = state.SeasonGoals;
            loadedPlayer.Stats.TotalGoals = state.TotalGoals;
            loadedPlayer.Stats.SeasonCleanSheets = state.SeasonCleanSheets;

            loadedPlayer.PreContractClub = state.PreContractClub;
            if (state.Attributes != null)
            {
                loadedPlayer.Attributes = state.Attributes;
            }

            TimeManager loadedTime = new TimeManager(loadedPlayer);
            loadedTime.SetDate(state.CurrentDate);

            return (loadedPlayer, loadedTime);
        }
    }
}