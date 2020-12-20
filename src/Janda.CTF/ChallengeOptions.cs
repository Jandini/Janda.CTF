using CommandLine;

namespace Janda.CTF
{
    [Verb("work", isDefault: true, HelpText = "Run challenge.")]
    public class ChallengeOptions : IChallengeOptions
    {
        [Option("name", Required = true)]
        public string Name { get; set; }
    }
}
