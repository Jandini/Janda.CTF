using System;

namespace Janda.CTF
{
    public class ChallengeLoggingAttribute : Attribute
    {
        public string LogDirectory { get; set; } = "Logs";
        public string LogFileExtension { get; set; } = "log";
    }
}
