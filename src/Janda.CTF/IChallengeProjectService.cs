namespace Janda.CTF
{
    internal interface IChallengeProjectService
    {
        string FindDirectory(string directoryName);
        string FindFile(string fileName);
    }
}