using System;
using System.Windows.Forms;
using ShutDown.Models;

namespace ShutDown.MachineState
{
    public class StandbyExecutor : IExecutor
    {
        public void Execute(ShutDownOperation operation, bool force)
        {
            PowerState standbyPowerState;
            switch (operation)
            {
                case ShutDownOperation.Sleep:
                    standbyPowerState = PowerState.Suspend;
                    break;
                case ShutDownOperation.Hibernate:
                    standbyPowerState = PowerState.Hibernate;
                    break;
                default:
                    throw new InvalidOperationException("Only sleep and hibernate standby operations are supported.");
            }

            Application.SetSuspendState(standbyPowerState, force, false);
        }
    }
}
