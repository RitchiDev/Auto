using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeItemBoxLauncher : LaunchableItem
{
    public override void Use()
    {
        SetFakeItemBoxActive();
    }

    private void SetFakeItemBoxActive()
    {
        FakeItemBox fakeItemBox = Instantiate(m_ProjectilePrefab, m_OwnerItemController.CurrentFirepoint.position, Quaternion.identity).GetComponent<FakeItemBox>();
        fakeItemBox.SetOwner(m_Owner);
    }
}
