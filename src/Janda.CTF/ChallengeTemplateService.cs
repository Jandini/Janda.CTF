using CommandLine;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

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
            var className = options.ChallengeClass.ToPascalCase() ?? options.ChallengeName.ToPascalCase();
            var challengeName = (string.IsNullOrEmpty(options.ChallengeName) ? className : options.ChallengeName).Replace("\"", "\\\"");

            if (string.IsNullOrEmpty(className))
                throw new Exception("Challenge class is invalid or empty.");

            if (number != null)
                className += ((int)number).ToString("D" + options.CounterPadding);

            var path = Path.Combine(root, Path.ChangeExtension(className, "cs"));

            if (!File.Exists(path))
            {            
                var contents = LoadTemplate(options);

                ReplaceNamespace(ref contents);
                ReplacePlaceholder(ref contents, "class", className);
                ReplacePlaceholder(ref contents, "name", challengeName);                

                foreach (var property in options.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(OptionAttribute))))
                    ReplacePlaceholder(ref contents, property.GetCustomAttributes(typeof(OptionAttribute), false).Cast<OptionAttribute>().First().LongName, property.GetValue(options));

                _launchSettings.AddChallenge(challengeName, className);

                if (options.HasResourceDir)
                {
                    _logger.LogTrace("Create resource directory for {name}", className);
                    Directory.CreateDirectory(Path.Combine(root, className));
                }

                File.WriteAllText(path, contents);

                _logger.LogTrace("Challenge {name} created in {path}", challengeName, path);
            }
            else
                _logger.LogError("The challenge {name} file already exist in {path}", challengeName, path);
        }

        private void ReplacePlaceholder(ref string contents, string placeholder, object value, object defaultValue = null)
        {
            contents = contents.Replace($"{{{{{placeholder}}}}}", value?.ToString() ?? defaultValue?.ToString() ?? "");
        }

        private void ReplaceNamespace(ref string contents)
        {
            var entryNs = Assembly.GetEntryAssembly().EntryPoint?.DeclaringType.Namespace;
            var thisNs = typeof(ChallengeTemplateService).Namespace;

            if (!entryNs.StartsWith(thisNs))
                contents = $"using {thisNs};\r\n" + contents;
          
            ReplacePlaceholder(ref contents, "namespace", Assembly.GetEntryAssembly().EntryPoint?.DeclaringType.Namespace ?? typeof(ChallengeTemplateService).Namespace);
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
