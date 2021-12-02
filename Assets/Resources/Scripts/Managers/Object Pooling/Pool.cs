using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool : MonoBehaviour
{
	[SerializeField] private PoolAbleObject m_PoolObject;
	[SerializeField] private int m_IncreaseAmount;
	private List<GameObject> m_Objects;
	public GameObject Prefab => m_PoolObject.Prefab;
	public PoolAbleObject PoolObject => m_PoolObject;

	public void Initialize()
	{
		m_Objects = new List<GameObject>();
		for (int i = 0; i < m_PoolObject.CopyAmount; i++)
		{
			AddPrefab();
		}
	}

	private void IncreasePoolSize()
	{
		for (int i = 0; i < m_IncreaseAmount; i++)
		{
			AddPrefab();
		}
	}

	public GameObject GetObject()
	{
		GameObject selectedObject = null;
		for (int i = 0; i < m_Objects.Count; i++)
		{
			if (!m_Objects[i].activeInHierarchy)
			{
				selectedObject = m_Objects[i];
				break;
			}
		}

		if (selectedObject == null)
		{
			selectedObject = AddPrefab();
			IncreasePoolSize();
		}

		selectedObject.SetActive(true);
		return selectedObject;
	}

	private GameObject AddPrefab()
	{
		GameObject prefab = Instantiate(m_PoolObject.Prefab);
		prefab.SetActive(false);
		m_Objects.Add(prefab);
		return prefab;
	}
}
