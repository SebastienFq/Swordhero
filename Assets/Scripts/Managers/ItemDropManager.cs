using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemDropManager : SingletonMB<ItemDropManager>
{
    [Header("References")]
    [SerializeField] private List<WeaponData> m_StartingWepaons = new List<WeaponData>();
    [SerializeField] private Loot m_LootPrefab = null;
    [SerializeField] private List<WeightedItemData> m_ItemLootTable = new List<WeightedItemData>();

    [Header("Settings")]
    [SerializeField] private bool m_ActivateLoot = false;
    [SerializeField] [Range(0, 1f)] private float m_DropRate = 0.3f;

    private float m_TotalLootTableWeight;

    public List<WeaponData> StartingWepaons => m_StartingWepaons;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
        EnemyController.onDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;
        EnemyController.onDeath -= OnEnemyDeath;
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch(_Phase)
        {
            case GamePhase.RESET:
                InitLootTable();
                break;
        }
        
    }

    private void InitLootTable()
    {
        if (m_ItemLootTable.Count == 0)
            Debug.LogError("Loot table is empty");

        m_TotalLootTableWeight = 0;

        for (int i = 0; i < m_ItemLootTable.Count; i++)
        {
            m_TotalLootTableWeight += m_ItemLootTable[i].m_Weight;
        }
    }

    private bool Roll(float _Power)
    {
        return Random.value <= _Power;
    }

    private void OnEnemyDeath(EnemyController _Enemy)
    {
        // modify value depending fo the enemy power
        if(Roll(m_DropRate) && m_ActivateLoot)       
        {
            var random = Random.insideUnitCircle * 2;
            random.Normalize();

            NavMeshHit hit;

            if(NavMesh.SamplePosition(_Enemy.transform.position + new Vector3(random.x, 0, random.y), out hit, 2.5f, NavMesh.AllAreas))
                DropLoot(hit.position);
        }
    }

    private void DropLoot(Vector3 _Position)
    {
        var randomValue = Random.value * m_TotalLootTableWeight;
        var currentTotalWeightValue = 0f;
        ItemData itemData = m_ItemLootTable[0].m_ItemData;

        for (int i=0; i< m_ItemLootTable.Count; i++)
        {
            if(currentTotalWeightValue >= randomValue)
            {
                itemData = m_ItemLootTable[i].m_ItemData;
                break;
            }
            else
            {
                currentTotalWeightValue += m_ItemLootTable[i].m_Weight;
            }
        }

        var loot = Instantiate(m_LootPrefab, _Position, Quaternion.identity);
        loot.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
        Item item = Instantiate(itemData.m_Item, loot.transform);
        item.Init(itemData);
        loot.Init(item, item.GetType());
    }
}

