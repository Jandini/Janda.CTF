using CommandLine;

namespace Janda.CTF
{
    [Verb("play", isDefault: false, HelpText = "Run multiple challenges.")]
    class ChallengePlayOptions : IChallengePlayOptions
    {
        [Option("scope", Default = "*")]
        public string Scope { get; set; }
    }
}
