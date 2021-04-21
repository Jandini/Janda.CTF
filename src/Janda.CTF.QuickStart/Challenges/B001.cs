using Microsoft.Extensions.Logging;

namespace Janda.CTF.QuickStart
{  
    [Challenge()]
    public class B001 : IChallenge
    {
        private readonly ILogger<B001> _logger;

        public B001(ILogger<B001> logger)
        {
            _logger = logger;
        }
        
        public void Run()
        {

        }
    }
}
