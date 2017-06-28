using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GUIFormatter : Singleton<GUIFormatter> 
{
	private Canvas m_GameCanvas;
	public GameObject mainMenu;
	public SettingsMenu settingsMenu;
	public DialogueBox dialogueBox;
	public GameObject pausedMenu;

	public AudioClip dialogueSoundBite;

	private static float defaultTextTypingSpeed = 0.08f;

	private bool m_IsTyping;

	public bool isTyping
	{
		get{ return m_IsTyping;}
	}

	public static Canvas gameCanvas
	{
		get{ return instance.m_GameCanvas;}
	}

	bool pausePerChar = true;

	void Start()
	{
		m_GameCanvas = GetComponent<Canvas> ();

		dialogueBox.gameObject.SetActive (false);

		pausedMenu.SetActive (false);
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		mainMenu.SetActive (scene.name == "Scene_Menu01");
	}

	void EnableMenu()
	{
		mainMenu.SetActive (true);
	}

	void DisableMenu()
	{
		mainMenu.SetActive (false);
	}

	public void StartGame()
	{
		RoomManager.instance.ChangeScene ("Debug01");
	}

	public void PrimeFlavor(DialogueFlavor flavor)
	{
		dialogueBox.nameText.text = LocalizationManager.instance.GetLocalizedValue (flavor.speakerNameKey);
		dialogueBox.dialogueText.font = flavor.font;
	}

	public IEnumerator AutoTypeField(string field, Text textObject, Animator anim = null)
	{
		int colorIndex = 0;
		bool colored = false;
		if (!pausePerChar)
			pausePerChar = true;
		textObject.text = string.Empty;
		m_IsTyping = true;
		if (anim)
			anim.SetBool ("Talk", true);
		for (int i = 0; i < field.ToCharArray().Length; i++)
		{
			if (field.ToCharArray () [i] == '|') 
			{
				i++;
				if (pausePerChar)
					yield return new WaitForSeconds (defaultTextTypingSpeed * 2f * float.Parse (field.ToCharArray () [i].ToString ()));
			}
			else if (field.ToCharArray () [i] == '@')
			{
				if (colored)
					colored = false;
				else 
				{
					colored = true;
					i++;
					colorIndex = int.Parse (field.ToCharArray () [i].ToString ());
				}
			}
			else
			{
				string letter = field.ToCharArray () [i].ToString ();
				if (colored)
					letter = "<color=" + GUIFormatter.IntToStringColorArray (colorIndex) + ">" + letter + "</color>";
				textObject.text += letter;
				if (pausePerChar)
					yield return new WaitForSeconds (defaultTextTypingSpeed);
			}
		}
		m_IsTyping = false;
		if (anim)
			anim.SetBool ("Talk", false);
		yield return null;
	}

	public IEnumerator SetupChoices(string[] keys, int[] ids)
	{
		Button[] bArray = new Button[keys.Length];
		for (int i = 0; i < ids.Length; i++) 
		{
			int newID = ids [i];
			Button button = Instantiate (dialogueBox.buttonTemplate, dialogueBox.choiceRange.position, Quaternion.identity) as Button;
			bArray [i] = button;
			bArray [i].interactable = false;
			button.GetComponentInChildren<Text> ().text = string.Empty;
			//button.GetComponentInChildren<Text> ().text = LocalizationManager.instance.GetLocalizedValue (keys [i]);
			button.transform.SetParent (dialogueBox.choiceRange, false);
			button.onClick.AddListener(delegate {
				ChangeNextCommand(newID);
			});
		}
		for (int i = 0; i < bArray.Length; i++) 
		{
			yield return StartCoroutine (AutoTypeField (LocalizationManager.instance.GetLocalizedValue (keys [i]), bArray [i].GetComponentInChildren<Text> ()));
		}
		for (int i = 0; i < bArray.Length; i++) 
		{
			bArray [i].interactable = true;
		}
		yield return null;
	}

	public void SkipDialogue()
	{
		pausePerChar = false;
	}

	public void CreateChoices(string[] keys, int[] ids)
	{
		for (int i = 0; i < ids.Length; i++) 
		{
			int newID = ids [i];
			Button button = Instantiate (dialogueBox.buttonTemplate, dialogueBox.choiceRange.position, Quaternion.identity) as Button;
			button.GetComponentInChildren<Text> ().text = LocalizationManager.instance.GetLocalizedValue (keys [i]);
			button.transform.SetParent (dialogueBox.choiceRange, false);
			button.onClick.AddListener(delegate {
				ChangeNextCommand(newID);
			});
		}
	}

	public void ChangeNextCommand(int newID)
	{
		InteractiveState.currentNextCommandID = newID;
		InteractiveState.proceed = true;
	}

	public static string IntToStringColorArray(int index = 0)
	{
		switch (index)
		{
		case 0:
			return "black";
		case 1:
			return "blue";
		case 2:
			return "cyan";
		case 3:
			return "gray";
		case 4:
			return "green";
		case 5:
			return "magneta";
		case 6:
			return "red";
		case 7:
			return "white";
		case 8:
			return "yellow";
		case 9:
			return "orange";
		default:
			return "black";
		}
	}
}
