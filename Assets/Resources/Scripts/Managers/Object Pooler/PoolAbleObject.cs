using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Create New Pool Able Object")]
public class PoolAbleObject : ScriptableObject
{
    [SerializeField] private GameObject m_Prefab;
    public GameObject Prefab => m_Prefab;

    [SerializeField] private float m_CopyAmount;
    public float CopyAmount => m_CopyAmount;
}
