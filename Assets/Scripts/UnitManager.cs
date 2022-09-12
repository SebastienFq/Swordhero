using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : SingletonMB<UnitManager>
{
    private const float c_DistanceUpdateRate = 0.2f;

    [Header("Scene References")]
    [SerializeField] private PlayerController m_Player;

    [Header("Project References")]
    [SerializeField] private EnemyController m_EnemyPrefab;

    [Header("Settings")]
    [SerializeField] private int m_MaxEnemiesInLevel = 10;
    [SerializeField] private float m_MinSpawnCooldown = 1f;
    [SerializeField] private float m_MaxSpawnCooldown = 5f;

    private float m_SpawnCooldown = -1f;
    private int m_SpawnCounter = 0;
    private int m_EnemiesRemainingToKill;
    private float m_DistanceUpdateTimer;
    private List<EnemyController> m_Enemies = new List<EnemyController>();

    public static Action<bool, EnemyController> onUnitsUpdated;

    private void Awake()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
        FSMManager.onGamePhaseEnded += OnGamePhaseEnded;
        EnemyController.onDeath += OnEnemyDeath;
        Spawner.onEnemySpawned += OnEnemySpawned;
    }

    private void OnDestroy()
    {
        FSMManager.onGamePhaseStarted -= OnGamePhaseStarted;
        FSMManager.onGamePhaseEnded -= OnGamePhaseEnded;
        EnemyController.onDeath -= OnEnemyDeath;
        Spawner.onEnemySpawned -= OnEnemySpawned;
    }

    private void Update()
    {
        if (FSMManager.Instance.CurrentPhase != GamePhase.GAME)
            return;

        m_DistanceUpdateTimer -= Time.deltaTime;

        if (m_DistanceUpdateTimer <= 0)
        {
            SortEnemiesByDistanceFromPlayer();
            m_DistanceUpdateTimer = c_DistanceUpdateRate;
        }

        if (m_SpawnCounter < m_MaxEnemiesInLevel)
        {
            m_SpawnCooldown -= Time.deltaTime;

            if (m_SpawnCooldown <= 0f)
            {
                SpawnEnemies();
            } 
        } 
    }

    private void OnGamePhaseEnded(GamePhase _Phase)
    {
        switch (_Phase)
        {
            case GamePhase.GAME:
                SetUnitsInvincible();
                break;
            case GamePhase.SUCCESS:
            case GamePhase.FAILURE:
                CleanEnemies();
                break;
        }
    }

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch (_Phase)
        {
            case GamePhase.RESET:
                m_EnemiesRemainingToKill = m_MaxEnemiesInLevel;
                m_SpawnCounter = 0;
                m_SpawnCooldown = -1f;
                break;
        }
    }

    private void SortEnemiesByDistanceFromPlayer()
    {
        for(int i = 0; i<m_Enemies.Count; i++)
        {
            var e = m_Enemies[i];
            e.SetDistanceFromPlayer(Vector3.Distance(e.transform.position, m_Player.transform.position));
        }

        m_Enemies.Sort((a, b) => a.DistanceFromPlayer.CompareTo(b.DistanceFromPlayer));

        onUnitsUpdated?.Invoke(m_Enemies.Count > 0, m_Enemies.Count > 0 ? m_Enemies[0] : null);
    }

    private void SetUnitsInvincible()
    {
        if (m_Player != null)
            m_Player.Health.SetInvincibility(true);

        foreach (EnemyController enemy in m_Enemies)
        {
            if (enemy != null)
                enemy.Health.SetInvincibility(true);
        }
    }

    private void CleanEnemies()
    {
        for (int i = m_Enemies.Count - 1; i >= 0; i--)
        {
            Destroy(m_Enemies[i].gameObject);
        }

        m_Enemies.Clear();
    }

    private void SpawnEnemies()
    {
        List<Spawner> spawners = new List<Spawner>(LevelManager.Instance.Spawners);

        for(int i= spawners.Count - 1; i >= 0; i--)
        {
            if (!spawners[i].IsAvailable)
                spawners.RemoveAt(i);
        }

        if (spawners.Count == 0)
            return;

        spawners.PickRandomElementInList().Spawn(m_EnemyPrefab);
        m_SpawnCounter++;
    }

    private void OnEnemyDeath(EnemyController _Enemy)
    {
        m_SpawnCounter--;
        m_Enemies.Remove(_Enemy);
        Destroy(_Enemy);
        m_EnemiesRemainingToKill--;

        if (m_EnemiesRemainingToKill <= 0)
        {
            FSMManager.Instance.ChangePhase(GamePhase.SUCCESS);
        }
    }

    private void OnEnemySpawned(Spawner _Spawner, EnemyController _Enemy)
    {
        m_Enemies.Add(_Enemy);
    }
}
