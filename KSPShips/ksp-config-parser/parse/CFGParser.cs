using System;
using System.Collections.Generic;
using System.IO;
using KSPCfgParser.Extensions;
using KSPCfgParser.Models;

namespace KSPCfgParser
{
    public static class CFGParser
    {
        public static ConfigFile ParseConfigFile(string filePath)
        {
            var fileLines = ReadStreamIntoInputLines(filePath);
            return new()
            {
                FilePath = filePath,
                RootNode = ParseInputLinesIntoNodes(fileLines)
            };
        }

        private static ConfigNode ParseInputLinesIntoNodes(IList<InputLine> fileLines)
        {
            if (fileLines == null) return new ConfigNode();

            IdentifyBlocks(fileLines);
            return ParseBlocksIntoNodes(fileLines);
        }

        public static ConfigNode ParseBlocksIntoNodes(IList<InputLine> fileLines)
        {
            var root = new ConfigNode();
            var currentNode = root;
            var nodeStack = new Stack<ConfigNode>();
            var currentBlockId = -1;
            var currentDepth = -1;
            nodeStack.Push(currentNode);

            foreach (var line in fileLines)
            {
                if (line.BlockDepth > currentDepth) // go deeper
                {
                    nodeStack.Push(currentNode);
                    currentNode = new ConfigNode
                    {
                        Type = line.Data.ToEnumOrDefault<NodeType>(),
                        TypeIdentifier = line.Data,
                        Parent = nodeStack.Peek(),
                    };
                    nodeStack.Peek().Nodes.Add(currentNode);
                }
                else if (line.BlockDepth < currentDepth) // go shallower
                {
                    currentNode = nodeStack.Pop();
                }
                else if (line.BlockId != currentBlockId) // new block at same level
                {
                    currentNode = new ConfigNode
                    {
                        Type = line.Data.ToEnumOrDefault<NodeType>(),
                        TypeIdentifier = line.Data,
                    };
                    nodeStack.Peek().Nodes.Add(currentNode);
                }

                currentNode.InputLines.Add(line);
                if (line.IsAttributeDefinition()) 
                    currentNode.AttributeDefinitions.Add(line.ToAttributeDefinition());

                currentBlockId = line.BlockId;
                currentDepth = line.BlockDepth;
            }
            
            return root;
        }

        public static void IdentifyBlocks(IList<InputLine> fileLines)
        {
            var idStack = new Stack<int>();
            int blockId = 0;
            int runningBlockId = 0;
            int depth = 0;
            for(var i = 0; i < fileLines.Count; i++)
            {
                fileLines[i].BlockId = blockId;
                fileLines[i].BlockDepth = depth;

                if (IsOpeningBrace(fileLines[i].Data))
                {
                    idStack.Push(blockId);
                    runningBlockId++;
                    blockId = runningBlockId;
                    depth++;

                    MarkBlockIdentifier(i, fileLines, runningBlockId, idStack.Peek(), depth);
                }

                if (IsClosingBrace(fileLines[i].Data))
                {
                    depth--;
                    blockId = idStack.Pop();

                    if (depth < 0){
                        var e = new Exception("Found too many closing braces!");
                        e.Data["rawLineNumber"] = fileLines[i].RawLineNumber;
                        e.Data["blockId"] = blockId;
                    }
                }
            }

            //TODO: Throw exception here if we don't end up back at depth zero?
        }

        ///<summary>Walk backwards in previous block/depth to find block identifier</summary>
        private static void MarkBlockIdentifier(int openingBraceIndex, IList<InputLine> fileLines, int blockId, int parentBlockId, int depth)
        {
            for (var i = openingBraceIndex; i >= 0; i--)
            {
                if (fileLines[i].BlockDepth != depth - 1) return;
                if (fileLines[i].BlockId != parentBlockId) return;

                fileLines[i].BlockDepth = depth;
                fileLines[i].BlockId = blockId;

                // The first non-blank data element prior to the opening brace is the block identifier
                if (i < openingBraceIndex
                    && !string.IsNullOrWhiteSpace(fileLines[i].Data)) return;
            }
        }

        private static bool IsOpeningBrace(string line)
        {
            return (line == Constants.OpeningBrace);
        }

        private static bool IsClosingBrace(string line)
        {
            return (line == Constants.ClosingBrace);
        }

        private static IList<InputLine> ReadStreamIntoInputLines(string filePath)
        {
            int rawLineCounter = 1;
            var lines = new List<InputLine>();

            using (StreamReader reader = File.OpenText(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var input = reader.ReadLine();
                    if (input != null)
                    {
                        var line = input.ParseLine(rawLineCounter);
                        if (!string.IsNullOrWhiteSpace(line.Data))
                        {
                            // We have an interesting line with data
                            var candidates = line.SplitLineDataOnBraces();
                            lines.AddRange(candidates);
                        }
                    }
                    rawLineCounter++;
                }
            }

            return lines;        
        }
    }
}
