using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Reflection;

namespace Janda.CTF
{
    public class CTF
    {
        public static void Run(string[] args, Action<IServiceCollection> services = null)
        {
            var entryPoint = Assembly.GetEntryAssembly().EntryPoint;
            var executingAssembly = Assembly.GetExecutingAssembly();

            var version = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;            
            var challengeLogging = entryPoint.GetCustomAttribute<ChallengeLoggingAttribute>() ?? new ChallengeLoggingAttribute();
            var ctf = entryPoint.GetCustomAttribute<CTFAttribute>() ?? new CTFAttribute();

            var title = $"CTF runner {version}";
            Console.Title = title;

            foreach (var assemblyFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.Template?.*.dll"))
                Assembly.LoadFrom(assemblyFile);

            new ChallengeRunner()
                 .WithParser(new Parser((settings) =>
                 {
                     settings.HelpWriter = null;
                 }))
                .ParseVerbs(args, (result) =>
                {
                    Console.WriteLine(HelpText.AutoBuild(result, h =>
                    {
                        h.Heading = title;
                        h.AdditionalNewLineAfterOption = false;                        
                        h.Copyright = string.Empty;
                        h.AddDashesToOption = true;

                        return HelpText.DefaultParsingErrorsHandler(result, h);
                    }, e => e, true));
                })
                .WithConfiguration(() =>
                {
                    using var stream = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), typeof(CTF).Namespace)
                        .GetFileInfo("CTF.appsettings.json").CreateReadStream();

                    return new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .AddJsonFile("appsettings.json", true)
                        .Build();
                })
                .WithLogging((logging) =>
                {
                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(ChallengeRunner.Configuration);

                    var options = ChallengeRunner.Options as IChallengeOptions;
                    var name = options?.Name ?? "CTF";

                    loggerConfiguration.WriteTo.File(
                        path: Path.Combine(
                            challengeLogging.LogDirectory ?? string.Empty, 
                            Assembly.GetEntryAssembly().GetName().Name, 
                            name, 
                            Path.ChangeExtension($"{name}-{DateTime.Now:yyyyMMddHHmmss}", challengeLogging.LogFileExtension ?? "log")));

                    logging.AddSerilog(
                        loggerConfiguration.CreateLogger(),
                        dispose: true);

                })
                .WithServices((services) => services.AddChallengeServices())
                .WithServices(services)
                .Run((provider) =>
                {
                    var logger = provider.GetRequiredService<ILogger<CTF>>();

                    if (!string.IsNullOrEmpty(ctf?.Name))
                        logger.LogTrace("Started {name}", ctf.Name);

                    logger.LogTrace("Using {title}", title);

                    switch (ChallengeRunner.Options)
                    {
                        case IChallengeOptions options:
                            provider.GetRequiredService<IChallengeRunnerService>().Run(options);
                            break;

                        case IChallengePlayOptions options:
                            provider.GetRequiredService<IChallengeRunnerService>().Run(options);
                            break;

                        case IChallengeTemplateOptions options:
                            provider.GetRequiredService<IChallengeTemplateService>().AddChallenges(options);
                            break;

                        default:
                            throw new NotImplementedException();
                    };
                });
        }
    }
}
