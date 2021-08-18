using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using ShutDown.Data;
using ShutDown.User32;

namespace ShutDown
{
    public static class MouseJigglerHelper
    {
        private static bool _started;
        
        public static void Start()
        {
            if (_started)
            {
                return;
            }

            _started = true;

            var timer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromMinutes(3).TotalMilliseconds
            };

            timer.Elapsed += OnTick;

            timer.Start();
        }

        private static void OnTick(object s, ElapsedEventArgs e)
        {
            if (!Settings.Instance.JiggleMouse)
            {
                return;
            }

            bool gotPosition = User32.User32.GetCursorPos(out User32.POINT currentPosition);

            Jiggle(5, 0);
            Thread.Sleep(5);

            Jiggle(0, 5);
            Thread.Sleep(5);
            
            Jiggle(-5, 0);
            Thread.Sleep(5);
            
            Jiggle(0, -5);
            Thread.Sleep(5);

            if (gotPosition)
            {
                User32.User32.SetCursorPos(currentPosition);
            }
        }

        private static void Jiggle(int deltaX, int deltaY)
        {
            INPUT[] input = new[]
            {
                new User32.INPUT
                {
                    type = 0, // mouse
                    U = new User32.InputUnion
                    {
                        mi = new User32.MOUSEINPUT
                        {
                            dx = deltaX,
                            dy = deltaY,
                            mouseData = 0,
                            dwFlags = User32.MOUSEEVENTF.MOVE,
                            time = 0,
                            dwExtraInfo = UIntPtr.Zero,
                        },
                    },
                }
            };

            User32.User32.SendInput(1, input, Marshal.SizeOf<User32.INPUT>());
        }
    }
}
