using UnityEngine;

namespace KSPShips.KSPCraftControl
{
    public class CockpitPart : BaseKSPPart
    {
        public SingleInteractionVolume interactVolume;
        public PlayerAttachPoint attachPoint;
        private PlayerAudioController playerAudio;
        public override void OnAttachedToMainCraftControl()
        {
            base.OnAttachedToMainCraftControl();
            enabled = false;

            playerAudio = Locator.GetPlayerAudioController();

            interactVolume.OnPressInteract += OnPressInteract;
        }

        public virtual void OnDestroy()
        {
            interactVolume.OnPressInteract -= OnPressInteract;
        }
        public virtual void OnPressInteract()
        {
            if (!enabled)
            {
                playerAudio.PlayOneShotInternal(AudioType.ShipCockpitBuckleUp);
                attachPoint.AttachPlayer();
                interactVolume.DisableInteraction();
                enabled = true;
                MainCraftControl.SetIsPlayerControlingCraft(true);
            }
        }

        public virtual void Update()
        {
            if (OWInput.IsNewlyPressed(InputLibrary.cancel, InputMode.All))
            {
                attachPoint.DetachPlayer();
                playerAudio.PlayOneShotInternal(AudioType.ShipCockpitUnbuckle);
                interactVolume.EnableInteraction();
                interactVolume.ResetInteraction();
                enabled = false;
                MainCraftControl.SetIsPlayerControlingCraft(false);
            }
            else if(OWInput.IsNewlyPressed(InputLibrary.boost, InputMode.All))
            {
                MainCraftControl.ActivateNextStage();
            }
        }
    }
}
