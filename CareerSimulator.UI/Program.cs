using CareerSimulator.Domain.Activities;
using CareerSimulator.Domain.Core;
using CareerSimulator.Domain.Models;
using System;
using System.Numerics;

namespace CareerSimulator.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Вітаємо в симуляторі кар'єри футболіста!");
            Console.Write("Введіть ім'я вашого гравця: ");
            string playerName = Console.ReadLine();

            Player myPlayer = new Player(playerName, Position.Defender);
            TimeManager gameTime = new TimeManager(myPlayer);

            var training = new TrainingActivity();
            var rest = new RestActivity();

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n==============================");
                Console.WriteLine($"Статус гравця: {myPlayer.Name}");
                Console.WriteLine($"Енергія: {myPlayer.Energy} | Гроші: {myPlayer.Money}$ | Рейтинг: {myPlayer.OverallRating}");
                Console.WriteLine($"Поточний тиждень: {gameTime.CurrentWeek}");
                Console.WriteLine("==============================");

                Console.WriteLine("Оберіть дію на цей тиждень:");
                Console.WriteLine($"1. {training.Name} (-30 енергії)");
                Console.WriteLine($"2. {rest.Name} (Відновлює енергію)");
                Console.WriteLine("0. Вийти з гри");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        gameTime.ExecuteActivity(training);
                        break;
                    case "2":
                        gameTime.ExecuteActivity(rest);
                        break;
                    case "0":
                        isRunning = false;
                        Console.WriteLine("Дякуємо за гру!");
                        break;
                    default:
                        Console.WriteLine("Невідомий вибір, спробуйте ще раз.");
                        break;
                }
            }
        }
    }
}