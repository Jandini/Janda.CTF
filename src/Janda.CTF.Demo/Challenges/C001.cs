using Microsoft.Extensions.Logging;

namespace Janda.CTF
{  
    public class C001 : IChallenge
    {
        private readonly ILogger<C001> _logger;

        public C001(ILogger<C001> logger)
        {
            _logger = logger;
        }
        
        public void Run()
        {
            _logger.LogInformation("Hello {challenge}", nameof(C001));
        }
    }
}
