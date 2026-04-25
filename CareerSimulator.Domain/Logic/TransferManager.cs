using CareerSimulator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CareerSimulator.Domain.Logic
{
    public static class TransferManager
    {
        private static readonly Random _random = new Random();

        private static readonly List<Club> _standardClubs = new List<Club>
        {
            new Club("Нива (Тернопіль)", 50m, 25),
            new Club("Оболонь (Київ)", 180m, 45, 25),
            new Club("Полісся (Житомир)", 500m, 60, 50),
            new Club("Динамо (Київ)", 1000m, 70, 150),
            new Club("Шахтар (Донецьк)", 1200m, 73, 150),

            new Club("Жирона (Іспанія)", 2000m, 76, 300),
            new Club("Галатасарай (Туреччина)", 2500m, 78, 500),
            new Club("Бенфіка (Португалія)", 2500m, 78, 500),
            new Club("Мілан (Італія)", 4000m, 84, 1000),
            new Club("Ювентус (Італія)", 4000m, 85, 1000),
            new Club("Атлетіко Мадрид", 5000m, 86, 1000),
            new Club("Челсі (Лондон)", 5000m, 86, 1000),
            new Club("Манчестер Юнайтед", 5000m, 87, 1000),
            new Club("Баварія (Мюнхен)", 8000m, 88, 2000),
            new Club("Ліверпуль", 8000m, 89, 2000),
            new Club("Манчестер Сіті", 8000m, 91, 2000),
            new Club("ПСЖ", 9000m, 92, 2000),
            new Club("Барселона", 10000m, 93, 2000)
        };

        private static readonly List<Club> _veteranClubs = new List<Club>
        {
            new Club("Лос-Анджелес ФК (США)", 15000m, 75, 5000),
            new Club("Інтер Маямі (США)", 18000m, 78, 5000),
            new Club("Аль-Іттіхад (СА)", 25000m, 80, 5000),
            new Club("Аль-Наср (СА)", 35000m, 82, 5000),
            new Club("Аль-Хіляль (СА)", 40000m, 83, 5000)
        };

        public static Club CheckForTransferOffers(Player player, DateTime currentDate)
        {
            bool isWinterWindow = currentDate.Month == 1;
            bool isSummerWindow = currentDate.Month == 7 || currentDate.Month == 8;

            if (!isWinterWindow && !isSummerWindow) return null;
            if (_random.Next(1, 101) > 20) return null;

            var availableClubs = _standardClubs
                .Where(c => player.OverallRating >= c.RequiredRating
                         && player.Reputation >= c.RequiredReputation
                         && c.Name != player.CurrentClub.Name)
                .ToList();

            if (player.Age >= 34)
            {
                var veteranOffers = _veteranClubs
                    .Where(c => player.OverallRating >= c.RequiredRating
                             && player.Reputation >= c.RequiredReputation
                             && c.Name != player.CurrentClub.Name)
                    .ToList();

                availableClubs.AddRange(veteranOffers);
            }

            var betterClubs = availableClubs
                .Where(c => c.WeeklySalary >= player.CurrentClub.WeeklySalary)
                .ToList();

            if (betterClubs.Count == 0) return null;

            var potentialClub = betterClubs[_random.Next(betterClubs.Count)];

            return potentialClub;
        }
        public static Club GetEmergencyTransfer(Player player)
        {
            var available = _standardClubs
                .Where(c => c.RequiredRating <= player.OverallRating &&
                            c.RequiredRating >= player.OverallRating - 10 &&
                            c.Name != player.CurrentClub.Name)
                .OrderByDescending(c => c.RequiredRating)
                .ToList();

            if (available.Count > 0)
                return available[0];

            return _standardClubs[0];
        }
    }
}