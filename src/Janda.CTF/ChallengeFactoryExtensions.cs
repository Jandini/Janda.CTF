using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Janda.CTF
{
    static class ChallengeFactoryExtensions
    {        
        internal static IServiceCollection AddChallengeFactory(this IServiceCollection services)
        {
            var challenges = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
               .Where(x => typeof(IChallenge).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var challenge in challenges)
            {
                if (!ChallengeFactory._challenges.TryAdd(challenge.Name, challenge))
                    throw new Exception($"Challenge \"{challenge.Name}\" class is already registered.");

                services.AddTransient(challenge);
            }

            return services.AddSingleton<IChallengeFactory, ChallengeFactory>();
        }
    }
}
