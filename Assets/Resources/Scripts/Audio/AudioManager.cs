using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Andrich.UtilityScripts
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Mixer Groups")]
        [SerializeField] private AudioMixerGroup m_Master;
        [SerializeField] private AudioMixerGroup m_Music;
        [SerializeField] private AudioMixerGroup m_SFX;
        
        [SerializeField] private List<Audio> m_AudioList = new List<Audio>();

        private static float m_CurrentMasterVolume;
        private static float m_CurrentMusicVolume;
        private static float m_CurrentSFXVolume;

        private string m_MasterKey = "Master";
        private string m_MusicKey = "Music";
        private string m_SFXKey = "SFX";

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("An instance of: " + Instance.ToString() + " already existed!");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            m_CurrentMasterVolume = 1f;
            m_CurrentMusicVolume = 1f;
            m_CurrentSFXVolume = 1f;

            foreach (Audio audio in m_AudioList)
            {
                audio.SetAudioSource(gameObject.AddComponent<AudioSource>());

                if(audio.AudioSource)
                {
                    audio.AudioSource.clip = audio.AudioClip;
                    audio.AudioSource.outputAudioMixerGroup = audio.Output;
                    audio.AudioSource.mute = audio.Mute;
                    audio.AudioSource.playOnAwake = audio.PlayOnWake;
                    audio.AudioSource.loop = audio.Loop;
                    audio.AudioSource.priority = audio.Priority;
                    audio.AudioSource.volume = audio.Volume;
                    audio.AudioSource.pitch = audio.Pitch;
                    audio.AudioSource.panStereo = audio.StereoPan;
                    audio.AudioSource.spatialBlend = audio.SpatialBlend;
                    audio.AudioSource.reverbZoneMix = audio.ReverbZone;
                }

                audio.AudioSource.Stop();
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];
                if(audio != null)
                {
                    audio.AudioSource.Stop();
                }
            }

            //Debug.Log("OnSceneLoaded: " + scene.name);
            //Debug.Log(mode);
        }

        public void Play(AudioKey key)
        {
            if (key == null)
            {
                Debug.LogWarning("Play sound: " + key.name + "is not found");
                return;
            }

            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];

                if(audio.AudioKey == key)
                {
                    audio.AudioSource.Play();
                }
            }
        }

        public void PlayOneShot(AudioKey key)
        {
            if (key == null)
            {
                Debug.LogWarning("Play sound: " + key.name + "is not found");
                return;
            }

            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];

                if (audio.AudioKey == key)
                {
                    audio.AudioSource.PlayOneShot(audio.AudioClip);
                }
            }
        }

        public void Stop(AudioKey key)
        {
            if (key == null)
            {
                Debug.LogWarning("Play sound: " + key.name + "is not found");
                return;
            }

            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];

                if (audio.AudioKey == key)
                {
                    audio.AudioSource.Stop();
                }
            }
        }

        public void Pause(AudioKey key)
        {
            if (key == null)
            {
                Debug.LogWarning("Pause sound: " + key.name + "is not found");
                return;
            }

            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];

                if (audio.AudioKey == key)
                {
                    audio.AudioSource.Pause();
                }
            }
        }

        public void UnPause(AudioKey key)
        {
            if (key == null)
            {
                Debug.LogWarning("Pause sound: " + key.name + "is not found");
                return;
            }

            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];

                if (audio.AudioKey == key)
                {
                    audio.AudioSource.UnPause();
                }
            }
        }

        public AudioSource GetAudioSource(AudioKey key)
        {
            AudioSource audioSource = null;

            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];

                if (audio.AudioKey == key)
                {
                    audioSource = audio.AudioSource;
                }
            }

            return audioSource;
        }

        public AudioClip GetAudioClip(AudioKey key)
        {
            AudioClip audioClip = null;

            for (int i = 0; i < m_AudioList.Count; i++)
            {
                Audio audio = m_AudioList[i];

                if (audio.AudioKey == key)
                {
                    audioClip = audio.AudioClip;
                }
            }

            return audioClip;
        }

        public void SetAudioMixerVolume(AudioMixerGroup group, float volume)
        {
            //Instead of setting the slider min / max values to -80 and 20, set them to min 0.0001 and max 1.
            if(group == m_Master)
            {
                m_Master.audioMixer.SetFloat(m_MasterKey, Mathf.Log10(volume) * 20f);
                m_CurrentMasterVolume = volume;
            }
            else if(group == m_Music)
            {
                m_Music.audioMixer.SetFloat(m_MusicKey, Mathf.Log10(volume) * 20f);
                m_CurrentMusicVolume = volume;
            }
            else if(group == m_SFX)
            {
                m_SFX.audioMixer.SetFloat(m_SFXKey, Mathf.Log10(volume) * 20f);
                m_CurrentSFXVolume = volume;
            }
            else
            {
                Debug.LogError(group + ": Does not exis!");
            }
        }

        public float GetAudioMixerVolume(AudioMixerGroup group)
        {
            float currentVolume = 0f;

            if (group == m_Master)
            {
                currentVolume = m_CurrentMasterVolume;
            }
            else if (group == m_Music)
            {
                currentVolume = m_CurrentMusicVolume;
            }
            else if (group == m_SFX)
            {
                currentVolume = m_CurrentSFXVolume;
            }
            else
            {
                Debug.LogError(group + ": Does not exis!");
            }

            return currentVolume;
        }
    }
}
