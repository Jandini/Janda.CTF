namespace Janda.CTF
{
    internal interface IChallengeDetails
    {
        public string Type { get; }
        public string Name { get; }
        public int Points { get; }
        public int Number { get; }
        public string Flag { get; }
        public string Brief { get; }
        public string Hints { get; }
        public string Resources { get; }
    }
}