using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Andrich.UtilityScripts;
using Photon.Realtime;

public class ItemController : MonoBehaviourPunCallbacks
{
    [Header("Components")]
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private InGameUI m_InGameUI;
    [SerializeField] private EliminationPlayerController m_PlayerController;
    public Player Owner => m_PlayerController.Owner;

    [Header("Firepoints")]
    [SerializeField] private Transform m_FrontFirepoint;
    [SerializeField] private Transform m_MiddleFirepoint;
    [SerializeField] private Transform m_BackFirepoint;
    [SerializeField] private Transform m_FurtherBackFirepoint;
    private Transform m_CurrentFirepoint;
    public Transform CurrentFirepoint => m_CurrentFirepoint;

    [Header("Controls")]
    [SerializeField] private KeyCode m_UseItemKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode m_UseItemKeySecondary = KeyCode.E;

    [Header("Item / UI")]
    [SerializeField] private int m_Loops = 2;
    [SerializeField] private float m_LoopTime = 1f;
    [SerializeField] private List<Item> m_ItemPrefabs = new List<Item>();
    private List<Item> m_Items = new List<Item>();
    private ItemData m_CurrentItemData;

    [Header("Misc")]
    [SerializeField] private GameObject m_ItemHolder;
    private bool m_IsShielded;
    private bool m_GameHasBeenWon;

    private void Awake()
    {
        m_InGameUI.EmptyItemImageSprite();
    }

    private void Start()
    {
        InstantiateItems();
    }

    private void Update()
    {
        if(!m_PhotonView.IsMine || m_GameHasBeenWon)
        {
            return;
        }

        if (Input.GetKeyDown(m_UseItemKey) || Input.GetKeyDown(m_UseItemKeySecondary))
        {
            if(!m_CurrentItemData)
            {
                return;
            }

            UseItem(m_CurrentItemData.ItemType);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(RoomProperties.GameHasBeenWonProperty))
        {
            m_GameHasBeenWon = PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon();
        }

        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    private void InstantiateItems()
    {
        for (int i = 0; i < m_ItemPrefabs.Count; i++)
        {
            Item item = Instantiate(m_ItemPrefabs[i], transform.position, transform.rotation);
            //Debug.Log(m_PlayerController.Player);
            item.SetOwner(m_PlayerController.Owner, this);

            item.SetActive(false);
            item.transform.SetParent(m_ItemHolder.transform);

            m_Items.Add(item);
        }
    }

    public void StartItemRoulette()
    {
        if(!m_PhotonView.IsMine)
        {
            return;
        }

        if(!m_CurrentItemData)
        {
            StartCoroutine(ItemRoulette());
        }
    }

    private IEnumerator ItemRoulette()
    {
        float count = m_Items.Count;
        float time = m_LoopTime;

        for (int l = 0; l < m_Loops; l++)
        {
            for (int i = 0; i < count; i++)
            {
                // Set New Image Sprite
                m_InGameUI.SetItemImageSprite(m_Items[i].ItemData.ItemSprite);
                yield return new WaitForSeconds(time / count);
            }

            //Debug.Log("Looped: " + l);
        }


        m_CurrentItemData = GetRandomItemData();
        m_InGameUI.SetItemImageSprite(m_CurrentItemData.ItemSprite);

        Debug.Log("Item Set: " + m_CurrentItemData.ItemType);
        // Item Image = m_CurrentItem Image

    }

    private ItemData GetRandomItemData()
    {
        int index = Random.Range(0, m_Items.Count);
        return m_Items[index].ItemData;
    }

    public virtual void UseItem(Item.Type itemType)
    {
        if (PhotonNetwork.CurrentRoom.GetIfGameHasBeenWon())
        {
            return;
        }

        m_PhotonView.RPC("RPC_UseItem", RpcTarget.All, itemType);
    }

    [PunRPC]
    private void RPC_UseItem(Item.Type itemType)
    {
        Debug.Log("Used: " + itemType);

        SetFirepoint(itemType);

        for (int i = 0; i < m_Items.Count; i++)
        {
            if (itemType == m_Items[i].ItemData.ItemType)
            {
                m_Items[i].Use();
                break;
            }
        }

        m_InGameUI.EmptyItemImageSprite();
        m_CurrentItemData = null;
        m_CurrentFirepoint = m_MiddleFirepoint;
    }

    private void SetFirepoint(Item.Type itemType)
    {
        switch (itemType)
        {
            case Item.Type.notSet:
                Debug.LogError("Item Type Not Set");
                break;

            case Item.Type.rocket:
                m_CurrentFirepoint = m_FrontFirepoint;

                break;
            case Item.Type.shield:
                m_CurrentFirepoint = m_MiddleFirepoint;

                break;
            case Item.Type.demolitionAura:
                m_CurrentFirepoint = m_MiddleFirepoint;

                break;
            case Item.Type.fakeItemBox:
                m_CurrentFirepoint = m_FurtherBackFirepoint;

                break;
            default:
                break;
        }
    }

    public void SetShielded(bool value)
    {
        m_IsShielded = value;
    }

    public void HideItems()
    {
        for (int i = 0; i < m_Items.Count; i++)
        {
            m_Items[i].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckIfEnteredDemolitionAura(other);

        CheckIfHitByRocket(other);

        CheckIfHitByExplosionArea(other);

        CheckIfHitByFakeItemBox(other);
    }

    private void CheckIfEnteredDemolitionAura(Collider other)
    {
        if(m_IsShielded)
        {
            return;
        }

        DemolitionAura aura = other.GetComponent<DemolitionAura>();

        if(aura)
        {
            if(aura.Owner == m_PlayerController.Owner)
            {
                return;
            }

            aura.Owner.AddKO(1);
            aura.Owner.AddScore(250);

            if(m_PhotonView.IsMine)
            {
                KO(aura.Owner.NickName);
            }
        }
    }

    private void CheckIfHitByRocket(Collider other)
    {
        RocketProjectile rocket = other.GetComponent<RocketProjectile>();

        if (rocket)
        {
            if (rocket.Owner == m_PlayerController.Owner)
            {
                return;
            }

            if (m_IsShielded)
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].ItemData.ItemType == Item.Type.shield)
                    {
                        m_Items[i].SetActive(false);
                        m_IsShielded = false;
                        break;
                    }
                }

                return;
            }

            rocket.Owner.AddScore(250);
            rocket.Owner.AddKO(1);

            if(m_PhotonView.IsMine)
            {
                KO(rocket.Owner.NickName);
            }
        }
    }

    private void CheckIfHitByExplosionArea(Collider other)
    {
        ExplosionArea explosionArea = other.GetComponent<ExplosionArea>();

        if (explosionArea)
        {
            if (m_IsShielded)
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].ItemData.ItemType == Item.Type.shield)
                    {
                        m_Items[i].SetActive(false);
                        m_IsShielded = false;
                        break;
                    }
                }

                return;
            }

            if (explosionArea.Owner != m_PlayerController.Owner)
            {
                explosionArea.Owner.AddScore(250);
                explosionArea.Owner.AddKO(1);
            }

            if (m_PhotonView.IsMine)
            {
                KO(explosionArea.Owner.NickName);
            }
        }
    }

    private void CheckIfHitByFakeItemBox(Collider other)
    {
        FakeItemBox fakeItemBox = other.GetComponent<FakeItemBox>();

        if (fakeItemBox)
        {
            if (m_IsShielded)
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].ItemData.ItemType == Item.Type.shield)
                    {
                        m_Items[i].SetActive(false);
                        m_IsShielded = false;
                        break;
                    }
                }

                return;
            }

            if (fakeItemBox.Owner != m_PlayerController.Owner)
            {
                fakeItemBox.Owner.AddScore(250);
                fakeItemBox.Owner.AddKO(1);
            }

            if (m_PhotonView.IsMine)
            {
                KO(fakeItemBox.Owner.NickName);
            }
        }
    }

    public void KO(string deathcause)
    {
        m_PlayerController.KO(deathcause);
    }
}
