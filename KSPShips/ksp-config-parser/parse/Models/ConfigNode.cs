using System.Collections.Generic;

namespace KSPCfgParser.Models
{
    public class ConfigNode
    {
        public ConfigNode Parent { get; set; }
        public NodeType Type { get; set; }
        public string TypeIdentifier { get; set; }
        public ICollection<ConfigNode> Nodes { get; set; } = new List<ConfigNode>();
        public ICollection<AttributeDefinition> AttributeDefinitions { get; set; } = new List<AttributeDefinition>();
        public IList<InputLine> InputLines { get; set;} = new List<InputLine>();
    }
}