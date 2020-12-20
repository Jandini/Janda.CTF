namespace Janda.CTF
{
    interface IChallengeTemplateOptions
    {
        string ChallengeName { get; }
        string TemplateName { get; }
        string ChallengeDir { get; }
        int? ChallengeCount { get; }
        int CounterStart { get; }
        int CounterPadding { get; }
        bool HasResourceDir { get; }
    }
}
