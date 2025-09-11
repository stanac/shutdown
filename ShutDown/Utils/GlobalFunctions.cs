using System;
using ShutDown.Models;

namespace ShutDown.Utils
{
    public class GlobalFunctions
    {
        public static GlobalFunctions Instance { get; } = new GlobalFunctions();

        public Action HideSettingsView { get; set; } = () => { };
        public Action NewVersionCheckRequested { get; set; } = () => { };
        public Action PatternListChanged { get; set; } = () => { };
        public Action<Guid> StartFromPattern { get; set; } = _ => { };
        public Action ShowNewPatternView { get; set; } = () => { };
        public Action HideNewPatternView { get; set; } = () => { };
        public Func<OperationModel> GetCurrentOperation { get; set; } = () => null;
        public Action HideCurrentOperationView { get; set; } = () => { };
        public Action StartOperation { get; set; } = () => { };
        public Action ExitApp { get; set; } = () => { };
        public Func<bool> IsShutDownInProgress { get; set; } = () => false;
        public Action CancelOperationRequested { get; set; } = () => { };

        private GlobalFunctions()
        {
        }
    }
}
