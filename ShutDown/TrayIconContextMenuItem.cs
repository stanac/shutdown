using System;

namespace ShutDown
{
    public class TrayIconContextMenuItem
    {
        public string Text { get; set; }
        public Action OnClick { get; set; }
    }
}
