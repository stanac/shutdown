namespace ShutDown.Models
{
    public class OperationModel
    {
        public ShutDownOperation Operation { get; set; }
        public bool Force { get; set; }
        public int DelayInMinutes { get; set; }
    }
}