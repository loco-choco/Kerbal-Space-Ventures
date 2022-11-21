using System.Collections.Generic;
using UnityEngine;

namespace KSPShips.KSPCraftControl
{
    public abstract class BaseKSPPart : MonoBehaviour
    {
        public float Mass;
        public Vector3 LocalCenterOfMass;
        public string ID;
        public string[] InitialLinks;
        protected MainCraftControl MainCraftControl;
        public bool IsPartActivated { get; protected set; }
        public int Stage;

        public bool IsAttachedToACraftControl { get; private set; } = false;

        public List<BaseKSPPart> attachedParts;
        public virtual void OnAttachedToMainCraftControl() 
        {
            MainCraftControl = transform.root.GetComponent<MainCraftControl>();
            MainCraftControl.OnFireStage += MainCraftControl_OnFireStage;
            IsAttachedToACraftControl = true;
        }

        private void MainCraftControl_OnFireStage(int stage)
        {
            if (Stage == stage)
                OnFireStage();
        }

        public virtual void OnFireStage() 
        {
            ActivatePart();
        }

        public virtual void OnDetachedFromMainCraftControl()
        {
            IsAttachedToACraftControl = false;
            MainCraftControl.OnFireStage -= MainCraftControl_OnFireStage;
            MainCraftControl = null;
        }
        public virtual void ActivatePart() 
        {
            IsPartActivated = true;
        }
        public virtual void DeactivatePart()
        {
            IsPartActivated = false;
        }
        public virtual void OnDrawGizmosSelected()
        {
            Vector3 center = transform.TransformPoint(LocalCenterOfMass);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(center, 0.2f);
        }
    }
}
