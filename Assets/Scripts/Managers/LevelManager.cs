using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMB<LevelManager>
{
    [SerializeField] private Level[] m_Levels;

    private Level m_CurrentLevel = null;

    public Transform PlayerOrigin => m_CurrentLevel.m_PlayerOrigin;
    public List<Spawner> Spawners => m_CurrentLevel.m_Spawners;

    public static Action onLevelSpawned;

    public void SpawnLevel(int _PlayerLevel)
    {
        /*int idx = _PlayerLevel % m_Levels.Length;
        m_CurrentLevel = Instantiate(m_Levels[idx]);*/

        m_CurrentLevel = Instantiate(m_Levels.PickRandomElementInArray());

        onLevelSpawned?.Invoke();
    }

    public void ClearLevel()
    {
        if (m_CurrentLevel != null)
            Destroy(m_CurrentLevel.gameObject);
    }

}
