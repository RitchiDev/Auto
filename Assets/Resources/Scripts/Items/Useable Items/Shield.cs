using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : UseableItem
{
    public override void Use()
    {
        m_OwnerItemController.SetShielded(true);
        gameObject.SetActive(true);

        if(m_UseTimerCoroutine != null)
        {
            StopCoroutine(m_UseTimerCoroutine);
        }

        m_UseTimerCoroutine = UseTimer();
        StartCoroutine(m_UseTimerCoroutine);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);

        m_OwnerItemController.SetShielded(false);
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
