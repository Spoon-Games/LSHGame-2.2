using UnityEngine;

namespace LSHGame.Util
{
    public class EffectLinearMapper : LinearMapper
    {
        [SerializeField]
        private string[] effects;

        protected override int ArrayLength { get => effects.Length; }

        private EffectsController effectsController;

        private EffectsController EffectsController
        {
            get
            {
                if (effectsController == null)
                    effectsController = animator.GetComponent<EffectsController>();
                return effectsController;
            }
        }

        protected override void SetIndex(int index)
        {
            EffectsController.TriggerEffect(effects[index]);
        }
    }
}
