using System;
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
    [SerializeField] private Collider m_Collider;
    [SerializeField] private ParticleSystem m_Particles;
    [SerializeField] private LootLabelUI m_LootLabelUIPrefab;
    [SerializeField] private float m_Duration = 10;

    private Item m_Item;
    private LootType m_LootType;
    private float m_Timer;
    private LootLabelUI m_LootLabelUI;

    public static Action<LootLabelUI> onLootCreated;
    public static Action<LootLabelUI> onLootDestroyed;

    public Item Item => m_Item;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
    }

    private void OnDisable()
    {
        FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;
    }

    public void Init(Item _Item, object _LootType)
    {
        m_Item = _Item;

        m_Collider.enabled = false;

        _Item.transform.SetParent(m_ItemSlot);
        _Item.transform.localRotation = Quaternion.Euler(90, 0, 0);
        _Item.transform.localPosition = Vector3.zero;
        _Item.transform.localScale = Vector3.one;
        
        m_Timer = m_Duration;

        m_LootLabelUI = Instantiate(m_LootLabelUIPrefab);
        m_LootLabelUI.Init(_Item, transform);
        onLootCreated?.Invoke(m_LootLabelUI);

        StartCoroutine(DelayLootCollider());
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch (_Phase)
        {
            case GamePhase.RESET:
                Destroy();
                break;
        }
    }

    private IEnumerator DelayLootCollider()
    {
        yield return new WaitForSeconds(1);
        m_Collider.enabled = true;
    }

    private void Update()
    {
        m_Timer -= Time.deltaTime;

        if(m_Timer < 2 && m_Particles.isPlaying)
        {
            m_Particles.Stop();
        }

        if(m_Timer <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        StopAllCoroutines();
        onLootDestroyed?.Invoke(m_LootLabelUI);
        Destroy(gameObject);
    }
}
