using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemController : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private KeyCode m_UseItemKey = KeyCode.LeftShift;
    [SerializeField] private int m_Loops = 2;
    [SerializeField] private float m_LoopTime = 1f;
    [SerializeField] private List<ItemIcon> m_ItemIcons = new List<ItemIcon>();
    [SerializeField] private List<Item> m_ItemPrefabs = new List<Item>();
    private List<Item> m_Items = new List<Item>();
    private ItemIcon m_CurrentItem;

    private bool m_IsShielded;

    private void Awake()
    {
        InstantiateItems();
    }

    private void Update()
    {
        if(!m_PhotonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(m_UseItemKey))
        {
            if(!m_CurrentItem)
            {
                return;
            }

            UseItem(m_CurrentItem.ItemType);
        }
    }

    private void InstantiateItems()
    {
        for (int i = 0; i < m_ItemPrefabs.Count; i++)
        {
            Item item = Instantiate(m_ItemPrefabs[i], transform.position, m_ItemPrefabs[i].transform.rotation);
            item.SetOwner(gameObject, this);
            item.SetActive(false);
            item.transform.SetParent(transform);

            m_Items.Add(item);
        }
    }

    public void StartItemRoulette()
    {
        if(!m_CurrentItem)
        {
            StartCoroutine(ItemRoulette());
        }
    }

    private IEnumerator ItemRoulette()
    {
        float count = m_ItemIcons.Count;
        float time = m_LoopTime;

        for (int l = 0; l < m_Loops; l++)
        {
            for (int i = 0; i < count; i++)
            {
                // Set New Image
                yield return new WaitForSeconds(time / count);
            }

            Debug.Log("Looped: " + l);
        }


        m_CurrentItem = GetRandomItem();
        Debug.Log("Item Set: " + m_CurrentItem.ItemType);
        // Item Image = m_CurrentItem Image

    }

    private ItemIcon GetRandomItem()
    {
        int index = Random.Range(0, m_ItemIcons.Count - 1);
        return m_ItemIcons[index];
    }

    public virtual void UseItem(Item.Type itemType)
    {
        m_PhotonView.RPC("RPC_UseItem", RpcTarget.All, itemType);
    }

    [PunRPC]
    private void RPC_UseItem(Item.Type itemType)
    {
        Debug.Log("Used: " + itemType);

        switch (itemType)
        {
            case Item.Type.notSet:
                Debug.LogError("Item Type Not Set");
                break;
            case Item.Type.rocket:

                break;
            case Item.Type.shield:

                break;
            default:
                break;
        }

        for (int i = 0; i < m_Items.Count; i++)
        {
            if (m_CurrentItem.ItemType == m_Items[i].ItemType)
            {
                m_Items[i].Use();
                break;
            }
        }

        m_CurrentItem = null;
    }

    public void SetShielded(bool value)
    {
        m_IsShielded = value;
    }
}
