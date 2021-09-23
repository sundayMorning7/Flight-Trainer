using FlyTraining.Enums;

namespace FlyTraining.EventArgs
{
    public class FlightEventArgs
    {
        public int Height { get; }
        public int Speed { get; }
        public bool ReachedMaxSpeed { get; }

        public int DirectionChangedTimes { get; }
        public PlaneState State { get; }
        public FlightEventArgs(PlaneState st,int height, int speed,int directionChangedTimes,bool reachedMaxSpeed)
        {
            Speed = speed;
            Height = height;
            State = st;
            DirectionChangedTimes = directionChangedTimes;
            ReachedMaxSpeed = reachedMaxSpeed;
        }
    }
}