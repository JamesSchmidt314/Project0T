using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldRoomData : Singleton<OverworldRoomData> 
{
	[SerializeField] private string m_RoomName = "_RoomName";
	[SerializeField] private string m_RoomBattleScene = "B_Debug01";
	[SerializeField] private AudioClip m_DayMusicClip;
	[SerializeField] private AudioClip m_NightMusicClip;

	public string roomName
	{
		get{ return m_RoomName;}
	}

	public string roomBattleScene
	{
		get{ return m_RoomBattleScene;}
	}

	public AudioClip dayMusicClip
	{
		get{ return m_DayMusicClip;}
	}

	public AudioClip nightMusicClip
	{
		get{ return m_NightMusicClip;}
	}
}