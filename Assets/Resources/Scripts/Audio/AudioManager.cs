using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Andrich.Audio
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

        private void Start()
        {
            GetAudioVolumes();   
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void GetAudioVolumes()
        {

            if (PlayerPrefs.HasKey(SettingsProperties.MasterVolumeProperty))
            {
                m_CurrentMasterVolume = PlayerPrefs.GetFloat(SettingsProperties.MasterVolumeProperty);
                SetAudioMixerVolume(m_Master, m_CurrentMasterVolume);
            }
            else
            {
                m_CurrentMasterVolume = 1f;
            }

            if (PlayerPrefs.HasKey(SettingsProperties.MusicVolumeProperty))
            {
                m_CurrentMusicVolume = PlayerPrefs.GetFloat(SettingsProperties.MusicVolumeProperty);
                SetAudioMixerVolume(m_Music, m_CurrentMusicVolume);
            }
            else
            {
                m_CurrentMusicVolume = 1f;
            }

            if (PlayerPrefs.HasKey(SettingsProperties.SFXVolumeProperty))
            {
                m_CurrentSFXVolume = PlayerPrefs.GetFloat(SettingsProperties.SFXVolumeProperty);
                SetAudioMixerVolume(m_SFX, m_CurrentSFXVolume);
            }
            else
            {
                m_CurrentSFXVolume = 1f;
            }

            //Debug.Log(m_CurrentMasterVolume);
            //Debug.Log(m_CurrentMusicVolume);
            //Debug.Log(m_CurrentSFXVolume);
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
            float valueToSet = Mathf.Log10(volume) * 20f;

            if (group == m_Master)
            {
                //Debug.Log(volume);
                m_Master.audioMixer.SetFloat(m_MasterKey, valueToSet);
                m_CurrentMasterVolume = volume;
                PlayerPrefs.SetFloat(SettingsProperties.MasterVolumeProperty, volume);
            }
            else if(group == m_Music)
            {
                m_Music.audioMixer.SetFloat(m_MusicKey, valueToSet);
                m_CurrentMusicVolume = volume;
                PlayerPrefs.SetFloat(SettingsProperties.MusicVolumeProperty, volume);
            }
            else if(group == m_SFX)
            {
                m_SFX.audioMixer.SetFloat(m_SFXKey, valueToSet);
                m_CurrentSFXVolume = volume;
                PlayerPrefs.SetFloat(SettingsProperties.SFXVolumeProperty, volume);
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
                currentVolume = PlayerPrefs.GetFloat(SettingsProperties.MasterVolumeProperty);
            }
            else if (group == m_Music)
            {
                currentVolume = PlayerPrefs.GetFloat(SettingsProperties.MusicVolumeProperty);
            }
            else if (group == m_SFX)
            {
                currentVolume = PlayerPrefs.GetFloat(SettingsProperties.SFXVolumeProperty);
            }
            else
            {
                Debug.LogError(group + ": Does not exis!");
            }

            return currentVolume;
        }
    }
}
