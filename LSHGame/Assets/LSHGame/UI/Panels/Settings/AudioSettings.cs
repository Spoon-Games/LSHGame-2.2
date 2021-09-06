using LSHGame.Util;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class AudioSettings : MonoBehaviour
    {
        [SerializeField]
        private Slider masterVolumeSlider;

        [SerializeField] 
        private Slider musicSlider;

        [SerializeField]
        private Slider gameSlider;



        void Start()
        {
            InitSlider(masterVolumeSlider,f => AudioManager.MasterVolume = f, AudioManager.MasterVolume);
            InitSlider(musicSlider, f => AudioManager.MusicVolume = f, AudioManager.MusicVolume);
            InitSlider(gameSlider, f => AudioManager.SFXVolume = f, AudioManager.SFXVolume);

        }

        private void InitSlider(Slider slider,Action<float> SetValue,float startValue)
        {
            slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, startValue);
            slider.onValueChanged.AddListener(f => SetValue(Mathf.InverseLerp(slider.minValue, slider.maxValue, f)));
        }


    } 
}
