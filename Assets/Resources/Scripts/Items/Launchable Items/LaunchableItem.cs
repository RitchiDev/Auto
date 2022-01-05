using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LaunchableItem : Item
{
    [SerializeField] protected Vector3 m_LaunchDirection;
    [SerializeField] protected float m_LaunchPower;
}
