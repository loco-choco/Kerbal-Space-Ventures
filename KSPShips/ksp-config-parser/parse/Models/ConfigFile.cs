using System.Collections.Generic;

namespace KSPCfgParser.Models
{
    public class ConfigFile
    {
        public string FilePath { get; set; }
        public ConfigNode RootNode { get; set; }
    }
}