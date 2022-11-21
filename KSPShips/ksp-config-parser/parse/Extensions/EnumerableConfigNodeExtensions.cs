using System.Collections.Generic;
using System.Linq;
using KSPCfgParser.Models;

namespace KSPCfgParser.Extensions
{
    public static class EnumerableConfigNodeExtensions
    {
        // https://stackoverflow.com/a/7063002
        public static IEnumerable<ConfigNode> Descendants(this ConfigNode root)
        {
            var nodes = new Stack<ConfigNode>(new[] { root });
            while (nodes.Any())
            {
                ConfigNode node = nodes.Pop();
                yield return node;
                foreach (var n in node.Nodes) nodes.Push(n);
            }
        }
    }
}