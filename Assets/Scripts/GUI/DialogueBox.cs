using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour 
{
	[SerializeField]protected Text m_DialogueText;
	[SerializeField]protected Image m_BoxImage;
	[SerializeField]protected Image m_NameBox;
	[SerializeField]protected Text m_NameText;
	[SerializeField]protected Button m_ButtonTemplate;
	[SerializeField]protected Transform m_ChoiceRange;

	public Button buttonTemplate
	{
		get{ return m_ButtonTemplate;}
	}

	public Transform choiceRange
	{
		get{ return m_ChoiceRange;}
	}

	public void OnEnable()
	{
		ToggleDialogueBox (true);
	}

	public void OnDisable()
	{
		ToggleDialogueBox (false);
	}

	public Text dialogueText
	{
		get{ return m_DialogueText;}
	}

	public Text nameText
	{
		get{ return m_NameText;}
	}

	public void ToggleDialogueBox(bool active)
	{
		m_DialogueText.text = string.Empty;
		m_NameText.text = string.Empty;
		m_NameBox.enabled = active;
		//m_BoxImage.enabled = active;
	}

	public void ClearChoices()
	{
		for (int i = 0; i < choiceRange.childCount; i++) 
		{
			Destroy (choiceRange.GetChild (i).gameObject);
		}
	}
}
