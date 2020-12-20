using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

namespace Janda.CTF
{
    class ChallengeLaunchSettingsService : IChallengeLaunchSettingsService
    {
        private readonly ILogger<ChallengeTemplateService> _logger;
        private readonly IChallengeProjectService _project;

        public ChallengeLaunchSettingsService(ILogger<ChallengeTemplateService> logger, IChallengeProjectService project)
        {
            _logger = logger;
            _project = project;
        }

        public void AddChallenge(string name)
        {
            const string LAUNCH_SETTINGS_JSON = "launchSettings.json";
            _logger.LogTrace("Adding challenge {name} to {file}", name, LAUNCH_SETTINGS_JSON);

            var sb = new StringBuilder();
            var launchSettings = _project.FindFile(LAUNCH_SETTINGS_JSON);

            foreach (var line in File.ReadAllLines(launchSettings))
            {
                if (line.Contains($"\"{name}\":"))
                {                    
                    sb = null;
                    break;
                }
                else 
                    if (line.Trim() == "}")
                        break;

                sb.AppendLine(line);
            }

            if (sb != null)
            {
                sb.AppendLine($"{"",4}}},");
                sb.AppendLine($"{"",4}\"{name}\": {{");
                sb.AppendLine($"{"",6}\"commandName\": \"Project\",");
                sb.AppendLine($"{"",6}\"commandLineArgs\": \"--name {name}\"");
                sb.AppendLine($"{"",4}}}");
                sb.AppendLine($"{"",2}}}");
                sb.AppendLine($"}}");

                File.WriteAllText(launchSettings, sb.ToString(), Encoding.ASCII);
            }
            else
            {
                _logger.LogTrace("Challenge {name} already exist in {file}", name, LAUNCH_SETTINGS_JSON);
            }
        }
    }
}
