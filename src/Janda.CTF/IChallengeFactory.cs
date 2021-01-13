namespace Janda.CTF
{
    interface IChallengeFactory
    {
        void Run(string className);
        string[] GetChallenges();
    }
}