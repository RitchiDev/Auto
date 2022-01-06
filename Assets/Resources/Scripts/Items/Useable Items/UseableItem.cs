using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseableItem : Item
{
    [SerializeField] protected float m_UseTime;
    protected IEnumerator m_UseTimerCoroutine;

    private void OnDisable()
    {
        if (m_UseTimerCoroutine != null)
        {
            StopCoroutine(m_UseTimerCoroutine);
        }
    }

    public abstract IEnumerator UseTimer();
}
