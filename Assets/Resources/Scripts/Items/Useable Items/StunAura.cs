using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunAura : UseableItem
{
    [SerializeField] private SphereCollider m_StunArea;
    [SerializeField] private float m_MaxStunAreaRadius = 10f;
    [SerializeField] private GameObject m_StunEffect;
    [SerializeField] private Vector3 m_MaxStunEffectSize = new Vector3(13f, 13f, 13f);
    private Vector3 m_OriginalEffectSize;

    private void Awake()
    {
        m_OriginalEffectSize = m_StunEffect.transform.localScale;   
    }

    public override void Use()
    {
        gameObject.SetActive(true);

        if (m_UseTimerCoroutine != null)
        {
            StopCoroutine(m_UseTimerCoroutine);
        }

        m_UseTimerCoroutine = UseTimer();
        StartCoroutine(m_UseTimerCoroutine);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public override IEnumerator UseTimer()
    {
        float totalTime = m_UseTime;

        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;

            m_StunEffect.transform.localScale = Vector3.Lerp(m_StunEffect.transform.localScale, m_MaxStunEffectSize, totalTime / m_UseTime);

            if(m_StunArea)
            {
                m_StunArea.radius = Mathf.Lerp(m_StunArea.radius, m_MaxStunAreaRadius, totalTime / m_UseTime);
            }

            yield return null;
        }

        m_StunEffect.transform.localScale = m_OriginalEffectSize;
        gameObject.SetActive(false);
    }
}
