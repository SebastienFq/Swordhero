using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private const float c_SpawnCooldown = 3;

    private float m_Cooldown = 0;
    private EnemyController m_EnemyPop;
    private bool m_IsAvailable = true;

    //public bool IsAvailable => m_Cooldown <= 0;
    public bool IsAvailable => m_IsAvailable;

    public static event UnityAction<Spawner, EnemyController> onEnemySpawned;

    private void OnEnable()
    {
        EnemyController.onDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyController.onDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath(EnemyController _Enemy)
    {
        if(_Enemy == m_EnemyPop)
        { 
            m_EnemyPop = null;
            m_IsAvailable = true;
        }
    }

    /*private void Update()
    {
        if(m_Cooldown > 0)
            m_Cooldown -= Time.deltaTime;
    }*/

    public void Spawn(EnemyController _Enemy)
    {
        var e = Instantiate(_Enemy, transform.position, transform.rotation);
        e.Init();
        //m_Cooldown = c_SpawnCooldown;
        m_EnemyPop = e;
        m_IsAvailable = false;
        onEnemySpawned?.Invoke(this, e);
    }
}
