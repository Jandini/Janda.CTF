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


        public IChallengeDetails GetChallengeDetails(string className)
        {
            var type = _challenges[className];
            return type.GetCustomAttribute<ChallengeAttribute>();
        }

        public string[] GetChallengeClasses()
        {
            return _challenges.Keys.ToArray();
        }

        public void Run(string className)
        {
            CTFConsole.PushTitle();

            if (!_challenges.ContainsKey(className))
                throw new Exception($"Challenge class \"{className}\" was not found.");

            var type = _challenges[className];
            var name = type.GetCustomAttribute<ChallengeAttribute>()?.Name ?? className;
            var challenge = (IChallenge)_services.GetService(type);

            _logger.LogTrace("Running challange {name}", name);

            try
            {
                CTFConsole.SetTitle($"{CTFConsole.Title} - Running {name}");
                challenge.Run();
                _logger.LogTrace("Finished challenge {name}", name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            finally
            {
                CTFConsole.PopTitle();
            }
        }
    }
}
