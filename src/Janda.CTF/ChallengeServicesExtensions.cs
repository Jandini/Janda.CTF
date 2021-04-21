using Microsoft.Extensions.DependencyInjection;

namespace Janda.CTF
{
    static class ChallengeServicesExtensions
    {
        public static IServiceCollection AddChallengeServices(this IServiceCollection services)
        {
            return services
                .AddChallengeFactory()
                .AddTransient<IChallengeTemplateService, ChallengeTemplateService>()
                .AddTransient<IChallengeRunnerService, ChallengeRunnerService>()
                .AddTransient<IChallengeProjectService, ChallengeProjectService>()
                .AddTransient<IChallengeLaunchSettingsService, ChallengeLaunchSettingsService>();
            
        }
    }
}
