using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
//using WorldGeneration;

public enum GamePhase
{
	NONE,
	MAIN_MENU,
	GAME,
	SUCCESS,
	FAILURE,
    RESET,
}

public class FSMManager : SingletonMB<FSMManager>
{
	public delegate void OnGamePhaseChanged(GamePhase _Phase);
	public static event OnGamePhaseChanged onGamePhaseStarted;
	public static event OnGamePhaseChanged onGamePhaseEnded;

	private MainMenuView m_MainMenuView;
	private ProgressionView m_ProgressionView;
	private SuccessView m_SuccessView;
	private FailureView m_FailureView;
	private float m_GameTime;

	public GamePhase CurrentPhase { get; private set; }

	private void Awake()
    {
		m_MainMenuView = MainMenuView.Instance;
		m_ProgressionView = ProgressionView.Instance;
		m_SuccessView = SuccessView.Instance;
		m_FailureView = FailureView.Instance;

		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		ChangePhase(GamePhase.RESET);
	}

	private void Update()
	{
		
		if (Input.GetMouseButtonDown(0))
		{
			switch (CurrentPhase)
			{
				case GamePhase.MAIN_MENU:
					ChangePhase(GamePhase.GAME);
					break;
			}
		}
	
	}

	/// <summary>
	/// Changes the phase.
	/// </summary>
	/// <param name="_GamePhase">Game phase.</param>
	public void ChangePhase(GamePhase _GamePhase)
	{
		switch (CurrentPhase)
		{
			case GamePhase.MAIN_MENU:
				m_MainMenuView.Hide();
				break;
			case GamePhase.GAME:
				m_ProgressionView.Hide();
				break;
			case GamePhase.FAILURE:
				m_FailureView.Hide();
				break;
			case GamePhase.SUCCESS:
				m_SuccessView.Hide();
				break;
			case GamePhase.RESET:
				break;
		}
		
		onGamePhaseEnded?.Invoke(CurrentPhase);
		
        switch (_GamePhase)
        {
			case GamePhase.MAIN_MENU:
				m_MainMenuView.Show();
				break;
			case GamePhase.GAME:
				m_GameTime = Time.time;
				m_ProgressionView.Show();
				break;
			case GamePhase.FAILURE:
				Debug.Log("Game time : " + (Time.time - m_GameTime).ToString("0"));
				m_FailureView.Show();
				break;
			case GamePhase.SUCCESS:
				Debug.Log("Game time : " + (Time.time - m_GameTime).ToString("0"));
				m_SuccessView.Show();
				break;
			case GamePhase.RESET:
				m_SuccessView.Hide();
				m_FailureView.Hide();
				m_MainMenuView.Hide();
				m_ProgressionView.Hide();
				break;
        }
        
		onGamePhaseStarted?.Invoke(_GamePhase);

		CurrentPhase = _GamePhase;

        if (_GamePhase == GamePhase.RESET)
        {
	        ChangePhase(GamePhase.MAIN_MENU);
        }
	}
}
