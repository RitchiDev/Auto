using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private List<Pool> m_Pools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this);
        }

		Initialize();
    }

	public void Initialize()
	{
		foreach (Pool pool in m_Pools)
		{
			pool.Initialize();
		}
	}

	public GameObject GetObjectFromPool(PoolAbleObject poolObject)
	{
		foreach (Pool pool in m_Pools)
		{
			if(poolObject == pool.PoolObject)
			{
				return pool.GetObject();
			}
		}

		Debug.LogError("The object pool with that prefab does not exist.");
		return null;
	}

	public GameObject GetObjectFromPool(PoolAbleObject poolObject, Transform parent)
	{
		foreach (Pool pool in m_Pools)
		{
			if (poolObject == pool.PoolObject)
			{
				GameObject pooledObject = pool.GetObject();
				pooledObject.transform.SetParent(parent, false);

				return pooledObject;
			}
		}

		Debug.LogError("The object pool with that prefab does not exist.");
		return null;
	}
}
