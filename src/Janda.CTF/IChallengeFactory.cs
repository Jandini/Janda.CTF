namespace Janda.CTF
{
    interface IChallengeFactory
    {
        void Run(string className);
        string[] GetChallengeClasses();
        IChallengeDetails GetChallengeDetails(string className);
    }
}