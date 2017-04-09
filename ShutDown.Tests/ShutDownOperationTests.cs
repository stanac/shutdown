using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShutDown.Models;

namespace ShutDown.Tests
{
    [TestClass]
    public class ShutDownOperationTests
    {
        [TestMethod]
        public void GetOperationName_WhenSleepWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Sleep.GetOperationName(false);
            Assert.AreEqual("Sleep", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenSleepWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Sleep.GetOperationName(true);
            Assert.AreEqual("Sleep", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenHibernateWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Hibernate.GetOperationName(false);
            Assert.AreEqual("Hibernate", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenHibernateWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Hibernate.GetOperationName(true);
            Assert.AreEqual("Hibernate", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenSignOutWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.SignOut.GetOperationName(false);
            Assert.AreEqual("Log off", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenSignOutWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.SignOut.GetOperationName(true);
            Assert.AreEqual("Log off", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenShutdownWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.ShutDown.GetOperationName(false);
            Assert.AreEqual("Shut down", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenShutdownWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.ShutDown.GetOperationName(true);
            Assert.AreEqual("Forced shut down", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenRestartWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Restart.GetOperationName(false);
            Assert.AreEqual("Restart", actual);
        }

        [TestMethod]
        public void GetOperationName_WhenRestartWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Restart.GetOperationName(true);
            Assert.AreEqual("Forced restart", actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenRestartWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Restart.GetCommandLineArgs(false);
            Assert.AreEqual("-r -t 0", actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenRestartWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.Restart.GetCommandLineArgs(true);
            Assert.AreEqual("-r -f -t 0", actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenShutdownWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.ShutDown.GetCommandLineArgs(false);
            Assert.AreEqual("-s -t 0", actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenShutdownWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.ShutDown.GetCommandLineArgs(true);
            Assert.AreEqual("-s -f -t 0", actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenSignOutWithoutForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.SignOut.GetCommandLineArgs(false);
            Assert.AreEqual("-l -t 0", actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenSignOutWithForce_ReturnsAppropriateString()
        {
            var actual = ShutDownOperation.SignOut.GetCommandLineArgs(true);
            Assert.AreEqual("-l -f -t 0", actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenSleepWithoutForce_ReturnsEmptyString()
        {
            var actual = ShutDownOperation.Sleep.GetCommandLineArgs(false);
            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenSleepWithForce_ReturnsEmptyString()
        {
            var actual = ShutDownOperation.Sleep.GetCommandLineArgs(true);
            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenHibernateWithoutForce_ReturnsEmptyString()
        {
            var actual = ShutDownOperation.Hibernate.GetCommandLineArgs(false);
            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void GetCommandLineArgs_WhenHibernateWithForce_ReturnsEmptyString()
        {
            var actual = ShutDownOperation.Hibernate.GetCommandLineArgs(true);
            Assert.AreEqual(string.Empty, actual);
        }
    }
}
