using System;
using System.Text;

namespace ShutDown.Models
{
    public class PatternModel
    {
        public string Name { get; set; }
        public ShutDownOperation Operation { get; set; }
        public bool Force { get; set; }
        public int DelayInMinutes { get; set; }

        public string ToSerializableString()
        {
            string escapedName = Convert.ToBase64String(Encoding.UTF8.GetBytes(Name));
            return $"p:{escapedName};{Operation};{Force};{DelayInMinutes}";
        }

        public static PatternModel Parse(string s)
        {
            var parts = s.Split(':')[1].Split(';');
            return new PatternModel
            {
                Name = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0])),
                Operation = (ShutDownOperation)Enum.Parse(typeof(ShutDownOperation), parts[1]),
                Force = bool.Parse(parts[2]),
                DelayInMinutes = int.Parse(parts[3])
            };
        }
    }
}
