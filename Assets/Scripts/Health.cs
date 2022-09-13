using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] private bool m_DisplayHealthBar = false;
    [SerializeField] private bool m_ShowAtMaxHealth = false;
    [SerializeField] private GameObject m_HealthBar = null;
    [SerializeField] private Image m_Fill;
    [SerializeField] private TextMeshProUGUI m_HealthText;

    private int m_MaxHealth;
    private int m_CurrentHealth;
    private bool m_IsInvincible = false;

    public int Value => m_CurrentHealth;

    public Action onDeath;
    
    public void Init(int _maxHealth)
    {
        m_MaxHealth = _maxHealth;
        m_HealthBar.SetActive(m_DisplayHealthBar);
        SetHealth(_maxHealth);
    }

    public void SetInvincibility(bool _IsInvincible)
    {
        m_IsInvincible = _IsInvincible;
    }

    public void SetHealth(int _value)
    {  
        m_CurrentHealth = _value;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, int.MaxValue);
        
        if (m_DisplayHealthBar)
        {
            m_HealthBar.SetActive((m_CurrentHealth < m_MaxHealth || m_ShowAtMaxHealth) && m_CurrentHealth > 0 );
            m_Fill.fillAmount = m_CurrentHealth / (float)m_MaxHealth;
            m_HealthText.text = m_CurrentHealth.ToString();
        }
      
    }

    public void AddHealth(int _value)
    {
        if (m_IsInvincible && _value < 0)
            return;

        m_CurrentHealth += _value;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_MaxHealth);

        if (m_DisplayHealthBar)
        {
            m_HealthBar.SetActive((m_CurrentHealth < m_MaxHealth || m_ShowAtMaxHealth) && m_CurrentHealth > 0);
            m_Fill.fillAmount = m_CurrentHealth / (float) m_MaxHealth;
            m_HealthText.text = m_CurrentHealth.ToString();
        }

        if(m_CurrentHealth == 0)
            onDeath?.Invoke();
    }
}
