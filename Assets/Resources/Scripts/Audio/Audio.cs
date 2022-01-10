using UnityEngine;
using UnityEngine.Audio;

namespace Andrich.Audio
{
    [System.Serializable]
    public class Audio
    {
        public AudioSource AudioSource { get; private set; }

        [SerializeField] private AudioKey m_AudioKey;
        public AudioKey AudioKey => m_AudioKey;

        [SerializeField] private AudioClip m_AudioClip;
        public AudioClip AudioClip => m_AudioClip;

        [SerializeField] private AudioMixerGroup m_Output;
        public AudioMixerGroup Output => m_Output;

        [SerializeField] private bool m_Mute = false;
        public bool Mute => m_Mute;

        [SerializeField] private bool m_PlayOnWake = false;
        public bool PlayOnWake => m_PlayOnWake;

        [SerializeField] private bool m_Loop = false;
        public bool Loop => m_Loop;

        [Range(0, 256)]
        [SerializeField] private int m_Priority = 128;
        public int Priority => m_Priority;

        [Range(0f, 1f)]
        [SerializeField] private float m_Volume = 1f;
        public float Volume => m_Volume;

        [Range(-3f, 3f)]
        [SerializeField] private float m_Pitch = 1f;
        public float Pitch => m_Pitch;

        [Range(-1f, 1f)]
        [SerializeField] private float m_StereoPan = 0f;
        public float StereoPan => m_StereoPan;

        [Range(0f, 1f)]
        [SerializeField] private float m_SpatialBlend = 0f;
        public float SpatialBlend => m_SpatialBlend;

        [Range(0f, 1.1f)]
        [SerializeField] private float m_ReverbZone = 1f;
        public float ReverbZone => m_ReverbZone;

        public void SetAudioSource(AudioSource audioSource)
        {
            AudioSource = audioSource;
        }
    }
}