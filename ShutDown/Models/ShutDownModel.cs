using System;

namespace ShutDown.Models
{
    public class ShutDownModel
    {
        public ShutDownOperation Operation { get; set; }
        public TimeSpan Delay { get; set; }
    }
}
