using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrich.UtilityScripts;

public class BackgroundMusicStarter : MonoBehaviour
{
    public static BackgroundMusicStarter Instance { get; private set; }
    [SerializeField] private AudioKey m_AudioKey;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("An instance of: " + ToString() + " already existed!");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Restart();
    }

    public void Restart()
    {
        //AudioManager.Instance.Stop(m_AudioKey);
        AudioManager.Instance.Play(m_AudioKey);
    }

    public void StopMusic()
    {
        AudioManager.Instance.Stop(m_AudioKey);
    }

    public void PlayMusic()
    {
        AudioManager.Instance.Play(m_AudioKey);
    }
}
