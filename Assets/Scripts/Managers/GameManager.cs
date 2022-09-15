using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMB<GameManager>
{
    private int m_PlayerLevel = -1;

    public int PlayerLevel => m_PlayerLevel;

    private void OnEnable()
    {
        FSMManager.onGamePhaseStarted += OnGamePhaseStarted;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;

        m_PlayerLevel = PlayerPrefs.GetInt(Constants.GameplayValues.c_PlayerLevel, 0);
    }

#if UNITY_EDITOR

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            PlayerPrefs.DeleteAll();
        }
    }
#endif

    private void OnGamePhaseStarted(GamePhase _Phase)
    {
        switch(_Phase)
        {
            case GamePhase.RESET:
                LevelManager.Instance.ClearLevel();
                LevelManager.Instance.SpawnLevel(m_PlayerLevel);
                break;

            case GamePhase.SUCCESS:
                m_PlayerLevel++;
                PlayerPrefs.SetInt(Constants.GameplayValues.c_PlayerLevel, m_PlayerLevel);
                break;
        }
        
    }
}
