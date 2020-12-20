using Microsoft.Extensions.Logging;

namespace QuickStart
{
    public class FlagFinder : IFlagFinder
    {
        readonly ILogger<FlagFinder> _logger;

        public FlagFinder(ILogger<FlagFinder> logger)
        {
            _logger = logger;
        }

        public void FindFlag()
        {
            _logger.LogInformation("Looking for a flag...");
        }
    }
}