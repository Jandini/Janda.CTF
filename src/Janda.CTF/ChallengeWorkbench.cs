﻿using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CommandLine;
using System.IO;
using Serilog;
using System.Linq;

namespace Janda.CTF
{
    public sealed class ChallengeWorkbench
    {
        public static IServiceProvider Services { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        public static object Options { get; internal set; }

        private Parser _parser;
        private Action _unparsed;
        private readonly ServiceCollection _services;
        private Func<IConfiguration> _configuration;
        private Action<ILoggingBuilder> _logging;


        public ChallengeWorkbench()
        {        
            _services = new ServiceCollection();
            _parser = Parser.Default;

            _configuration = DefaultConfiguration;
            _logging = DefaultLogging;
            _unparsed = DefaultExit;
        }

        public static void Run<TService, TImplementation>(string[] args, Action<TService> run, Action<IServiceCollection> services = null)
            where TService : class
            where TImplementation : class, TService => new ChallengeWorkbench()
                .SetUnparsedOptions(args)
                .AddServices(services)
                .Run<TService, TImplementation>(run);

        
        public static int Run<TService, TImplementation, TOptions>(string[] args, Func<TService, int> run, Action<IServiceCollection> services = null)
            where TService : class
            where TImplementation : class, TService => new ChallengeWorkbench()
                .ParseOptions<TOptions>(args)
                .WithServices(services)
                .Run<TService, TImplementation>(run);

        public static void Run<TService, TImplementation, TOptions>(string[] args, Action<TService> run, Action<IServiceCollection> services = null)
            where TService : class
            where TImplementation : class, TService => new ChallengeWorkbench()
                .ParseOptions<TOptions>(args)
                .WithServices(services)
                .Run<TService, TImplementation>(run);




        public static TService RunService<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return new ChallengeWorkbench()
                .AddServices((services) => { services.AddTransient<TService, TImplementation>(); })
                .Build()
                .GetRequiredService<TService>();
        }


        public static void RunVerbs<TService, TImplementation>(string[] args, Action<TService> run, Action<IServiceCollection> services = null)
            where TService : class
            where TImplementation : class, TService => new ChallengeWorkbench()
                .ParseVerbs(args)
                .AddServices(services)
                .Run<TService, TImplementation>((service) => { run?.Invoke(service); return 0; });


        public static int RunVerbs<TService, TImplementation>(string[] args, Func<TService, int> run, Action<IServiceCollection> services = null)
            where TService : class
            where TImplementation : class, TService => new ChallengeWorkbench()
                .ParseVerbs(args)
                .AddServices((services) => services.AddTransient<TService, TImplementation>())
                .WithServices(services)
                .Run<TService, TImplementation>(run);



        public static TService RunVerbs<TService, TImplementation>(string[] args, Action<IServiceCollection> services = null)
            where TService : class
            where TImplementation : class, TService => new ChallengeWorkbench()
                .ParseVerbs(args)
                .AddServices((services) => { services.AddTransient<TService, TImplementation>(); })
                .AddServices(services)
                .Build()
                .GetRequiredService<TService>();


        public static int RunVerbs(string[] args, Action<IServiceCollection> services, Func<IServiceProvider, int> run) => new ChallengeWorkbench()
            .ParseVerbs(args)
            .AddServices(services)
            .Run(run);


        public int Run<TService, TImplementation>(Func<TService, int> run)
            where TService : class
            where TImplementation : class, TService
        {
            _services.AddTransient<TService, TImplementation>();            
            return Run((provider) => run(provider.GetRequiredService<TService>()));
        }


        public void Run<TService, TImplementation>(Action<TService> run)
            where TService : class
            where TImplementation : class, TService
        {
            _services.AddTransient<TService, TImplementation>();            
            Run((provider) => run(provider.GetRequiredService<TService>()));
        }
        

        public int Run(Func<IServiceProvider, int> run)
        {
            try
            {
                return run(Build());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        public void Run(Action<IServiceProvider> run)
        {
            try
            {
                run(Build());
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }


        private int HandleException(Exception ex)
        {
            var logger = GetService<ILogger<ChallengeWorkbench>>();

            if (logger != null)
                logger.LogCritical(ex.Message, ex);
            else
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");

            return ex.HResult;
        }


        private T GetService<T>()
        {
            return Services != null
                ? Services.GetService<T>()
                : default;
        }

        private void DefaultExit()
        {
            Environment.Exit(1);
        }


        private IConfiguration DefaultConfiguration()
        {          
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private void DefaultLogging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddSerilog(
                new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .CreateLogger(),
                dispose: true);
        }


        private static Type[] LoadVerbs()
        {                                               
            var verbs = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !IsSystemAssembly(a))
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null)
                .ToArray();

            if (verbs.Length == 0)
                throw new Exception("At least one command line verb is required.");

            return verbs;
        }


        private static bool IsSystemAssembly(Assembly assembly)
        {
            var ignore = new[] { "Microsoft.", "System.", "netstandard", "CommandLine." };

            return ignore.Any(a => assembly.FullName.StartsWith(a))
                || assembly.FullName == typeof(ChallengeWorkbench).Namespace;
        }


        internal IServiceProvider Build()
        {
            Configuration = _configuration();
            _services.AddLogging(_logging);
            return Services = _services.BuildServiceProvider();
        }


        internal ChallengeWorkbench AddLogging(Action<ILoggingBuilder> logging)
        {
            _logging = logging;
            return this;
        }

        internal ChallengeWorkbench AddConfiguration(Func<IConfiguration> builder)
        {
            _configuration = builder;
            return this;
        }

        internal ChallengeWorkbench AddServices(Action<IServiceCollection> services)
        {
            services?.Invoke(_services);
            return this;
        }


        internal ChallengeWorkbench ConfigureWorkbench(Action<ChallengeWorkbench> workbench)
        {
            workbench?.Invoke(this);
            return this;
        }


        internal ChallengeWorkbench SetParser(Parser parser)
        {
            _parser = parser;
            return this;
        }

        internal ChallengeWorkbench SetUnparsedOptions(string[] args)
        {
            Options = args;
            return this;
        }

        internal ChallengeWorkbench SetUnparsedAction(Action unparsed)
        {
            _unparsed = unparsed;
            return this;
        }

        internal ChallengeWorkbench ParseOptions<T>(string[] args)
        {
            T parsed = default;

            _parser.ParseArguments<T>(args)
                .WithNotParsed((e) => _unparsed.Invoke()) 
                .WithParsed((o) => { parsed = o; });

            Options = parsed;

            return this;
        }

        internal ChallengeWorkbench ParseVerbs(string[] args, Type[] verbs)
        {
            object parsed = default;

            _parser.ParseArguments(args, verbs)
                .WithNotParsed((e) => _unparsed.Invoke())
                .WithParsed((o) => { parsed = o; });

            Options = parsed;

            return this;
        }

        internal ChallengeWorkbench ParseOptions<T>(string[] args, out T options)
        {
            ParseOptions<T>(args);
            options = (T)Options;
            return this;
        }

        internal ChallengeWorkbench ParseVerbs(string[] args)
        {
            ParseVerbs(args, LoadVerbs());
            return this;
        }
    }
}