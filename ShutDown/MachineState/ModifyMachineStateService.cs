using System;
using System.Diagnostics;
using ShutDown.Data;
using ShutDown.Models;

namespace ShutDown.MachineState
{
    public class ModifyMachineStateService : IModifyMachineStateService
    {
        private readonly IExecutor _shutdownExecutor;
        private readonly IExecutor _standbyExecutor;

        public ModifyMachineStateService(IExecutor shutdownExecutor, IExecutor standbyExecutor)
        {
            _shutdownExecutor = shutdownExecutor;
            _standbyExecutor = standbyExecutor;
        }

        public void ModifyMachineState(ShutDownOperation operation, bool force)
        {
            try
            {
                //The shutdown command does not support sleep.  Use the psshutdown command instead
                if (operation != ShutDownOperation.Sleep && operation != ShutDownOperation.Hibernate)
                {
                    _shutdownExecutor.Execute(operation, force);
                }
                else
                {
                    _standbyExecutor.Execute(operation, force);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                string operationName = force ? "forced " : "not forced ";
                operationName += operation.ToString().ToLower();
                Log.LogErrorAndDisplayMessageBox($"Failed to execute {operation}", ex);
            }
        }
    }
}
