using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private const float c_SpawnCooldown = 3;

    private float m_Cooldown = 0;

    public bool IsAvailable => m_Cooldown <= 0;

    public static event UnityAction<Spawner, EnemyController> onEnemySpawned;

    private void Update()
    {
        if(m_Cooldown > 0)
            m_Cooldown -= Time.deltaTime;
    }

    public void Spawn(EnemyController _Enemy)
    {
        var e = Instantiate(_Enemy, transform.position, transform.rotation);
        m_Cooldown = c_SpawnCooldown;
        onEnemySpawned?.Invoke(this, e);
    }
}
