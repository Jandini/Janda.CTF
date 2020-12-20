using CommandLine;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Janda.CTF
{
    class ChallengeTemplateService : IChallengeTemplateService
    {
        private readonly ILogger<ChallengeTemplateService> _logger;
        private readonly IChallengeProjectService _project;
        private readonly IChallengeLaunchSettingsService _launchSettings;

        public ChallengeTemplateService(ILogger<ChallengeTemplateService> logger, IChallengeProjectService project, IChallengeLaunchSettingsService launchSettings)
        {
            _logger = logger;
            _project = project;
            _launchSettings = launchSettings;
        }

        public void AddChallenges(IChallengeTemplateOptions options)
        {
            var root = _project.FindDirectory(options.ChallengeDir);

            if (options.ChallengeCount != null)
                for (int i = options.CounterStart; i <= options.CounterStart + options.ChallengeCount; i++)
                    AddChallenge(options, root, i);
            else
                AddChallenge(options, root);
        }


        public void AddChallenge(IChallengeTemplateOptions options, string root, int? number = null)
        {            
            var name = options.ChallengeName;

            if (number != null)
                name += ((int)number).ToString("D" + options.CounterPadding);

            var path = Path.Combine(root, Path.ChangeExtension(name, "cs"));

            if (!File.Exists(path))
            {
                var contents = LoadTemplate(options);

                ReplacePlaceholder(ref contents, "namespace", typeof(ChallengeTemplateService).Namespace);
                ReplacePlaceholder(ref contents, "name", name);

                foreach (var property in options.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(OptionAttribute))))
                    ReplacePlaceholder(ref contents, property.GetCustomAttributes(typeof(OptionAttribute), false).Cast<OptionAttribute>().First().LongName, property.GetValue(options));

                _launchSettings.AddChallenge(name);

                if (options.HasResourceDir)
                {
                    _logger.LogTrace("Create resource directory for {name}", name);
                    Directory.CreateDirectory(Path.Combine(root, name));
                }

                File.WriteAllText(path, contents);

                _logger.LogTrace("Challenge {name} added successfully", name);
            }
            else
                _logger.LogError("The challenge {name} file already exist in {path}", name, path);
        }

        private void ReplacePlaceholder(ref string contents, string placeholder, object value, object defaultValue = null)
        {
            contents = contents.Replace($"{{{{{placeholder}}}}}", value?.ToString() ?? defaultValue?.ToString() ?? "");
        }

        private string LoadTemplate(IChallengeTemplateOptions options)
        {
            try
            {
                return File.ReadAllText(FindTemplateFilePath(options.TemplateName));
            }
            catch (FileNotFoundException)
            {
                return LoadEmbeddedTemplate(options);
            }
        }

        private string GetTemplateFileName(string templateName)
        {
            return Path.ChangeExtension(templateName, "ch");
        }
       
        private string FindTemplateFilePath(string templateName)
        {                               
            _logger.LogTrace("Locating {name} template file", templateName);
            return _project.FindFile(GetTemplateFileName(templateName));
        }


        private string LoadEmbeddedTemplate(IChallengeTemplateOptions options)
        {
            var assembly = options.GetType().Assembly;

            _logger.LogTrace("Loading embedded template {name} from {assembly}", options.TemplateName, assembly.GetName().Name);

            using var stream = new EmbeddedFileProvider(assembly, typeof(ChallengeTemplateService).Namespace)
                .GetFileInfo(GetTemplateFileName(options.TemplateName)).CreateReadStream();

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
