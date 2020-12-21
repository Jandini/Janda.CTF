using System;

namespace Janda.CTF
{
    public class CTFAttribute : Attribute
    {
        public string Name { get; set; }
        public string LogDirectory { get; set; } = "Logs";
        public string LogFileExtension { get; set; } = "log";
    }
}
