using UnityEngine;

namespace KSPShips.KSPCraftControl
{
    public class DecouplerPart : BaseKSPPart
    {
        public Transform decouplingForceDirection;

        OWRigidbody rigidbody;
        public float decoupleForce = 100;

        private bool isUsed = false;
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
            //Can't deactivate a decoupler
        }
        public virtual void FixedUpdate() 
        {
            if (rigidbody != null && IsPartActivated && !isUsed) 
            {
                //Before detach, on old MainCraftControl and old rigidbody
                rigidbody.AddImpulse(decouplingForceDirection.forward * decoupleForce, decouplingForceDirection.position);
                MainCraftControl.DetachPartsFromControl(this);
                //After detach, on new MainCraftControl and new rigidbody
                rigidbody.AddImpulse(-decouplingForceDirection.forward * decoupleForce, decouplingForceDirection.position);

                isUsed = true;
            }
        }
    }
}
