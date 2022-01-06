using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item Icon", menuName = "Create New Item Icon")]
public class ItemIcon : ScriptableObject
{
    [SerializeField] private Sprite m_ItemSprite;
    public Sprite ItemSprite => m_ItemSprite;

    [SerializeField] private Item.Type m_ItemType;
    public Item.Type ItemType => m_ItemType;
}
