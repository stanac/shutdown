using ShutDown.Models;

namespace ShutDown.MachineState
{
    public interface IModifyMachineStateService
    {
        void ModifyMachineState(ShutDownOperation operation, bool force);
    }
}