using Janda.CTF;
using Microsoft.Extensions.DependencyInjection;

namespace QuickStart
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