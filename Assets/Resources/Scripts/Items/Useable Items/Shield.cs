using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : UseableItem
{
    private IEnumerator m_UseTimerCoroutine;

    public override void Use()
    {
        m_OwnerItemController.SetShielded(true);
        gameObject.SetActive(true);

        m_UseTimerCoroutine = UseTimer();
        StartCoroutine(m_UseTimerCoroutine);
    }

    public override IEnumerator UseTimer()
    {
        float totalTime = m_UseTime;

        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);

        m_OwnerItemController.SetShielded(false);
    }
}
