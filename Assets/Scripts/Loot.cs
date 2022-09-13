using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public enum LootType
    {
        WEAPON,
        CURRENCY
    }

    [SerializeField] private Transform m_ItemSlot;

    private Item m_Item;
    private LootType m_LootType;

    public void Init(Item _Item, object _LootType)
    {
        m_Item = _Item;

        _Item.transform.SetParent(m_ItemSlot);
        _Item.transform.localRotation = Quaternion.Euler(90, 0, 0);
        _Item.transform.localPosition = Vector3.zero;
        _Item.transform.localScale = Vector3.one;
    }
}
