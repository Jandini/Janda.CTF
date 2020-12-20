using CommandLine;

namespace Janda.CTF
{
    [Verb("add", isDefault: false, HelpText = "Add new challenge.")]
    public class ChallengeTemplateOptions : IChallengeTemplateOptions
    {
        [Option("name", HelpText = "Challenge name.", Required = true)]        
        public string ChallengeName { get; set; }

        [Option("template", Default = "Challenge", HelpText = "Challenge template name.")]
        public virtual string TemplateName { get; set; }

        [Option("root", Default = "Challenges", HelpText = "Challenge root directory.")]
        public string ChallengeDir { get; set; }

        [Option("resource", Default = false, HelpText = "Create resource directory for new challenge.")]
        public bool HasResourceDir { get; set; }

        [Option("count", HelpText = "Create number of consecutive challenges.")]
        public int? ChallengeCount { get; set; }

        [Option("padding", Default = 3, HelpText = "Consecutive challenge number padding.")]
        public int CounterPadding { get; set; }

        [Option("start", Default = 1, HelpText = "Consecutive challenge start number.")]
        public int CounterStart { get; set; }

    }
}
