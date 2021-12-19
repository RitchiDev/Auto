using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicStarter : MonoBehaviour
{
    [SerializeField] private AudioKey m_AudioKey;

    private void Start()
    {
        Andrich.UtilityScripts.AudioManager.Instance.Play(m_AudioKey);
    }
}
