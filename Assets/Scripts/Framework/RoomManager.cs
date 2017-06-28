using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : PersistentSingleton<RoomManager>
{
	public bool startFromSpecifiedRoom;
	public string specifiedRoomName;
	public const string persistentSceneName = "Persistent";
	public const string initSceneName = "Scene_Menu01";
	private bool b_readyToLoadScenes = false;
	private bool b_LoadingScene = false;

	public delegate void OnRoomLoaded();
	public delegate void OnRoomExit();
	public delegate void OnLevelTransitionStart();

	public OnRoomLoaded onRoomLoaded;
	public OnRoomExit onRoomExit;
	public OnLevelTransitionStart onLevelTransitionStart;
	private List<RPGEvent> RPGEvents = new List<RPGEvent>();

	public bool readyToLoadScenes
	{
		get{ return b_readyToLoadScenes;}
	}

	public bool loadingScene
	{
		get{ return b_LoadingScene;}
	}

	private IEnumerator Start ()
	{
		if (SceneManager.GetActiveScene ().name != persistentSceneName) 
		{
			SceneManager.LoadScene(persistentSceneName);
			yield return null;
		}
		b_readyToLoadScenes = true;
	}

	private IEnumerator InitializeWithPersistentScene(string sceneName)
	{
		SceneManager.LoadScene (persistentSceneName);
		yield return StartCoroutine (LoadSceneAndSetActive (sceneName));
		yield return null;
	}

	public IEnumerator LoadSceneAndSetActive(string sceneName, RoomLoadType roomLoadType = RoomLoadType.Normal)
	{
		GameManager.instance.audioListener.enabled = true;
		yield return SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
		Scene newlyLoadedScene = SceneManager.GetSceneAt (SceneManager.sceneCount - 1);
		GameManager.instance.audioListener.enabled = false;
		SceneManager.SetActiveScene (newlyLoadedScene);
		if (roomLoadType == RoomLoadType.Battle)
			BattleManager.instance.SetupBattleScene ();
		Debug.Log ("New Room Loaded: " + newlyLoadedScene.name);

		LoadingModal.instance.FadeIn ();
		while (!LoadingModal.instance.transitionIsReady) 
		{
			yield return new WaitForEndOfFrame();
		}
	}

	public IEnumerator ChangeSceneCo(string sceneName, RoomLoadType roomLoadType = RoomLoadType.Normal)
	{
		b_LoadingScene = true;
		GameManager.instance.SwitchState (new LoadingState ());
		if (roomLoadType == RoomLoadType.Battle)
			MusicManager.instance.StopMusicIntantly ();
		LoadingModal.instance.FadeOut (roomLoadType);
		if (onLevelTransitionStart != null)
			onLevelTransitionStart ();
		while (!LoadingModal.instance.transitionIsReady) 
		{
			yield return new WaitForEndOfFrame();
		}
		if (onRoomExit != null)
			onRoomExit ();
		while (RPGEvents.Count > 0)
		{
			for (int i = 0; i < RPGEvents.Count; i++)
			{
				if (!RPGEvents [i].eventActive) 
				{
					RPGEvents.Remove (RPGEvents [i]);
					RPGEvents.TrimExcess ();
				}
			}
			yield return new WaitForEndOfFrame();
		}
		onRoomExit = null;
		yield return SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ().buildIndex);
		yield return StartCoroutine (LoadSceneAndSetActive (sceneName, roomLoadType));
		if (onRoomLoaded != null)
			onRoomLoaded ();
		b_LoadingScene = false;
		yield return null;
	}

	public void ChangeScene(string sceneName, RoomLoadType roomLoadType = RoomLoadType.Normal)
	{
		StartCoroutine (ChangeSceneCo (sceneName, roomLoadType));
	}

	public void AddRPGEventToOnRoomExit(RPGEvent rpgEvent)
	{
		onRoomExit += rpgEvent.PlayRPGEvent;
		RPGEvents.Add (rpgEvent);
	}
}
	
public enum RoomLoadType
{
	Normal,
	Battle,
}