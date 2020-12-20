using Microsoft.Extensions.Logging;
using QuickStart;

namespace Janda.CTF
{
    public class C001 : IChallenge
    {
        readonly ILogger<C001> _logger;
        readonly IFlagFinder _flagFinder;

        public C001(ILogger<C001> logger, IFlagFinder flagFinder)
        {
            _logger = logger;
            _flagFinder = flagFinder;
        }

        public void Run()
        {
            _logger.LogInformation("I will capture the flag!");
            _flagFinder.FindFlag();
        }
    }
}
