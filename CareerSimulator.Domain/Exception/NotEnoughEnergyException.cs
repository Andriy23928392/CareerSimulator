using System;

namespace CareerSimulator.Domain.Exceptions
{
    public class NotEnoughEnergyException : Exception
    {
        public NotEnoughEnergyException(string message) : base(message)
        {
        }
    }
}