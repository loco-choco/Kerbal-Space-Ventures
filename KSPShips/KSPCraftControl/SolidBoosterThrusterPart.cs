using UnityEngine;

namespace KSPShips.KSPCraftControl
{
    public class SolidBoosterThrusterPart : BaseKSPPart
    {
        public Transform thrustSource;

        OWRigidbody rigidbody;
        public float fuel = 100;
        public float fuelConsumption = 10;
        public float thrustPower = 10;
        public virtual void Start() 
        {
            IsPartActivated = false;
        }
        public override void OnAttachedToMainCraftControl()
        {
            base.OnAttachedToMainCraftControl();
            rigidbody = gameObject.GetAttachedOWRigidbody();
        }
        public override void OnDetachedFromMainCraftControl()
        {
            base.OnDetachedFromMainCraftControl();
            rigidbody = null;
        }
        public override void DeactivatePart()
        {
            //Can't deactivate a solid booster
        }
        public virtual void FixedUpdate() 
        {
            if (rigidbody != null && IsPartActivated && fuel > 0f) 
            {
                rigidbody.AddAcceleration(thrustSource.forward * thrustPower, thrustSource.position);

                fuel -= fuelConsumption * Time.fixedDeltaTime;
            }
        }
    }
}
