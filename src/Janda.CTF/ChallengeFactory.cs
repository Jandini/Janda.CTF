using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public void Run(string className)
        {
            var previous = Console.Title;

            if (!_challenges.ContainsKey(className))
                throw new Exception($"Challenge class \"{className}\" was not found.");

            var type = _challenges[className];
            var challenge = (IChallenge)_services.GetService(type);
            var name = type.GetCustomAttribute<ChallengeAttribute>()?.Name ?? className;

            _logger.LogTrace("Running challange {name}", name);

            try
            {
                Console.Title = $"{previous} - Running {name}";
                challenge.Run();
                _logger.LogTrace("Finished challenge {name}", name);
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
