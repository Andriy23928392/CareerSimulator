using System;

namespace CareerSimulator.Domain.Models
{
    public class PlayerStats
    {
        public int SeasonLeagueMatches { get; set; }
        public int TotalMatches { get; private set; } = 0;
        public int RetirementAge { get; private set; }
        public int SeasonMatches { get; set; }
        public int SeasonGoals { get; set; }
        public int SeasonAssists { get; set; }
        public int SeasonCleanSheets { get; set; }
        public int SeasonWins { get; set; }
        public int SeasonDraws { get; set; }
        public int SeasonLosses { get; set; }
        public List<string> Trophies { get; set; } = new List<string>();


        public int TotalGoals { get; set; } = 0;
        public int TotalAssists { get; set; } = 0;
        public int TotalCleanSheets { get; set; } = 0;
        public int TotalPenaltiesSaved { get; set; } = 0;
        public int BallonDorAwards { get; set; }


        public PlayerStats()
        {
            RetirementAge = new Random().Next(35, 42);
        }
        public void ResetSeasonStats()
        {
            SeasonMatches = 0;
            SeasonGoals = 0;
            SeasonAssists = 0;
            SeasonCleanSheets = 0;
            SeasonLeagueMatches = 0;
        }

        public void RecordMatchStats(int goals, int assists, int cleanSheets, int penaltiesSaved)
        {
            TotalMatches++;
            TotalGoals += goals;
            TotalAssists += assists;
            TotalCleanSheets += cleanSheets;

            SeasonMatches++;
            SeasonGoals += goals;
            SeasonAssists += assists;
            SeasonCleanSheets += cleanSheets;
        }

        public void LoadStats(int matches, int retirementAge, int goals = 0, int assists = 0, int cleanSheets = 0, int penalties = 0)
        {
            TotalMatches = matches;
            RetirementAge = retirementAge;
            TotalGoals = goals;
            TotalAssists = assists;
            TotalCleanSheets = cleanSheets;
            TotalPenaltiesSaved = penalties;
        }
    }
}