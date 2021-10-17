using System;

namespace Janda.CTF
{
    public class CTFAttribute : Attribute
    {
        /// <summary>
        /// The name of CTF project. This name is be displayed and logged during the start up.        
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Informational only to keep track of CTF's address. Not logged or displayed. 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Provide project name in case output executable file name is diffrent than *.csproj file. 
        /// The project name is required to locate Challenges folder within multiproject solution. 
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Automatically maximize console window under Windows operating system.
        /// </summary>
        public bool MaximizeConsole { get; set; }
        
        /// <summary>
        /// If you don't provide appsettings.json you can use the internal settings. 
        /// Internal settings are configuring logging only to the console.
        /// </summary>
        public bool UseEmbeddedAppSettings { get; set; }    
    }
}
