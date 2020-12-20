using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Janda.CTF
{
    class ChallengeFactory : IChallengeFactory
    {
        internal static Dictionary<string, Type> _challenges = new Dictionary<string, Type>();
        private readonly ILogger<ChallengeFactory> _logger;
        private readonly IServiceProvider _services;

        public ChallengeFactory(ILogger<ChallengeFactory> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;            
        }

        public string[] GetChallenges()
        {
            return _challenges.Keys.ToArray();
        }

        public void Run(string name)
        {
            var previous = Console.Title;

            if (!_challenges.ContainsKey(name))
                throw new Exception($"Challenge \"{name}\" was not found.");

            var challenge = (IChallenge)_services.GetService(_challenges[name]);            

            _logger.LogTrace("Running challange {name}", name);

            try
            {
                Console.Title = $"{previous} - Running {name}";
                challenge.Run();
                _logger.LogTrace("Challenge {name} completed", name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            finally
            {
                Console.Title = previous;
            }
        }     
    }
}
