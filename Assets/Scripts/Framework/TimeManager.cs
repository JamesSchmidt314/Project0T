using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : PersistentSingleton<TimeManager> 
{
	public float cycleScale = 1.0f;

	private static double m_CurrentTime;
	private static double m_CurrentDayTime;

	public static double currentDayTime
	{
		get{ return m_CurrentDayTime;}
	}

	public bool isDay
	{
		get{ return (currentDayTime < 43200);}
	}

	public static double currentTime
	{
		get{ return m_CurrentTime;}
	}

	public float adjustedScale
	{
		get { return (cycleScale * CORE_SCALE);}
	}

	public delegate void OnTimeChanged(bool isDay);
	public OnTimeChanged onTimeChanged;

	private const float CORE_SCALE = 0.225f;
	private const double TIME_CYCLE_SCALE = 240.0d;
	private const double HOUR_TO_MILLI = 3600;

	bool hasChangedMid;

	void Start()
	{
		onTimeChanged += TimeChanged;
	}

	void TimeChanged(bool day)
	{
		Debug.Log (day ? "is day" : "is night");
	}

	void FixedUpdate()
	{
		if (GameManager.gameState != GameState.Loading && GameManager.gameState != GameState.MainMenu && GameManager.gameState != GameState.Menu)
		{
			KeepTime ();
		}
	}

	void KeepTime()
	{
		m_CurrentTime += Time.fixedDeltaTime * TIME_CYCLE_SCALE * adjustedScale;
		m_CurrentDayTime += Time.fixedDeltaTime * TIME_CYCLE_SCALE * adjustedScale;

		if (m_CurrentDayTime > 86400) 
		{
			//one full day has passed
			onTimeChanged (true);
			m_CurrentDayTime = 0;	
			hasChangedMid = false;
		}

		if (m_CurrentDayTime > 43200)
		{
			//night has come
			if (!hasChangedMid)
				onTimeChanged (false);
			hasChangedMid = true;
		}
	}

	public static void SetTime(int hour)
	{
		if (hour < 6)
		{
			hour = 24 - hour;
		}
		else hour -= 6;

		m_CurrentDayTime = (double)(hour * HOUR_TO_MILLI);
		m_CurrentTime = (double)(hour * HOUR_TO_MILLI);
	}

	public static float GetSunAngle()
	{
		return (float)(TimeManager.currentTime / TIME_CYCLE_SCALE);
	}

	public static string GetCurrentTimeFormatted()
	{
		System.TimeSpan t = System.TimeSpan.FromSeconds (m_CurrentTime + 21600);
		return string.Format ("{0:D2}:{1:D2}", t.Hours, t.Minutes);
	}
}
