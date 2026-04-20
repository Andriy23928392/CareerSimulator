using CareerSimulator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CareerSimulator.Domain.Logic
{
    public static class TransferManager
    {
        private static readonly Random _random = new Random();

        // Основні клуби (УПЛ та Європа)
        private static readonly List<Club> _standardClubs = new List<Club>
        {
            // Україна
            new Club("Нива (Тернопіль)", 50m, 25),
            new Club("Оболонь (Київ)", 180m, 45, 25),
            new Club("Полісся (Житомир)", 500m, 60, 50),
            new Club("Динамо (Київ)", 1000m, 70, 150),
            new Club("Шахтар (Донецьк)", 1200m, 73, 150),

            // Європа (Різні рівні)
            new Club("Галатасарай (Туреччина)", 1800m, 76, 500),
            new Club("Бенфіка (Португалія)", 2200m, 78, 500),
            new Club("Жирона (Іспанія)", 2500m, 80, 500),
            new Club("Мілан (Італія)", 4000m, 84, 1000),
            new Club("Ювентус (Італія)", 4500m, 85, 1000),
            new Club("Челсі (Лондон)", 5000m, 86, 1000),
            new Club("Манчестер Юнайтед", 6000m, 87, 1000),
            new Club("Баварія (Мюнхен)", 7500m, 88, 2000),
            new Club("Ліверпуль", 8000m, 89, 2000),
            new Club("Манчестер Сіті", 9500m, 91, 2000),
            new Club("ПСЖ", 10000m, 92, 2000),
            new Club("Барселона", 11000m, 93, 2000)
        };

        // Клуби для завершення кар'єри (Тільки для гравців 34+ років)
        private static readonly List<Club> _veteranClubs = new List<Club>
        {
            new Club("Лос-Анджелес ФК (США)", 15000m, 75, 5000),
            new Club("Інтер Маямі (США)", 18000m, 78, 5000),
            new Club("Аль-Іттіхад (СА)", 25000m, 80, 5000),
            new Club("Аль-Наср (СА)", 35000m, 82, 5000),
            new Club("Аль-Хіляль (СА)", 40000m, 83, 5000)
        };

        public static Club? CheckForTransferOffers(Player player, DateTime currentDate)
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
                availableClubs.AddRange(_veteranClubs
                    .Where(c => player.OverallRating >= c.RequiredRating && c.Name != player.CurrentClub.Name));
            }

            if (availableClubs.Count == 0) return null;

            return availableClubs[_random.Next(availableClubs.Count)];
        }
    }
}