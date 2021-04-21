using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Janda.CTF
{
    class ChallengeProjectService : IChallengeProjectService
    {    
        private readonly ILogger<ChallengeProjectService> _logger;
        private string _projectPath;

        public ChallengeProjectService(ILogger<ChallengeProjectService> logger)
        {
            _logger = logger;
            
        }

        public string FindDirectory(string directoryName)
        {
            _logger.LogTrace("Searching for {directory}", directoryName);

            var dirs = Directory.GetDirectories(GetProjectDirectory(), directoryName, SearchOption.AllDirectories);

            if (dirs.Length == 0)
                throw new Exception($"Directory \"{directoryName}\" was not found.");

            var path = new DirectoryInfo(dirs[0]).FullName;

            _logger.LogTrace("Directory {folderName} found in {path}", directoryName, path);
            return path;
        }


        public string FindFile(string fileName)
        {
            _logger.LogTrace("Searching for {file}", fileName);

            var files = Directory.GetFiles(GetProjectDirectory(), fileName, SearchOption.AllDirectories);

            if (files.Length == 0)
                throw new FileNotFoundException($"File \"{fileName}\" was not found.");

            var path = new FileInfo(files[0]).FullName;
            _logger.LogTrace("File found {file}", path);

            return path;
        }

        public string GetProjectDirectory()
        {
            if (_projectPath != null)
                return _projectPath;

            const string CURRENT_DIR = ".";
            const string PARENT_DIR = @"\..";
            const int PARENT_MAX_DEPTH = 4;
            
            var path = CURRENT_DIR;
            var pattern = "*.csproj";
            var files = new string[0];

            _logger.LogTrace("Searching for {file} project file...", pattern);

            while (files.Length == 0 && path.Length < CURRENT_DIR.Length + PARENT_DIR.Length * PARENT_MAX_DEPTH)
            {
                files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                path += @"\..";
            };

            if (files.Length == 0)
                throw new FileNotFoundException($"File \"{pattern}\" was not found.");

            path = new FileInfo(files[0]).FullName;
            _projectPath = new DirectoryInfo(path).Parent.FullName;

            _logger.LogTrace("Project file found in {path}", _projectPath);

            return _projectPath;
        }
    }
}

