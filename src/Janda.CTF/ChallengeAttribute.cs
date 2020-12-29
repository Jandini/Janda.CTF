using System;

namespace Janda.CTF
{
    /// <summary>
    /// Use this attribute for describe challenge features.
    /// </summary>
    public class ChallengeAttribute : Attribute
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Number { get; set; }
        public string Flag { get; set; }
    }
}
