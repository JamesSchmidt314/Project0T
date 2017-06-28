using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : PersistentSingleton<MusicManager> 
{
	public MusicType musicType = MusicType.Simple;
	public float fadeSpeed = 1.5f;
	private float fadeLevel = 1.0f;
	private FadeMode m_FadeMode;
	private AudioSource m_AudioSource;

	private float m_OriginalVolume;

	bool m_lastKnownDayStage;

	void Start()
	{
		m_AudioSource = GetComponent<AudioSource> ();
		m_OriginalVolume = PlayerDataManager.instance.saveFile.settingsData.musicVolume;
	}

	void Update()
	{
		if (m_FadeMode == FadeMode.In) 
		{
			fadeLevel = Mathf.MoveTowards (fadeLevel, 1, Time.deltaTime * fadeSpeed);
		}
		if (m_FadeMode == FadeMode.Out) 
		{
			fadeLevel = Mathf.MoveTowards (fadeLevel, 0, Time.deltaTime * fadeSpeed);
		}
		m_AudioSource.volume = m_OriginalVolume * fadeLevel;

		switch (musicType) 
		{
		case MusicType.None:
			FadeMusicOut ();
			break;
		case MusicType.OverworldMain:
			if (OverworldRoomData.instance) 
			{
				if (m_lastKnownDayStage != TimeManager.instance.isDay) 
				{
					ChangeTimedAmbientMusic (TimeManager.instance.isDay);
					m_lastKnownDayStage = TimeManager.instance.isDay;
				}
			}
			break;
		case MusicType.Simple:
			break;
		}
	}

	public void ChangeMusicVolume(float value)
	{
		m_OriginalVolume = value;
		Debug.Log ("Music Volume: " + m_OriginalVolume);
	}

	private void PlayMusic(AudioClip music)
	{
		m_AudioSource.clip = music;
		m_AudioSource.PlayDelayed (0);
	}

	public void PlayMusicInstantly(AudioClip music)
	{
		fadeLevel = 1;
		PlayMusic (music);
	}

	public void StopMusicIntantly()
	{
		m_AudioSource.Stop ();
	}

	public void PlayMusicFadeIn(AudioClip music)
	{
		PlayMusic (music);
		m_FadeMode = FadeMode.In;
	}

	public void FadeMusicOut()
	{
		m_FadeMode = FadeMode.Out;
	}

	public void ChangeTimedAmbientMusic(bool day)
	{
		StartCoroutine (ChangeAmbientMusic(day));
	}

	private IEnumerator ChangeAmbientMusic(bool day)
	{
		FadeMusicOut ();
		while (fadeLevel != 0)
		{
			yield return new WaitForEndOfFrame ();
		}
		PlayMusicFadeIn (day ? OverworldRoomData.instance.dayMusicClip : OverworldRoomData.instance.nightMusicClip);
		if (day)
			PlayMusicFadeIn (OverworldRoomData.instance.dayMusicClip);
		else PlayMusicFadeIn(OverworldRoomData.instance.nightMusicClip);
		yield return null;
	}

	public void ChangeMusicType(MusicType _MusicType)
	{
		musicType = _MusicType;
		switch (musicType)
		{
		case MusicType.OverworldMain:
			if (OverworldRoomData.instance)
				PlayMusicFadeIn (TimeManager.instance.isDay ? OverworldRoomData.instance.dayMusicClip : OverworldRoomData.instance.nightMusicClip);
			break;
		case MusicType.Simple:
			break;
		}
		Debug.Log("changed");
	}
}

public enum MusicType
{
	OverworldMain,
	OverworldSub,
	Battle,
	Simple,
	None,
}
