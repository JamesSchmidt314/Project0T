using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour 
{
	//43200 = 12:00 pm
	//86400 = 12:00 am (24 hours)
	public bool overrideMainTime = false;
	[Range(0,24)]public int startingHour = 12;
	public float nightAmbientIntensity = 0.2f;
	public double timeDouble;
	public string actualTime;
	public string time;
	private Light m_Light;

	float defaultSunIntensity;
	float defaultAmbientIntensity;
	Color defaultFogColor;


	void Start()
	{
		m_Light = GetComponent<Light> ();
		defaultSunIntensity = m_Light.intensity;
		defaultAmbientIntensity = RenderSettings.ambientIntensity;
		defaultFogColor = RenderSettings.fogColor;
		transform.rotation = Quaternion.identity;

		if (overrideMainTime)
			TimeManager.SetTime (startingHour);

		transform.eulerAngles = new Vector3(TimeManager.GetSunAngle(),0f,0f);
	}

	void FixedUpdate () 
	{
		time = TimeManager.GetCurrentTimeFormatted ();
		timeDouble += Time.deltaTime;

		System.TimeSpan t = System.TimeSpan.FromSeconds (timeDouble);
		actualTime = string.Format ("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

		if (transform.eulerAngles.x <= 90) 
		{
			m_Light.intensity = (transform.eulerAngles.x / 90f) * defaultSunIntensity;
			RenderSettings.ambientIntensity = Mathf.Clamp ((transform.eulerAngles.x) / 90f, nightAmbientIntensity, defaultAmbientIntensity);
			RenderSettings.fogColor = defaultFogColor * RenderSettings.ambientIntensity;
		}
		else 
		{
			m_Light.intensity = 0;
			RenderSettings.ambientIntensity = nightAmbientIntensity;
			RenderSettings.fogColor = defaultFogColor * nightAmbientIntensity;
		}
		if (TimeManager.instance != null)
			transform.RotateAround (Vector3.zero, Vector3.right, TimeManager.instance.adjustedScale * Time.deltaTime);
		else Debug.Log("Time Manager not found. leaving sun at static position.");
		transform.LookAt (Vector3.zero);
	}
}
