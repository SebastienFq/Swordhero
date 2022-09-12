using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMB<GameManager>
{
    [SerializeField] private Transform m_PlayerSpawnPoint;

    public Transform PlayerSpawnPoint => m_PlayerSpawnPoint;
}
