namespace CareerSimulator.Domain.Models
{
    public class Club
    {
        public string Name { get; private set; }
        public decimal WeeklySalary { get; private set; }
        public int RequiredRating { get; private set; }

        public Club(string name, decimal weeklySalary, int requiredRating)
        {
            Name = name;
            WeeklySalary = weeklySalary;
            RequiredRating = requiredRating;
        }
    }
}