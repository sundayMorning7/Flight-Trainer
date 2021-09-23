namespace FlyTraining.EventArgs
{
    public class OutputEventArgs
    {
        public string Message { get; }

        public OutputEventArgs(string message)
        {
            Message = message;
        }
    }
}