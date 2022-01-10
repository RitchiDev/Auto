using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDiedItem : MonoBehaviour
{
    [SerializeField] private float m_TimeActive = 3f;
    [SerializeField] private TMP_Text m_PlayerNameText;
    [SerializeField] private TMP_Text m_DeathCauseText;

    public void SetUp(string name, string deathCause)
    {
        m_PlayerNameText.text = name;

        if(m_DeathCauseText)
        {
            m_DeathCauseText.text = deathCause;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DeactivateCountdown());
    }

    private IEnumerator DeactivateCountdown()
    {
        float totalTime = m_TimeActive;
        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
