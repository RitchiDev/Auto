using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseableItem : Item
{
    [SerializeField] protected float m_UseTime;

    public abstract IEnumerator UseTimer();
}
