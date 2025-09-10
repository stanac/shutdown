using System;

namespace ShutDown.Utils
{
    public class Mediator
    {
        public static Mediator Instance { get; } = new Mediator();

        public Action HideSettingsView { get; set; } = () => { };
        public Action NewVersionCheckRequested { get; set; } = () => { };

        private Mediator()
        {
        }
    }
}
