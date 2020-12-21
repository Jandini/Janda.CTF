using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Janda.CTF
{
    public static class ChallengeRunnerExtensions
    {
        public static ChallengeRunner WithConfiguration(this ChallengeRunner runner, Func<IConfiguration> builder) => runner.AddConfiguration(builder);
        public static ChallengeRunner WithServices(this ChallengeRunner runner, Action<IServiceCollection> services) => runner.AddServices(services);
        public static ChallengeRunner WithLogging(this ChallengeRunner runner, Action<ILoggingBuilder> logging) => runner.AddLogging(logging);
        public static ChallengeRunner WithParser(this ChallengeRunner runner, Parser parser) => runner.SetParser(parser);
        public static ChallengeRunner ParseOptions<T>(this ChallengeRunner runner, string[] args) => runner.ParseOptions<T>(args);
        public static ChallengeRunner ParseVerbs(this ChallengeRunner runner, string[] args, Type[] verbs, Action<ParserResult<object>> unparsed = null) => runner.ParseVerbs(args, verbs, unparsed);
        public static ChallengeRunner ParseOptions<T>(this ChallengeRunner runner, string[] args, out T options) => runner.ParseOptions(args, out options);
        public static ChallengeRunner ParseVerbs(this ChallengeRunner runner, string[] args, Action<ParserResult<object>> unparsed = null) => runner.ParseVerbs(args, unparsed);
        public static IServiceProvider Build(this ChallengeRunner runner) => runner.Build();
     
    }
}
