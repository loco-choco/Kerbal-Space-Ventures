using UnityEngine;
using System.Collections.Generic;
using System;

namespace KSPShips.KSPCraftControl
{
    public class MainCraftControl : MonoBehaviour
    {
        int currentStage = -1;
        private Dictionary<string,BaseKSPPart> parts = new();
        public bool IsPlayerControlingCraft;

        public event Action<int> OnFireStage;

        private bool hasInitialized = false;
        public void Start() 
        {
            if (!hasInitialized)
                Init();
        }
        public void ForceSetCenterOfMass(BaseKSPPart[] parts)
        {
            Vector3 centerOfMass = Vector3.zero;
            float mass = 0f;
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                //Center Of Mass
                mass += part.Mass;
                Vector3 partCenterOfMass = part.transform.TransformPoint(part.LocalCenterOfMass);

                centerOfMass += partCenterOfMass * part.Mass;
            }
            centerOfMass /= mass;

            Vector3 localCenterOfMass = transform.InverseTransformPoint(centerOfMass);

            for (int i = 0; i < parts.Length; i++) //Move all the parts down or up instead of having the center of the parent not be the center of mass
            {
                parts[i].transform.localPosition -= localCenterOfMass;
            }
        }
        public void Init()
        {
            hasInitialized = true;

            OWRigidbody rigidbody = gameObject.GetAttachedOWRigidbody();

            BaseKSPPart[] foundParts = GetComponentsInChildren<BaseKSPPart>();

            Vector3 centerOfMass = rigidbody.GetCenterOfMass() * rigidbody.GetMass();
            float mass = rigidbody.GetMass();
            for (int i = 0; i < foundParts.Length; i++)
            {
                var part = foundParts[i];
                //Stages
                AddPartToStage(part);

                //Center Of Mass
                mass += part.Mass;
                Vector3 partCenterOfMass = part.transform.TransformPoint(part.LocalCenterOfMass);

                centerOfMass += partCenterOfMass * part.Mass;

                //Storing each part for easier access
                parts[part.name] = part;

                part.OnAttachedToMainCraftControl();
            }
            centerOfMass /= mass;

            Vector3 localCenterOfMass = transform.InverseTransformPoint(centerOfMass);
            rigidbody.SetMass(mass);
            rigidbody.SetCenterOfMass(Vector3.zero);

            for (int i = 0; i < foundParts.Length; i++) //Move all the parts down or up instead of having the center of the parent not be the center of mass
            {
                foundParts[i].transform.localPosition -= localCenterOfMass;
            }


            //Linking parts
            foreach (var partPair in parts)
            {
                var part = partPair.Value;
                for (int i = 0; i < part.InitialLinks.Length; i++)
                {
                    if (parts.TryGetValue(part.InitialLinks[i], out var linkedPart))
                        linkedPart.transform.parent = part.transform;
                }
            }
        }
        public void DetachPartsFromControl(BaseKSPPart rootPart)
        {
            DetachParts(rootPart);
            //We need to initially copy all the informations, so that when MainCraftControl Start is called, everything will detach nicely 
            GameObject detachedPartsControl = Instantiate(KSPCraftCreator.EmptyCraftPrefab,transform.position,transform.rotation);
            detachedPartsControl.name = name + "-detached";
            rootPart.transform.parent = detachedPartsControl.transform;

            detachedPartsControl.GetComponent<MainCraftControl>().Init();
        }
        private void DetachParts(BaseKSPPart rootPart)
        {
            BaseKSPPart[] linkedParts = rootPart.GetComponentsInChildren<BaseKSPPart>();
            for (int i = 0; i < linkedParts.Length; i++)
            {
                linkedParts[i].OnDetachedFromMainCraftControl();
                parts.Remove(linkedParts[i].ID);
            }
        }
        public void SetIsPlayerControlingCraft (bool isPlayerControlingCraft)
        {
            IsPlayerControlingCraft = isPlayerControlingCraft;
            OnPlayerControlingCraftChange?.Invoke(isPlayerControlingCraft);
        }

        public event Action<bool> OnPlayerControlingCraftChange;
        public void AddPartToStage(BaseKSPPart part) 
        {
            if(currentStage <= part.Stage) 
            {
                currentStage = part.Stage + 1;
            }
        }
        public void ActivateNextStage()
        {
            currentStage--;
            if (currentStage >= 0)
            {
                KSPShips.modHelper.Console.WriteLine($"Firing Stage {currentStage}");
                OnFireStage?.Invoke(currentStage); 
            }
        }
        public int GetCurrentStage()
        {
            return currentStage;
        }

    }
    public class CraftStage 
    {
        public List<BaseKSPPart> Parts = new();

        public void ActivateStage() 
        {
            for(int i = 0; i< Parts.Count; i++)
            {
                var part = Parts[i];
                if(part!=null)
                    part.ActivatePart();
            }
        }
    }
}
