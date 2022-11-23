using KSPCfgParser.Models;
using KSPCfgParser;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using System.Globalization;
using KSPShips.KSPCraftControl;

namespace KSPShips
{
    public static class KSPCraftCreator
    {
        public static Dictionary<string, GameObject> PiecesPrefabs = new();
        public static GameObject EmptyCraftPrefab;
        public static GameObject GenerateCraft(ConfigFile configFile, out string craftName) 
        {
            var baseNode = configFile.RootNode.Nodes.ElementAt(0);
            craftName = baseNode.AttributeDefinitions.FirstOrDefault(att => att.Name == "ship").Value;

            GameObject craftPrefab = Object.Instantiate(EmptyCraftPrefab);
            craftPrefab.SetActive(false);
            craftPrefab.name = craftName;
            //Creates empty base craft prefab
            Object.DontDestroyOnLoad(craftPrefab);

            MainCraftControl mainCraftControl = craftPrefab.GetComponent<MainCraftControl>();

            List<BaseKSPPart> parts = new();
            foreach (var node in baseNode.Nodes)
            {
                if (node.Type != NodeType.Part)
                    continue;

                string partNameWithID = node.AttributeDefinitions.FirstOrDefault(att => att.Name == "part").Value;
                int positionOfIdStart = partNameWithID.LastIndexOf('_');

                string id = partNameWithID.Substring(positionOfIdStart);
                string partName = partNameWithID.Substring(0, positionOfIdStart);

                if (!PiecesPrefabs.TryGetValue(partName, out var partPrefab))
                    continue;

                string posStr = node.AttributeDefinitions.FirstOrDefault(att => att.Name == "pos").Value;
                string rotStr = node.AttributeDefinitions.FirstOrDefault(att => att.Name == "rot").Value;
                string stageStr = node.AttributeDefinitions.FirstOrDefault(att => att.Name == "istg").Value;


                Vector3 pos = Vector3FromCFGString(posStr);
                Quaternion rot = QuaternionFromCFGString(rotStr);

                int stage = int.Parse(stageStr);

                var part = Object.Instantiate(partPrefab, craftPrefab.transform);
                part.name = partNameWithID;
                part.transform.localPosition = pos;
                part.transform.localRotation = rot;

                BaseKSPPart kspPart = part.GetComponent<BaseKSPPart>();

                //link indicates that the objects are attached
                var allLinks = node.AttributeDefinitions.Where(att => att.Name == "link");
                string[] initialLinks = new string[allLinks.Count()];
                for(int i =0; i < initialLinks.Length; i++) 
                {
                    initialLinks[i] = allLinks.ElementAt(i).Value;
                }
                kspPart.InitialLinks = initialLinks;
                kspPart.Stage = stage;

                parts.Add(kspPart);
            }

            mainCraftControl.ForceSetCenterOfMass(parts.ToArray());

            return craftPrefab;
        }
        static NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        public static Vector3 Vector3FromCFGString(string str) 
        {
            var strValues = str.Split(',');
            return new() 
            {
                x = float.Parse(strValues[0], nfi),
                y = float.Parse(strValues[1], nfi),
                z = float.Parse(strValues[2], nfi),
            };
        }
        public static Quaternion QuaternionFromCFGString(string str)
        {
            var strValues = str.Split(',');
            return new()
            {
                x = float.Parse(strValues[0], nfi),
                y = float.Parse(strValues[1], nfi),
                z = float.Parse(strValues[2], nfi),
                w = float.Parse(strValues[3], nfi),
            };
        }
    }
}
