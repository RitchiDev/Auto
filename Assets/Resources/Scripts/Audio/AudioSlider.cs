using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Andrich.Audio
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private Slider m_Slider;
        [SerializeField] private AudioMixerGroup m_AudioMixerGroup;
        //Instead of setting the slider min / max values to -80 and 20, set them to min 0.0001 and max 1.

        private void OnEnable()
        {
            m_Slider.value = AudioManager.Instance.GetAudioMixerVolume(m_AudioMixerGroup);
            m_Slider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        }

        private void OnDisable()
        {
            m_Slider.onValueChanged.RemoveListener(delegate { ChangeVolume(); });
        }

        private void ChangeVolume()
        {
            AudioManager.Instance.SetAudioMixerVolume(m_AudioMixerGroup, m_Slider.value);
        }
    }
}
