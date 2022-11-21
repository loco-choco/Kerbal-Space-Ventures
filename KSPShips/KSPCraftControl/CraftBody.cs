using KSPShips.KSPCraftControl;
using UnityEngine;

namespace Spaceshipinha.Navinha
{
    public class CraftBody : OWRigidbody
    {
        private bool _isPlayerAtFlightConsole;
        public MainCraftControl MainCraftControl;

        public override void Awake() 
        {
            MainCraftControl.OnPlayerControlingCraftChange += MainCraftControl_OnPlayerControlingCraftChange; 
            base.Awake();
        }

        private void MainCraftControl_OnPlayerControlingCraftChange(bool isPlayerControlingCraft)
        {
            _isPlayerAtFlightConsole = isPlayerControlingCraft;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainCraftControl.OnPlayerControlingCraftChange -= MainCraftControl_OnPlayerControlingCraftChange;
        }

		public override void SetPosition(Vector3 worldPosition)
		{
			if (_isPlayerAtFlightConsole)
			{
				base.SetPosition(worldPosition);
				GlobalMessenger.FireEvent("PlayerRepositioned");
				return;
			}
			base.SetPosition(worldPosition);
		}

		public override void SetRotation(Quaternion rotation)
		{
			base.SetRotation(rotation);
		}

		public override void SetVelocity(Vector3 newVelocity)
		{
			base.SetVelocity(newVelocity);
		}
	}
}
