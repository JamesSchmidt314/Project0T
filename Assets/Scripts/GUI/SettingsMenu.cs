using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour 
{
	public Toggle m_AAToggle;
	public Toggle m_BloomToggle;
	public Toggle m_DOFToggle;
	public Slider m_MusicSlider;

	public void InitializeSettings()
	{
		m_AAToggle.isOn = PlayerDataManager.instance.saveFile.settingsData.antiAliasing;
		m_BloomToggle.isOn = PlayerDataManager.instance.saveFile.settingsData.bloom;
		m_DOFToggle.isOn = PlayerDataManager.instance.saveFile.settingsData.depthOfField;
		m_MusicSlider.value = PlayerDataManager.instance.saveFile.settingsData.musicVolume;
		Debug.Log ("settings initialized");
	}
}