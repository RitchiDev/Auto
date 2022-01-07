using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : LaunchableItem
{
    public override void Use()
    {
        LaunchRocket();
    }

    private void LaunchRocket()
    {
        RocketProjectile rocket = Instantiate(m_ProjectilePrefab, m_OwnerItemController.CurrentFirepoint.position, transform.rotation).GetComponent<RocketProjectile>();
        rocket.SetOwner(m_Owner);
        rocket.Launch(m_LaunchDirection, m_LaunchPower);
    }
}
