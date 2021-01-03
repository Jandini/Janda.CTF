using System;

namespace Janda.CTF
{
    public class CTFAttribute : Attribute
    {
        public string Name { get; set; }
        public bool MaximizeConsole { get; set; }        
        public bool UseEmbeddedAppSettings { get; set; }    
    }
}
