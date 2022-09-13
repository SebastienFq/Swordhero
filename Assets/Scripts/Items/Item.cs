using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemData m_ItemData;

    public ItemData Data => m_ItemData;

    public void Init(ItemData _itemData)
    {
        m_ItemData = _itemData;
    }
}
