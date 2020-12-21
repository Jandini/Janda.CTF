using Microsoft.Extensions.Logging;

namespace Janda.CTF
{
    class ChallengeRunnerService : IChallengeRunnerService
    {
        private readonly ILogger<ChallengeRunnerService> _logger;
        private readonly IChallengeFactory _factory;

        public ChallengeRunnerService(ILogger<ChallengeRunnerService> logger, IChallengeFactory factory)
        {
            _logger = logger;
            _factory = factory;            
        }

        public void Run(IChallengePlayOptions options)
        {
            var challenges = _factory.GetChallenges();            
            _logger.LogTrace("Playing {@challenges}", string.Join(", ", challenges));

            foreach (var challenge in challenges)
                _factory.Run(challenge);
        }

        public void Run(IChallengeOptions options)
        {            
            _factory.Run(options.Name);
        }
    }
}
