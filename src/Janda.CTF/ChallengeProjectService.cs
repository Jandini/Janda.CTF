using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Janda.CTF
{
    class ChallengeProjectService : IChallengeProjectService
    {
        const string CURRENT_DIR = ".";
        const string PARENT_DIR = @"\..";
        const int PARENT_MAX_DEPTH = 4;

        private readonly ILogger<ChallengeProjectService> _logger;

        public ChallengeProjectService(ILogger<ChallengeProjectService> logger)
        {
            _logger = logger;
        }

        public string FindDirectory(string directoryName)
        {
            var path = CURRENT_DIR;
            var dirs = new string[0];

            _logger.LogTrace("Searching for {directory}", directoryName);

            while (dirs.Length == 0 && path.Length < CURRENT_DIR.Length + PARENT_DIR.Length * PARENT_MAX_DEPTH)
            {                
                dirs = Directory.GetDirectories(path, directoryName, SearchOption.AllDirectories);
                path += PARENT_DIR;
            };

            if (dirs.Length == 0)
                throw new Exception($"Directory \"{directoryName}\" was not found.");

            path = new DirectoryInfo(dirs[0]).FullName;

            _logger.LogTrace("Directory {folderName} found in {path}", directoryName, path);
            return path;
        }


        public string FindFile(string fileName)
        {
            var path = CURRENT_DIR;
            var files = new string[0];

            _logger.LogTrace("Searching for {file}", fileName);

            while (files.Length == 0 && path.Length < CURRENT_DIR.Length + PARENT_DIR.Length * PARENT_MAX_DEPTH)
            {
                files = Directory.GetFiles(path, fileName, SearchOption.AllDirectories);
                path += @"\..";
            };

            if (files.Length == 0)
                throw new FileNotFoundException($"File \"{fileName}\" was not found.");

            path = new FileInfo(files[0]).FullName;
            _logger.LogTrace("File \"{templateName}\" was found in {file}", fileName, path);

            return path;
        }
    }
}

