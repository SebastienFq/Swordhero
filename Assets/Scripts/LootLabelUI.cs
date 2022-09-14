using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootLabelUI : MonoBehaviour
{
    [SerializeField] private GameObject m_Background = null;
    [SerializeField] private TextMeshProUGUI m_Text = null;
    [SerializeField] private Vector3 m_Offset = Vector3.zero;

    private Transform m_Target;

    public Transform Target => m_Target;
    public Vector3 Offset => m_Offset;

    public void Init(Item _Item, Transform _Target)
    {
        m_Background.gameObject.SetActive(false);
        m_Text.text = _Item.Data.m_ItemName;
        m_Target = _Target;

        StartCoroutine(ShowLabel());
    }

    private IEnumerator ShowLabel()
    {
        yield return new WaitForSeconds(1);
        m_Background.gameObject.SetActive(true);
    }
}
