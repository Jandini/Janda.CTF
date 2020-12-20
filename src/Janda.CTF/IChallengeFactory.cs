namespace Janda.CTF
{
    interface IChallengeFactory
    {
        void Run(string name);
        string[] GetChallenges();
    }
}