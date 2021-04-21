using CommandLine;

namespace Janda.CTF
{
    [Verb("list", isDefault: false, HelpText = "List challenges.")]
    class ChallengeListOptions : IChallengeListOptions
    {
        [Option("flag", Default = true)]
        public bool HasFlag { get; set; }
    }
}
