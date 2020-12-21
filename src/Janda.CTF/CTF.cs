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
        const string TITLE = "CTF runner";
        const string LOG_DIR = "Logs";
        const string LOG_FILE_EXTENSION = "log";

        public static void Run(string[] args, Action<IServiceCollection> services = null)
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var ctf = Assembly.GetEntryAssembly().EntryPoint.GetCustomAttribute<CTFAttribute>();

            var title = $"{TITLE} {version}";
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
                            LOG_DIR, 
                            Assembly.GetEntryAssembly().GetName().Name, 
                            name, 
                            Path.ChangeExtension($"{name}-{DateTime.Now:yyyyMMddHHmmss}", LOG_FILE_EXTENSION)));

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
                        logger.LogTrace("Started {ctf} with {title}", ctf.Name, title);
                    else
                        logger.LogTrace("Started {title}", title);

                    switch (ChallengeRunner.Options)
                    {
                        case IChallengeOptions options:
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
