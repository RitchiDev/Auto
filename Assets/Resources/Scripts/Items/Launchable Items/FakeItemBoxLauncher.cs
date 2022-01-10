using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeItemBoxLauncher : LaunchableItem
{
    public override void Use()
    {
        if (!m_ChildPhotonView.IsMine)
        {
            return;
        }

        SetFakeItemBoxActive();
    }

    private void SetFakeItemBoxActive()
    {
        object[] data = new object[] { m_Owner, m_LaunchDirection, m_LaunchPower };
        byte group = 0;

        //FakeItemBox fakeItemBox = PhotonPoolManager.Instance.NetworkInstantiate(m_ProjectilePrefab.name, m_OwnerItemController.CurrentFirepoint.position, Quaternion.identity, group, data).GetComponent<FakeItemBox>();
        FakeItemBox fakeItemBox = PhotonPool.Instantiate(m_ProjectilePrefab.name, m_OwnerItemController.CurrentFirepoint.position, Quaternion.identity, group, data).GetComponent<FakeItemBox>();
        fakeItemBox.SetOwner(m_Owner);
    }
}
