using ShutDown.Models;

namespace ShutDown.MachineState
{
    public interface IExecutor
    {
        void Execute(ShutDownOperation operation, bool force);
    }
}
