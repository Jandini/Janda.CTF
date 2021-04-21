namespace Janda.CTF
{
    interface IChallengeRunnerService
    {

        /// <summary>
        /// Run given challenge
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        void Run(IChallengeOptions options);

        /// <summary>
        /// Run multipe challenges
        /// </summary>
        /// <returns></returns>
        void Run(IChallengePlayOptions options);

        /// <summary>
        /// List challenges
        /// </summary>
        /// <param name="options"></param>
        void List(IChallengeListOptions options);

    }
}
