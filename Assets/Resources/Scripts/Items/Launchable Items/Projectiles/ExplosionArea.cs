using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class ExplosionArea : MonoBehaviour
{
    [SerializeField] private float m_ActiveTime = 1f;
    private IEnumerator m_Deactivate;
    private Player m_Owner;
    public Player Owner => m_Owner;

    private void OnEnable()
    {
        m_Deactivate = DeactivateDelay();
        StartCoroutine(m_Deactivate);
    }

    public void SetOwner(Player owner)
    {
        m_Owner = owner;
    }

    private IEnumerator DeactivateDelay()
    {
        float totalTime = m_ActiveTime;

        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;

            yield return null;
        }

        Debug.Log("Deactivated Explosion");

        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
