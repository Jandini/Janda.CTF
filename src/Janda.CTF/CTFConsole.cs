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

        public static string Title { get; set; }

        public static void Maximize()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ShowWindow(_console, MAXIMIZE);
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            }
        }

        public static void PushTitle()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Title = Console.Title;
        }


        public static void PopTitle()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console.Title = Title;
        }

        public static void SetTitle(string title)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console.Title = title;
        }

    }
}
