using Microsoft.Extensions.Logging;
using System;
using System.Linq;

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

        public void List(IChallengeListOptions options)
        {
            var classes = _factory.GetChallengeClasses();
            var longest = classes.Max(className => _factory.GetChallengeDetails(className)?.Name?.Length ?? className.Length);

            foreach (var className in classes) 
            {
                var details = _factory.GetChallengeDetails(className);
                if (!options.HasFlag || (options.HasFlag && !string.IsNullOrEmpty(details.Flag)))
                    _logger.LogInformation("{name} {flag}", (details?.Name ?? className).PadRight(longest), details.Flag);
            }
        }

        public void Run(IChallengePlayOptions options)
        {
            var challenges = _factory.GetChallengeClasses();            
            _logger.LogTrace("Playing {@challenges}", string.Join("; ", challenges));

            foreach (var challenge in challenges)
                _factory.Run(challenge);
        }

        public void Run(IChallengeOptions options)
        {            
            _factory.Run(options.Class);
        }
    }
}
