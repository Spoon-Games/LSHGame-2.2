﻿using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.Util
{
    [DisallowMultipleComponent]
    public class EffectsController : MonoBehaviour
    {
        protected Dictionary<string, IEffectTrigger> effectTriggers = new Dictionary<string, IEffectTrigger>();

        protected virtual void Awake()
        {
            LoadEffectTriggers(transform);
        }

        /// <summary>
        /// Use this function if you don't have the direct refrence to the VFXTrigger, like in an animator (VFXTriggerSMB).
        /// </summary>
        /// <param name="name">The name of the trigger</param>
        public void TriggerEffectParams(string name,Bundle parameters)
        {
            if (effectTriggers.TryGetValue(name, out IEffectTrigger trigger))
            {
                trigger.Trigger(parameters);
            }
            else
                Debug.Log("EffectTrigger " + name + " was not found");
        }

        public void TriggerEffect(string name)
        {
            TriggerEffectParams(name, null);
        }

        public void SetAllMaterialsToDefault()
        {
            foreach(var trigger in effectTriggers.Values)
            {
                trigger.SetMaterialToDefault();
            }
        }

        public void SetMaterial(string effect,string material)
        {
            if (effectTriggers.TryGetValue(effect, out IEffectTrigger trigger))
                trigger.SetMaterial(material);
            else
                Debug.Log("Can not set Material in " + this.name + ", effect " + effect + " does not exists");
        }

        public void StopEffect(string name)
        {
            if (effectTriggers.TryGetValue(name, out IEffectTrigger trigger))
            {
                trigger.Stop(); ;
            }
            else
                Debug.Log("EffectTrigger " + name + " was not found");
        }

        private void LoadEffectTriggers(Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<IEffectTrigger>(out IEffectTrigger t))
                {
                    t.AddToDict(effectTriggers);
                }
                else if (child.TryGetComponent<EffectsRelay>(out EffectsRelay r))
                {
                    LoadEffectTriggers(child);
                }
            }
        }
    }
    
    public interface IEffectTrigger
    {
        void AddToDict(Dictionary<string, IEffectTrigger> triggers);

        void SetMaterialToDefault();

        void SetMaterial(string material);

        void Trigger(Bundle parameters);

        void Stop();
    }


    public interface IEffectPlayer
    {
        void Play(Bundle parameters);

        void Stop();
    }
}
