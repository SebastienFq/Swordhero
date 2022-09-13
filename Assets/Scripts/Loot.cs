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

    private Item m_Item;
    private LootType m_LootType;

    public void Init(Item _Item, object _LootType)
    {
        m_Item = _Item;
        Debug.Log(_LootType);
        //m_LootType = _LootType;
    }
}
