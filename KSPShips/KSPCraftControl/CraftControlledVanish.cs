using UnityEngine;
using SlateShipyard.VanishObjects;
using KSPShips.KSPCraftControl;

namespace Spaceshipinha.Navinha
{
    public class CraftControlledVanish : ControlledVanishObject
    {
        public OWRigidbody rigidbody;
        public MainCraftControl mainCraftControl;
        public void Awake()
        {
            DestroyComponentsOnGrow = false;
            VanishVolumesPatches.OnConditionsForPlayerToWarp += OnConditionsForPlayerToWarp;
        }
        public void OnDestroy()
        {
            VanishVolumesPatches.OnConditionsForPlayerToWarp -= OnConditionsForPlayerToWarp;
        }
        public override bool OnDestructionVanish(DestructionVolume destructionVolume)
        {
            if (mainCraftControl.IsPlayerControlingCraft)
            {
                Locator.GetDeathManager().KillPlayer(destructionVolume._deathType);
                return false;
            }
            return true;
        }
        public override bool OnSupernovaDestructionVanish(SupernovaDestructionVolume supernovaDestructionVolume)
        {
            return OnDestructionVanish(supernovaDestructionVolume);
        }
        public override bool OnBlackHoleVanish(BlackHoleVolume blackHoleVolume, RelativeLocationData entryLocation)
        {
            blackHoleVolume._whiteHole.ReceiveWarpedBody(rigidbody, entryLocation);
            return false;
        }
        public override bool OnWhiteHoleReceiveWarped(WhiteHoleVolume whiteHoleVolume, RelativeLocationData entryData)
        {
            whiteHoleVolume.SpawnImmediately(rigidbody, entryData);
            return false;
        }
        public override void OnWhiteHoleSpawnImmediately(WhiteHoleVolume whiteHoleVolume, RelativeLocationData entryData, out bool playerPassedThroughWarp)
        {
            playerPassedThroughWarp = false;
            if (Time.time > whiteHoleVolume._lastShipWarpTime + Time.deltaTime)
            {
                whiteHoleVolume._lastShipWarpTime = Time.time;
                if (mainCraftControl.IsPlayerControlingCraft)
                {
                    playerPassedThroughWarp = true;
                    whiteHoleVolume.MakeRoomForBody(rigidbody);
                }
            }
        }
        public override bool OnTimeLoopBlackHoleVanish(TimeLoopBlackHoleVolume timeloopBlackHoleVolume)
        {
            if (mainCraftControl.IsPlayerControlingCraft)
            {
                Locator.GetDeathManager().KillPlayer(DeathType.TimeLoop);
                return false;
            }
            return true;
        }
        private bool OnConditionsForPlayerToWarp()
        {
            if (mainCraftControl != null) {
                return !mainCraftControl.IsPlayerControlingCraft; 
            }
            return true;
        }
    }
}
