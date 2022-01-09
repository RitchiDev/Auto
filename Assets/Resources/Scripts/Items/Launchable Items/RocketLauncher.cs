using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : LaunchableItem
{
    public override void Use()
    {
        if(!m_ChildPhotonView.IsMine)
        {
            return;
        }

        LaunchRocket();
    }

    private void LaunchRocket()
    {
        //Debug.Log("Test");
        object[] data = new object[] { m_Owner, m_LaunchDirection, m_LaunchPower};
        byte group = 0;
        //RocketProjectile rocket = Instantiate(m_ProjectilePrefab, m_OwnerItemController.CurrentFirepoint.position, transform.rotation).GetComponent<RocketProjectile>();

        RocketProjectile rocket = PhotonPoolManager.Instance.NetworkInstantiate(m_ProjectilePrefab.name, m_OwnerItemController.CurrentFirepoint.position, transform.rotation, group, data).GetComponent<RocketProjectile>();
        rocket.SetOwner(m_Owner);
        rocket.Launch(m_LaunchDirection, m_LaunchPower);
    }
}
