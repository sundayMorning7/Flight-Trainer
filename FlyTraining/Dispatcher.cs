using System;
using System.Collections.Generic;
using FlyTraining.Enums;
using FlyTraining.EventArgs;

namespace FlyTraining
{
    public sealed class Dispatcher
    {
        public string Name { get; }
        private int WeatherFix { get; } = GenerateWeatherFix(Random);
        public event EventHandler<RecomendedHeightEventArgs> RecomendedHeightEventHandler;

        public Dictionary<int, int> RecomendedHeight { get; }
        public int PenaltyPoints { get; private set; }

        public ConsoleColor DispatcherColor { get; }
        private const int MaxSpeed = 1000;
        private const int MaxPenaltyPoints = 1000;
        private static readonly Random Random;

        public Dispatcher(string name, ConsoleColor dispatcherDispatcherColor,int penaltyPoints = 0)
        {
            Name = name;
            DispatcherColor = dispatcherDispatcherColor;
            RecomendedHeight = new Dictionary<int, int>(10);
            PenaltyPoints = penaltyPoints;
        }
        static Dispatcher()
        {
            Random = new Random();
        }

        private static int GenerateWeatherFix(Random random)
        {
            int min = -200;
            int max = 200;

            return random.Next(min, max);
        }
        public void UnAttach()
        {
            RecomendedHeightEventHandler = null;
        }
        public void OnSpeedHeightChanged(object sender, FlightEventArgs args)
        {
            if (args.State == PlaneState.Starting || args.State == PlaneState.Landing ||
                args.State == PlaneState.NotReady || args.State == PlaneState.Ready)
            {
                return;
            }

            if (args.State == PlaneState.InAir)
            {
                int recomendedHeight = 7 * args.Speed - WeatherFix;
                recomendedHeight = recomendedHeight < 0 ? 0 : recomendedHeight;

                int absDifferenceHeight = Math.Abs(recomendedHeight - args.Height);
                if (absDifferenceHeight > 300 && absDifferenceHeight < 600)
                {
                    PenaltyPoints += 25;
                }
                else if (absDifferenceHeight > 600 && absDifferenceHeight < 1000)
                {
                    PenaltyPoints += 50;
                }
                else if (absDifferenceHeight > 1000)
                {
                    throw new InvalidOperationException("Plain crashed!");
                }

                if (args.Speed == 0 && args.Height == 0)
                {
                    throw new InvalidOperationException("Plain crashed!");
                }
                if (args.Speed > MaxSpeed)
                {
                    PenaltyPoints += 100;
                    RecomendedHeightEventHandler?.Invoke(this,
                        new RecomendedHeightEventArgs(recomendedHeight,
                            $"{Name}: Immediately decrease your speed!"));
                }
                else
                {
                    RecomendedHeightEventHandler?.Invoke(this, new RecomendedHeightEventArgs(recomendedHeight));
                }
                RecomendedHeight.Add(args.DirectionChangedTimes, recomendedHeight);
            }
            if (PenaltyPoints > MaxPenaltyPoints)
            {
                throw new InvalidOperationException("Unfit to fly!");
            }

            if (args.Speed == 0 && args.Height == 0)
            {
                if (args.ReachedMaxSpeed)
                {
                    RecomendedHeightEventHandler = null;
                }
                else
                {
                    throw new InvalidOperationException("Plain crashed!");
                }
            }
        }
    }
}