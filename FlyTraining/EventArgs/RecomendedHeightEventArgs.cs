namespace FlyTraining.EventArgs
{
    public class RecomendedHeightEventArgs
    {
        public int RecomendedHeight { get; }
        public string SpecialMessage { get; }
        public RecomendedHeightEventArgs(int recomendedHeight,string mSpecialMessage = null)
        {
            RecomendedHeight = recomendedHeight;
            SpecialMessage = mSpecialMessage;
        }
    }
}