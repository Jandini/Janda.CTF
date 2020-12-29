using Microsoft.Extensions.DependencyInjection;
using QuickStart;

namespace Janda.CTF.QuickStart
{
    class Program
    {
        [CTF(Name = "Quick Start")]
        static void Main(string[] args) => CTF.Run(args, (services) =>
        {
            services.AddTransient<IFlagFinder, FlagFinder>();
        });
    }
}