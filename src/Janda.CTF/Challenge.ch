using Janda.CTF;
using Microsoft.Extensions.Logging;

namespace {{namespace}}
{  
    public class {{name}} : IChallenge
    {
        private readonly ILogger<{{name}}> _logger;

        public {{name}}(ILogger<{{name}}> logger)
        {
            _logger = logger;
        }
        
        public void Run()
        {

        }
    }
}
