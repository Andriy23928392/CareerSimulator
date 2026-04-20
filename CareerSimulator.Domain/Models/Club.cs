namespace CareerSimulator.Domain.Models
{
    public class Club
    {
        public string Name { get; set; }
        public decimal WeeklySalary { get; set; }
        public int RequiredRating { get; set; }

        public int RequiredReputation { get; set; }

        public Club(string name, decimal weeklySalary, int requiredRating, int requiredReputation = 0)
        {
            Name = name;
            WeeklySalary = weeklySalary;
            RequiredRating = requiredRating;
            RequiredReputation = requiredReputation;
        }
    }
}