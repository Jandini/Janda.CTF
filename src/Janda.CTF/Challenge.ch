using Microsoft.Extensions.Logging;

namespace {{namespace}}
{  
    [Challenge(Name = "{{name}}")]
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
