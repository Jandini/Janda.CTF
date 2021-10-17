using Microsoft.Extensions.Logging;

namespace {{namespace}}
{  
    [Challenge(Name = "{{name}}", Brief = @"")]
    public class {{class}} : IChallenge
    {
        private readonly ILogger<{{class}}> _logger;

        public {{class}}(ILogger<{{class}}> logger)
        {
            _logger = logger;
        }
        
        public void Run()
        {

        }
    }
}
