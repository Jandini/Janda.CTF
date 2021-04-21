using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using RightTurn;
using RightTurn.Extensions.CommandLine;
using RightTurn.Extensions.Configuration;
using RightTurn.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Janda.CTF
{
    public class CTF
    {
        public static void Run(string[] args, Action<IServiceCollection> services = null)
        {
            var entryPoint = Assembly.GetEntryAssembly().EntryPoint;
            var ctf = entryPoint.GetCustomAttribute<CTFAttribute>() ?? new CTFAttribute();

            if (ctf.MaximizeConsole)
                CTFConsole.Maximize();

            var executingAssembly = Assembly.GetExecutingAssembly();

            var version = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var challengeLogging = entryPoint.GetCustomAttribute<ChallengeLoggingAttribute>() ?? new ChallengeLoggingAttribute();

            var title = $"{executingAssembly.GetName().Name} {version}";
            Console.Title = title;

            foreach (var assemblyFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.Template?.*.dll"))
                Assembly.LoadFrom(assemblyFile);

            new Turn()
                .WithParser(new Parser((settings) => { settings.HelpWriter = null; }))
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
                    if (ctf.UseEmbeddedAppSettings == false && File.Exists("appsettings.json"))
                        return new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", true)
                            .Build();
                    else
                    {
                        using var stream = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), typeof(CTF).Namespace)
                            .GetFileInfo("CTF.appsettings.json").CreateReadStream();

                        return new ConfigurationBuilder()
                            .AddJsonStream(stream)
                            .Build();
                    }
                })
                .WithLogging((logging, turn) =>
                {
                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(turn.Directions.Configuration());

                    var name = turn.Directions.TryGet<ITurnArgs>().TryGetOptions<IChallengeOptions>(out var options)
                        ? options.Class
                        : "CTF";

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
                .WithDirections()
                .WithUnhandledExceptionLogging()
            .Take((provider) =>
            {
                var logger = provider.GetRequiredService<ILogger<CTF>>();
                var directions = provider.GetRequiredService<ITurnDirections>();

                if (!string.IsNullOrEmpty(ctf?.Name))
                    logger.LogTrace("Started {name}", ctf.Name);

                logger.LogTrace("Using {title}", title);

                switch (directions.Get<ITurnArgs>().Options)
                {
                    case IChallengeOptions options:
                        provider.GetRequiredService<IChallengeRunnerService>().Run(options);
                        break;

                    case IChallengePlayOptions options:
                        provider.GetRequiredService<IChallengeRunnerService>().Run(options);
                        break;

                    case IChallengeListOptions options:
                        provider.GetRequiredService<IChallengeRunnerService>().List(options);
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
