using CommandLine;

namespace Janda.CTF
{
    [Verb("run", isDefault: true, HelpText = "Run challenge.")]
    public class ChallengeOptions : IChallengeOptions
    {
        [Option("class", Required = true)]
        public string Class { get; set; }
    }
}
