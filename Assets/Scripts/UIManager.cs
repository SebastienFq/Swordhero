using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private RectTransform m_HealthBarsParent;
    [SerializeField] private RectTransform m_LootLabelsParent;

    private List<HealthBarUI> m_HealthBars = new List<HealthBarUI>();
    private List<LootLabelUI> m_LootLabels = new List<LootLabelUI>();

    private void OnEnable()
    {
        Health.onHealthBarCreated += OnHealthBarCreated;
        Health.onHealthBarDestroyed += OnHealthBarDestroyed;

        Loot.onLootCreated += OnLootCreated;
        Loot.onLootDestroyed += OnLootDestroyed;
    }

    private void OnDisable()
    {
        Health.onHealthBarCreated -= OnHealthBarCreated;
        Health.onHealthBarDestroyed -= OnHealthBarDestroyed;

        Loot.onLootCreated += OnLootCreated;
        Loot.onLootDestroyed += OnLootDestroyed;

    }

    private void LateUpdate()
    {
        foreach(HealthBarUI _HealthBar in m_HealthBars)
        {
            MoveHealthBar(_HealthBar);
        }

        foreach (LootLabelUI _LootLabel in m_LootLabels)
        {
            MoveLootLabel(_LootLabel);
        }
    }

    private void OnHealthBarCreated(HealthBarUI _HealthBar)
    {
        _HealthBar.transform.SetParent(m_HealthBarsParent, false);
        m_HealthBars.Add(_HealthBar);
        MoveHealthBar(_HealthBar);
    }

    private void OnHealthBarDestroyed(HealthBarUI _Target)
    {
        if (!m_HealthBars.Contains(_Target))
            return;

        m_HealthBars.Remove(_Target);
        Destroy(_Target.gameObject);
    }

    private void MoveHealthBar(HealthBarUI _HealthBar)
    {
        _HealthBar.transform.position = m_Camera.WorldToScreenPoint(_HealthBar.Target.position + _HealthBar.Offset);
    }

    private void OnLootCreated(LootLabelUI _LootLabel)
    {
        _LootLabel.transform.SetParent(m_HealthBarsParent, false);
        m_LootLabels.Add(_LootLabel);
        MoveLootLabel(_LootLabel);
    }

    private void OnLootDestroyed(LootLabelUI _LootLabel)
    {
        if (!m_LootLabels.Contains(_LootLabel))
            return;

        m_LootLabels.Remove(_LootLabel);
        Destroy(_LootLabel.gameObject);
    }

    private void MoveLootLabel(LootLabelUI _LootLabel)
    {
        _LootLabel.transform.position = m_Camera.WorldToScreenPoint(_LootLabel.Target.position + _LootLabel.Offset);
    }

}
