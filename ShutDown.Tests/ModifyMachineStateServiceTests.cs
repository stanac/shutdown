using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShutDown.MachineState;
using ShutDown.Models;

namespace ShutDown.Tests
{
    [TestClass]
    public class ModifyMachineStateServiceTests
    {
        private Mock<IExecutor> _mockShutdownExecutor;
        private Mock<IExecutor> _mockStandbyExecutor;
        private ModifyMachineStateService _modifyMachineStateService;

        [TestInitialize]
        public void Setup()
        {
            _mockShutdownExecutor = new Mock<IExecutor>();
            _mockStandbyExecutor = new Mock<IExecutor>();

            _modifyMachineStateService = new ModifyMachineStateService(_mockShutdownExecutor.Object, _mockStandbyExecutor.Object);
        }

        [TestMethod]
        public void ModifyMachineState_WhenOperationIsSleep_CallStandbyExecutorExecute()
        {
            var shutDownOperation = ShutDownOperation.Sleep;
            var force = false;
            _modifyMachineStateService.ModifyMachineState(shutDownOperation, force);

            _mockStandbyExecutor.Verify(x => x.Execute(shutDownOperation, force), Times.Once);
            _mockShutdownExecutor.Verify(x => x.Execute(It.IsAny<ShutDownOperation>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void ModifyMachineState_WhenOperationIsHibernate_CallStandbyExecutorExecute()
        {
            var shutDownOperation = ShutDownOperation.Hibernate;
            var force = false;
            _modifyMachineStateService.ModifyMachineState(shutDownOperation, force);

            _mockStandbyExecutor.Verify(x => x.Execute(shutDownOperation, force), Times.Once);
            _mockShutdownExecutor.Verify(x => x.Execute(It.IsAny<ShutDownOperation>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void ModifyMachineState_WhenOperationIsShutdown_CallShutdownExecutorExecute()
        {
            var shutDownOperation = ShutDownOperation.ShutDown;
            var force = false;
            _modifyMachineStateService.ModifyMachineState(shutDownOperation, force);

            _mockShutdownExecutor.Verify(x => x.Execute(shutDownOperation, force), Times.Once);
            _mockStandbyExecutor.Verify(x => x.Execute(It.IsAny<ShutDownOperation>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void ModifyMachineState_WhenOperationIsRestart_CallShutdownExecutorExecute()
        {
            var shutDownOperation = ShutDownOperation.Restart;
            var force = false;
            _modifyMachineStateService.ModifyMachineState(shutDownOperation, force);

            _mockShutdownExecutor.Verify(x => x.Execute(shutDownOperation, force), Times.Once);
            _mockStandbyExecutor.Verify(x => x.Execute(It.IsAny<ShutDownOperation>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void ModifyMachineState_WhenOperationIsSignOut_CallShutdownExecutorExecute()
        {
            var shutDownOperation = ShutDownOperation.SignOut;
            var force = false;
            _modifyMachineStateService.ModifyMachineState(shutDownOperation, force);

            _mockShutdownExecutor.Verify(x => x.Execute(shutDownOperation, force), Times.Once);
            _mockStandbyExecutor.Verify(x => x.Execute(It.IsAny<ShutDownOperation>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
