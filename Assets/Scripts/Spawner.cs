using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    public static event UnityAction<Spawner, EnemyController> onEnemySpawned;

    public void Spawn(EnemyController _Enemy)
    {
        var e = Instantiate(_Enemy, transform.position, transform.rotation);
        onEnemySpawned?.Invoke(this, e);
    }
}
