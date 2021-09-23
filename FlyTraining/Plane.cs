using System;
using System.Collections.Generic;
using FlyTraining.Enums;
using FlyTraining.EventArgs;

namespace FlyTraining
{
    public sealed class Plane
    {
        private static Plane MyPlane { get; set; }
        public ConsoleColor PlaneColor { get; } = ConsoleColor.Yellow;
        public PlaneState State { get; private set; }
        public int Height { get; private set; }
        public int Speed { get; private set; }

        public List<Dispatcher> Dispatchers { get; }
        public List<int> FlightSpeeds { get; }
        

        public event EventHandler<OutputEventArgs> OutPutEventHandler;
        private event EventHandler<FlightEventArgs> HeightSpeedEventHandler;

        private const int MaxSpeed = 1000;
        public bool ReachedMaxSpeed { get; set; }


        private Plane()
        {
            State = PlaneState.NotReady;
            FlightSpeeds = new List<int>(10);
            Dispatchers = new List<Dispatcher>(2);
        }
        public static Plane GetInstance()
        {
            if (MyPlane == null)
            {
                MyPlane = new Plane();
                MyPlane.Height = 0;//6750  7750   500
                MyPlane.Speed = 0;//950  1100  100
                //MyPlane.AddDispatcher(new Dispatcher("Matt", Utils.Utils.GetRandomDistinctColor()));
                //MyPlane.AddDispatcher(new Dispatcher("Sam", Utils.Utils.GetRandomDistinctColor()));
            }
            return MyPlane;
        }
        public void Start()
        {
            if (Dispatchers.Count >= 2)
            {
                State = PlaneState.Ready;
            }
            else
            {
                State = PlaneState.NotReady;
                var message =
                    $"Not enough dispatchers: minium 2 is required.{Environment.NewLine}Present: {Dispatchers.Count}";
                OutPutEventHandler?.Invoke(this, new OutputEventArgs(message));
            }
        }

        public void AddDispatcher(Dispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

            if (Dispatchers.Count == 1)
            {
                State = PlaneState.Ready;
            }
            HeightSpeedEventHandler += dispatcher.OnSpeedHeightChanged;
            Dispatchers.Add(dispatcher);
        }
        public void RemoveDispatcher(Dispatcher dispatcher)
        {
            HeightSpeedEventHandler -= dispatcher.OnSpeedHeightChanged;
            dispatcher.UnAttach();
            Dispatchers.Remove(dispatcher);
        }

        public void ChangeDirection(Direction direction, bool shiftHeld)
        {
            if (MyPlane.State == PlaneState.NotReady)
                return;
            switch (direction)
            {
                case Direction.Left:
                    if (Speed == 0){ return; }
                    Speed += shiftHeld ? Speed - 150 >= 0 ? -150 : 0
                        : Speed - 50 >= 0 ? -50 : 0;
                    break;
                case Direction.Right:
                    Speed += shiftHeld ? 150 : 50;
                    break;
                case Direction.Up:
                    if (Speed >= 50)
                    {
                        if (State != PlaneState.Landing)
                        {
                            Height += shiftHeld ? 500 : 250;
                        }
                    }
                    else return;
                    break;
                case Direction.Down:
                    Height += shiftHeld ? Height - 500 >= 0 ? -500 : 0
                        : Height - 250 >= 0 ? -250 : 0;
                    break;
            }
            NotifyDispatcher();
        }
        private void ChangeState()
        {
            if (Speed >= MaxSpeed)
            {
                ReachedMaxSpeed = true;
            }

            if (State == PlaneState.NotStarted || State == PlaneState.Ready)
            {
                if (Speed >= 0 && Speed <= 50)
                {
                    State = PlaneState.Starting;
                }
                else if (Height > 50 && Speed > 50)
                {
                    State = PlaneState.InAir;
                }
            }
            else if (State == PlaneState.Starting || State == PlaneState.Landing)
            {
                if (Height > 50 && Speed > 50)
                {
                    State = PlaneState.InAir;
                }
            }
            else if (State == PlaneState.InAir)
            {
                if(Speed<50)
                    State = PlaneState.Landing;
            }

            if (State == PlaneState.Landing)
            {
                if (Speed > 50)
                {
                    State = PlaneState.InAir;
                }
                else if (Height < 50)
                {
                    if (Speed < 50)
                    {
                        State = PlaneState.Landed;
                    }
                }
            }
        }
        private void NotifyDispatcher()
        {
            ChangeState();
            FlightSpeeds.Add(Speed);
            HeightSpeedEventHandler?.Invoke(this, new FlightEventArgs(State, Height, Speed, FlightSpeeds.Count,ReachedMaxSpeed));
        }
    }
}