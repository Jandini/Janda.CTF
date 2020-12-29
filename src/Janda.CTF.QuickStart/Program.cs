using Microsoft.Extensions.DependencyInjection;
using QuickStart;

namespace Janda.CTF.QuickStart
{
    class Program
    {
        [CTF(Name = "Quick Start", MaximizeConsole = true)]
        static void Main(string[] args) => CTF.Run(args, (services) =>
        {
            services.AddTransient<IFlagFinder, FlagFinder>();
        });
    }
}