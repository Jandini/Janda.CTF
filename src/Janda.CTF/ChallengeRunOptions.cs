using CommandLine;

namespace Janda.CTF
{
    [Verb("run", isDefault: false, HelpText = "Run multiple challenges.")]
    class ChallengeRunOptions : IChallengeRunOptions
    {
        [Option("scope", Default = "*")]
        public string Scope { get; set; }
    }
}
