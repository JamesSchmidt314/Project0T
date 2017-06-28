using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
	public GameState m_gameState = GameState.Loading;
	private IGameState activeState;
	public static bool m_FullyInitialized = false;
	public AudioListener audioListener;

	private static float m_OriginalTimeScale = 1.0f;

	public static float originalTimeScale
	{
		get { return m_OriginalTimeScale;}
	}

	public static bool fullyInitialized
	{
		get{ return m_FullyInitialized;}
	}

	public static GameState gameState
	{
		get{ return instance.m_gameState;}
	}

	private IEnumerator Start()
	{
		SwitchState (new LoadingState ());
		//setup room manager
		while (!RoomManager.instance.readyToLoadScenes) 
		{
			yield return new WaitForEndOfFrame();
		}
		//setup Player Data
		while (!PlayerDataManager.instance.saveFileLoaded) 
		{
			yield return new WaitForEndOfFrame();
		}

		//setup Localization Data
		LocalizationManager.instance.LoadLocalizedText(PlayerDataManager.instance.saveFile.localizationFileName);
		while (!LocalizationManager.instance.localizationReady) 
		{
			yield return new WaitForEndOfFrame();
		}

		//settings are to be initialized at this point:
		GUIFormatter.instance.settingsMenu.InitializeSettings();
		GUIFormatter.instance.settingsMenu.gameObject.SetActive (false);
		m_FullyInitialized = true;

		if (RoomManager.instance.startFromSpecifiedRoom) 
		{
			yield return RoomManager.instance.LoadSceneAndSetActive (RoomManager.instance.specifiedRoomName);
		}
		else
		{
			yield return RoomManager.instance.LoadSceneAndSetActive (RoomManager.initSceneName);
			SwitchState (new MenuState ());
		}
	}

	void Update()
	{
		if (activeState != null) 
		{
			activeState.StateUpdate ();
		}
	}

	void FixedUpdate()
	{
		if (activeState != null) 
		{
			activeState.StateFixedUpdate ();
		}
	}

	public void SwitchState(IGameState newState)
	{
		if (activeState != null)
			activeState.StateStop ();
		if (newState.GetState () == GameState.Menu)
			MenuState.lastKnownState = activeState;
		activeState = newState;
		m_gameState = activeState.GetState ();
		activeState.StateStart ();
		Debug.Log ("Current state: " + activeState.GetState ());
	}

	public IGameState GetState(GameState state)
	{
		switch (state)
		{
		case GameState.Battle:
			return new BattleState();
		case GameState.Cutscene:
			return new CutsceneState();
		case GameState.Interaction:
			return new InteractiveState();
		case GameState.Loading:
			return new LoadingState();
		case GameState.MainMenu:
			return new MenuState();
		case GameState.Menu:
			return new MenuState(true);
		case GameState.Overworld:
			return new OverworldState();
		default:
			return new MenuState();
		}
	}
}