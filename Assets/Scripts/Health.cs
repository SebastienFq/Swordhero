using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("HealthBarUI")]
    [SerializeField] private bool m_DisplayHealthBar = false;
    [SerializeField] private bool m_ShowAtMaxHealth = false;
    [SerializeField] private HealthBarUI m_HealthBarUIPrefab = null;
    [SerializeField] private Vector3 m_HealthBarPosition = Vector3.zero;

    private HealthBarUI m_HealthBarUI;
    private int m_MaxHealth;
    private int m_CurrentHealth;
    private bool m_IsInvincible = false;

    public int Value => m_CurrentHealth;
    public HealthBarUI HealthBarUI => m_HealthBarUI;

    public static Action<HealthBarUI> onHealthBarCreated;
    public static Action<HealthBarUI> onHealthBarDestroyed;
    public Action onDeath;
    
    public void Init(int _maxHealth)
    {
        m_MaxHealth = _maxHealth;
        
        if(m_DisplayHealthBar)
        {
            CreateHealthBarUI();
        }

        SetHealth(_maxHealth);
    }

    private void CreateHealthBarUI()
    {
        m_HealthBarUI = Instantiate(m_HealthBarUIPrefab);
        m_HealthBarUI.Init(transform, m_HealthBarPosition);
        onHealthBarCreated?.Invoke(m_HealthBarUI);
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
            m_HealthBarUI.UpdateVisibility((m_CurrentHealth < m_MaxHealth || m_ShowAtMaxHealth) && m_CurrentHealth > 0);
            m_HealthBarUI.UpdateFillAmount(m_CurrentHealth / (float)m_MaxHealth);
            m_HealthBarUI.UpdateText(m_CurrentHealth.ToString());
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
            m_HealthBarUI.UpdateVisibility((m_CurrentHealth < m_MaxHealth || m_ShowAtMaxHealth) && m_CurrentHealth > 0);
            m_HealthBarUI.UpdateFillAmount(m_CurrentHealth / (float)m_MaxHealth);
            m_HealthBarUI.UpdateText(m_CurrentHealth.ToString());
        }

        if (m_CurrentHealth == 0)
        {
            onHealthBarDestroyed?.Invoke(m_HealthBarUI);
            onDeath?.Invoke();
        }            
    }

    public void Destroy()
    {
        onHealthBarDestroyed?.Invoke(m_HealthBarUI);
    }
}
