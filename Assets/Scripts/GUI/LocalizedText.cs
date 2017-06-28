using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour 
{
	public string key;
	private Text m_Text;

	void Awake()
	{
		m_Text = GetComponent<Text> ();
	}

	private IEnumerator Start () 
	{
		while (!LocalizationManager.instance.localizationReady) 
		{
			yield return null;
		}
		SetLocalizedValue ();
	}

	public void SetLocalizedValue()
	{
		m_Text.text = LocalizationManager.instance.GetLocalizedValue (key);
	}
}