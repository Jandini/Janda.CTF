using System;

namespace Janda.CTF
{
    /// <summary>
    /// Use this attribute to mark completed challenges.
    /// </summary>
    public class ChallengeAttribute : Attribute
    {
        public string Flag { get; set; }
    }
}
