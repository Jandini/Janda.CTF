using System;
using System.Runtime.InteropServices;

namespace Janda.CTF
{
    class CTFConsole
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static readonly IntPtr _console = GetConsoleWindow();
        private const int MAXIMIZE = 3;

        public void Maximize()
        {
            ShowWindow(_console, MAXIMIZE);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        }
    }
}
