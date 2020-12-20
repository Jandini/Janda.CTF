using Janda.CTF;
using Microsoft.Extensions.DependencyInjection;

namespace QuickStart
{
    class Program
    {
        static void Main(string[] args) => CTF.Run(args, (services) =>
        {
            services.AddTransient<IFlagFinder, FlagFinder>();
        });
    }
}