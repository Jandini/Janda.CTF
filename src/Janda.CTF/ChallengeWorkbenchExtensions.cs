using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Janda.CTF
{
    public static class ChallengeWorkbenchExtensions
    {
        public static ChallengeWorkbench WithConfiguration(this ChallengeWorkbench workbench, Func<IConfiguration> builder) => workbench.AddConfiguration(builder);
        public static ChallengeWorkbench WithServices(this ChallengeWorkbench workbench, Action<IServiceCollection> services) => workbench.AddServices(services);
        public static ChallengeWorkbench WithLogging(this ChallengeWorkbench workbench, Action<ILoggingBuilder> logging) => workbench.AddLogging(logging);
        public static ChallengeWorkbench WithParser(this ChallengeWorkbench workbench, Parser parser) => workbench.SetParser(parser);
        public static ChallengeWorkbench ParseOptions<T>(this ChallengeWorkbench workbench, string[] args) => workbench.ParseOptions<T>(args);
        public static ChallengeWorkbench ParseVerbs(this ChallengeWorkbench workbench, string[] args, Type[] verbs, Action<ParserResult<object>> unparsed = null) => workbench.ParseVerbs(args, verbs, unparsed);
        public static ChallengeWorkbench ParseOptions<T>(this ChallengeWorkbench workbench, string[] args, out T options) => workbench.ParseOptions(args, out options);
        public static ChallengeWorkbench ParseVerbs(this ChallengeWorkbench workbench, string[] args, Action<ParserResult<object>> unparsed = null) => workbench.ParseVerbs(args, unparsed);
        public static IServiceProvider Build(this ChallengeWorkbench workbench) => workbench.Build();
     
    }
}
