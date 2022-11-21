using KSPCfgParser;
using SlateShipyard.ShipSpawner;

using OWML.ModHelper;
using System.IO;
using UnityEngine;
using OWML.Common;

namespace KSPShips
{
    public class KSPShips : ModBehaviour
    {
        public static IModHelper modHelper;
        private void Start()
        {
            modHelper = ModHelper;
            AssetBundle partsBundle = ModHelper.Assets.LoadBundle("AssetBundles/parts");
            AssetBundle emptyCraftBundle = ModHelper.Assets.LoadBundle("AssetBundles/emptycraft");

            var emptyCraft = emptyCraftBundle.LoadAsset<GameObject>("emptyCraft.prefab");
            KSPCraftCreator.EmptyCraftPrefab = emptyCraft;
            

            var partsPrefabs = partsBundle.LoadAllAssets<GameObject>();
            for(int i = 0; i < partsPrefabs.Length; i++) 
            {
                var part = partsPrefabs[i];
                KSPCraftCreator.PiecesPrefabs[part.name] = part;
            }

            string rootDirectoryPath = ModHelper.Manifest.AssemblyPath;

            ModHelper.Console.WriteLine($"Path {Path.GetDirectoryName(rootDirectoryPath)}");
            string[] craftFiles = Directory.GetFiles(Path.GetDirectoryName(rootDirectoryPath), "*.craft");
            ModHelper.Console.WriteLine($"Found {craftFiles.Length} files");

            for (int i = 0; i < craftFiles.Length; i++) 
            {
                ModHelper.Console.WriteLine($"Loading {craftFiles[i]} . . .");
                var configFile = CFGParser.ParseConfigFile(craftFiles[i]);
                GameObject craftPrefab = KSPCraftCreator.GenerateCraft(configFile, out string craftName);
                ModHelper.Console.WriteLine($"Added {craftName} as ship");
                ShipSpawnerManager.AddShip(craftPrefab, craftName);
            }
        }

    }
}