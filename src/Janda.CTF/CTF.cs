﻿using CommandLine;
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
        const string TITLE = "CTF workbench";

        public static void Run(string[] args, Action<IServiceCollection> services = null)
        {

            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            var title = $"{TITLE} {version}";

            Console.Title = title;

            foreach (var assemblyFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.Template?.*.dll"))
                Assembly.LoadFrom(assemblyFile);

            new ChallengeWorkbench()
                 .WithParser(new Parser((settings) =>
                 {
                     settings.HelpWriter = null;
                 }))                 
                .ParseVerbs(args, (result) =>
                {
                    Console.WriteLine(HelpText.AutoBuild(result, h => {
                        h.Heading = title;
                        h.Copyright = string.Empty;
                        return HelpText.DefaultParsingErrorsHandler(result, h);
                    }, e => e));
                })               
                .WithServices((services) => services.AddChallengeServices())
                .WithServices(services)
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
                    const string LOG_DIR = "Logs";
                    const string LOG_FILE_EXTENSION = "log";

                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(ChallengeWorkbench.Configuration);

                    var options = ChallengeWorkbench.Options as IChallengeOptions;
                    var name = options?.Name ?? "CTF";

                    loggerConfiguration.WriteTo.File(
                        path: Path.Combine(LOG_DIR, name, Path.ChangeExtension($"{name}-{DateTime.Now:yyyyMMddHHmmss}", LOG_FILE_EXTENSION)));

                    logging.AddSerilog(
                        loggerConfiguration.CreateLogger(),
                        dispose: true);

                })
                .Run((provider) =>
                {
                    provider.GetRequiredService<ILogger<CTF>>().LogTrace("Running {Title} {Version}", TITLE, version);

                    switch (ChallengeWorkbench.Options)
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
