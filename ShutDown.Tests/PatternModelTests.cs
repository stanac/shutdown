using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShutDown.Models;

namespace ShutDown.Tests
{
    [TestClass]
    public class PatternModelTests
    {
        [TestMethod]
        public void CanSerializeAndDeserializeModel()
        {
            var pattern = new PatternModel
            {
                DelayInMinutes = 30,
                Force = true,
                Name = "SD",
                Operation = ShutDownOperation.ShutDown
            };
            string s = pattern.ToSerializableString();
            var deserialized = PatternModel.Parse(s);
            Assert.AreEqual(pattern.Description, deserialized.Description);
        }
    }
}
