using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private GameObject m_Background = null;
    [SerializeField] private Image m_Fill = null;
    [SerializeField] private TextMeshProUGUI m_Text = null;

    private Transform m_Target;
    private Vector3 m_Offset;

    public Transform Target => m_Target;
    public Vector3 Offset => m_Offset;

    public void Init(Transform _Target, Vector3 _Offset)
    {
        m_Target = _Target;
        m_Offset = _Offset;
    }

    public void UpdateVisibility(bool _IsVisible)
    {
        m_Background.SetActive(_IsVisible);       
    }

    public void UpdateText(string _Health)
    {
        m_Text.text = _Health;
    }

    public void UpdateFillAmount(float _Value)
    {
        m_Fill.fillAmount = _Value;
    }
}

