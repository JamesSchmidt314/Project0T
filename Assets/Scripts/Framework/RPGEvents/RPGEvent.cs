using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGEvent : MonoBehaviour
{
	public bool isNPC;
	public RPGTrigger trigger;
	public RPGEventPage eventPage;
	private int eventIndex = 0;
	private float waitTime = 0.0f;

	private bool interactive = false;
	public bool eventActive = false;

	//if interactive
	public float triggerableDistance = 4.0f;
	private SphereCollider m_SphereCollider;
	private Transform IconPositioning;
	private GameObject icon;
	private GameObject m_InAnimator;
	private bool m_Triggered = false;

	//if NPC:
	public Animator m_NPCAnimator;
	public float timeBeforeIdleReturn = 10.0f;

	private const float TIME_TILL_SPECIAL_IDLE = 5;
	private const float LOOK_SPEED = 3;
	private const float TURN_SPEED = 5;
	private float m_LookLayerWeight = 1;

	private float m_Turn = 0.0f;
	private float m_LookX = 0.0f;
	private float m_LookY = 0.0f;

	private float timeSinceStill;
	private bool specialIdling = false;
	private float timeSinceSpecialIdle;

	public IEnumerator Start()
	{
		while (!GameManager.fullyInitialized) 
		{
			yield return new WaitForEndOfFrame ();
		}


		switch (trigger) 
		{
		case RPGTrigger.OnStart:
			PlayRPGEvent ();
			break;
		case RPGTrigger.AfterLevelTransition:
			RoomManager.instance.onRoomLoaded += PlayRPGEvent;
			break;
		case RPGTrigger.OnInteract:
			m_SphereCollider = GetComponent<SphereCollider> ();
			IconPositioning = transform.Find ("IconPositioning");
			icon = Resources.Load ("Debug_Interactable01") as GameObject;
			m_InAnimator = Instantiate (icon, IconPositioning.position, Quaternion.identity) as GameObject;
			m_InAnimator.transform.SetParent (this.transform);
			break;
		case RPGTrigger.OnLevelTransitionStart:
			RoomManager.instance.onLevelTransitionStart += PlayRPGEvent;
			break;
		case RPGTrigger.OnRoomExit:
			RoomManager.instance.AddRPGEventToOnRoomExit (this);
			break;
		}
		yield return null;
	}

	void OnEnable()
	{
		if (trigger == RPGTrigger.OnEnable) 
		{
			PlayRPGEvent ();
		}
	}

	void OnDestroy()
	{
		if (trigger == RPGTrigger.AfterLevelTransition)
		{
			RoomManager.instance.onRoomLoaded -= PlayRPGEvent;
		}
		if (trigger == RPGTrigger.OnLevelTransitionStart)
		{
			RoomManager.instance.onLevelTransitionStart -= PlayRPGEvent;
		}
	}

	void Update()
	{
		if (isNPC) 
		{
			if (Mathf.Approximately (m_NPCAnimator.GetFloat ("Turn"), 0)) 
			{
				if (!specialIdling) 
				{
					timeSinceStill += Time.deltaTime;
					if (timeSinceStill >= TIME_TILL_SPECIAL_IDLE) 
					{
						timeSinceStill = 0;
						m_LookLayerWeight = 0;
						m_NPCAnimator.SetTrigger ("SpecialIdle");
						timeSinceSpecialIdle = Time.time;
						specialIdling = true;
					}
				}
			} 

			if (specialIdling)
			{
				WaitToReturnToIdle ();
			}
		}

		if (interactive && !m_Triggered)
		{
			OverworldState.currentRPGEvent = this;
			OverworldState.inInteractivePosition = true;
		}
			
		if (trigger == RPGTrigger.OnInteract && m_InAnimator)
			m_InAnimator.GetComponent<Animator> ().SetBool ("Active", interactive && !m_Triggered);
	}

	void FixedUpdate()
	{
		if (isNPC)
		{
			m_NPCAnimator.SetLayerWeight (1, Mathf.MoveTowards (m_NPCAnimator.GetLayerWeight (1), m_LookLayerWeight, Time.deltaTime));
			m_NPCAnimator.SetFloat ("Turn", Mathf.MoveTowards (m_NPCAnimator.GetFloat("Turn"), m_Turn, Time.deltaTime * TURN_SPEED));
			m_NPCAnimator.SetFloat ("LookX", Mathf.MoveTowards (m_NPCAnimator.GetFloat("LookX"), m_LookX, Time.deltaTime * LOOK_SPEED));
			m_NPCAnimator.SetFloat ("LookY", Mathf.MoveTowards (m_NPCAnimator.GetFloat("LookY"), m_LookY, Time.deltaTime * LOOK_SPEED));
		}
	}

	public void WaitToReturnToIdle()
	{
		if (Time.time > (timeSinceSpecialIdle + timeBeforeIdleReturn))
		{
			m_LookLayerWeight = 1;
			specialIdling = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		switch (trigger)
		{
		case RPGTrigger.OnEventTrigger:
			PlayRPGEvent ();
			break;
		case RPGTrigger.OnPlayerTrigger:
			if (other.GetComponent<OverworldPlayerCharacter>() && other.GetComponent<OverworldPlayerCharacter>().isPlayer)
				PlayRPGEvent ();
			break;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (trigger == RPGTrigger.OnInteract) 
		{
			if (other.CompareTag ("Player"))
			{
				OverworldPlayerCharacter player = other.GetComponent<OverworldPlayerCharacter> ();
				if (player.isPlayer)
				{
					Vector3 direction = (other.transform.position - transform.position).normalized;
					RaycastHit hit;
					if (Physics.Raycast (transform.position + m_SphereCollider.center, direction.normalized, out hit, m_SphereCollider.radius)) 
					{
						if (hit.collider == other)
						{
							if (hit.distance <= triggerableDistance)
								interactive = true;
							else
							{
								interactive = false;
								if (OverworldState.currentRPGEvent == this) 
								{
									OverworldState.inInteractivePosition = false;
									OverworldState.currentRPGEvent = null;
								}
							}

							if (isNPC)
							{
								direction = m_NPCAnimator.transform.InverseTransformDirection (direction).normalized;
								bool isInRange = (direction.x > 0 && direction.x < 0.01f) || (direction.x < 0 && direction.x > -0.01f);
								m_Turn = !specialIdling ? (isInRange ? 0 : Mathf.Atan2 (direction.x, direction.z)) : 0;
								m_LookX = Mathf.Atan2 (direction.x, direction.z);
								m_LookY = -((transform.position + m_SphereCollider.center) - (player.GetMiddlePosition ())).y;
							}
						} 
						else 
						{
							interactive = false;
							if (isNPC)
								m_Turn = 0;
						}
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player")) 
		{
			OverworldPlayerCharacter player = other.GetComponent<OverworldPlayerCharacter> ();
			if (player.isPlayer) 
			{
				switch (trigger) 
				{
				case RPGTrigger.OnInteract:
					OverworldState.inInteractivePosition = false;
					interactive = false;
					if (isNPC)
					{
						m_Turn = 0;
					}
					break;
				}
			}
		}
	}

	private IEnumerator RunRPGEventCo()
	{
		if (trigger == RPGTrigger.OnInteract)
			m_Triggered = true;
		eventActive = true;
		while (eventIndex < eventPage.commands.Count && eventIndex >= 0 && (eventIndex != eventPage.commands[eventIndex].nextCommand))
			yield return StartCoroutine (ProcessRPGEventCommand (eventIndex));
		if (eventPage.reset) 
			ResetVariables ();
		eventActive = false;
		if (GameManager.instance.m_gameState == GameState.Interaction) 
		{
			GameManager.instance.SwitchState (InteractiveState.lastKnownState);
		}
		yield return null;
	}

	private IEnumerator ProcessRPGEventCommand(int ind = 0)
	{
		switch (eventPage.commands [ind].eventType)
		{
		case RPGEventType.Generic:
			eventPage.commands [ind].command.Invoke ();
			yield return new WaitForSeconds (waitTime);
			waitTime = 0;
			break;
		case RPGEventType.keyInput:
			if (!PlayerDataManager.instance.saveFile.gamePersistenceValues.ContainsKey (eventPage.commands [ind].keyInput.key)) 
			{
				PlayerDataManager.instance.saveFile.gamePersistenceValues.Add (eventPage.commands [ind].keyInput.key, eventPage.commands [ind].keyInput.value);
			}
			switch (eventPage.commands [ind].keyInput.keyInputType) 
			{
			case KeyInputType.Decrement:
				PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].keyInput.key]--;
				break;
			case KeyInputType.Increment:
				PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].keyInput.key]++;
				break;
			case KeyInputType.SolidValue:
				PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].keyInput.key] = eventPage.commands [ind].keyInput.value;
				break;
			}
			break;
		case RPGEventType.BattleInitialize:
			InitializeBattle (eventPage.commands [ind].enemies);
			break;
		case RPGEventType.Dialogue:
			switch (eventPage.commands [ind].order.orderType)
			{
			case DialogueOrderType.Statement:
				if (GameManager.instance.m_gameState != GameState.Interaction) 
				{
					InteractiveState.lastKnownState = GameManager.instance.GetState (GameManager.instance.m_gameState);
					GameManager.instance.SwitchState (new InteractiveState ());
				}
				PrimeDialogueOrder (false);
				InteractiveState.SetUnskippable (eventPage.commands [ind].order.statement.unskippable);
				GUIFormatter.instance.PrimeFlavor (eventPage.commands [ind].order.statement.statementFlavor);
				yield return StartCoroutine (GUIFormatter.instance.AutoTypeField (LocalizationManager.instance.GetLocalizedValue (eventPage.commands [ind].order.statement.statementKey),
					GUIFormatter.instance.dialogueBox.dialogueText,
					eventPage.commands [ind].order.statement.currentlySpeakingActor));
				while (!InteractiveState.proceed) 
				{
					yield return new WaitForEndOfFrame ();
				}
				break;
			case DialogueOrderType.Divergence:
				int nextCommand = eventPage.commands [ind].order.divergence.defaultDivergenceID;
				for (int j = 0; j < eventPage.commands [ind].order.divergence.conditions.Count; j++) 
				{
					if (PlayerDataManager.instance.saveFile.gamePersistenceValues.ContainsKey (eventPage.commands [ind].order.divergence.conditions [j].keyName)) 
					{
						switch (eventPage.commands [ind].order.divergence.conditions [j].validType) 
						{
						case RPGCondition.ValidType.EqualTo:
							if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].order.divergence.conditions [j].keyName] == eventPage.commands [ind].order.divergence.conditions [j].value)
								nextCommand = eventPage.commands [ind].order.divergence.dialogueConditionIDs[j];
							break;
						case RPGCondition.ValidType.GreaterOrEqualTo:
							if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].order.divergence.conditions [j].keyName] >= eventPage.commands [ind].order.divergence.conditions [j].value)
								nextCommand = eventPage.commands [ind].order.divergence.dialogueConditionIDs[j];
							break;
						case RPGCondition.ValidType.LessOrEqualTo:
							if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].order.divergence.conditions [j].keyName] <= eventPage.commands [ind].order.divergence.conditions [j].value)
								nextCommand = eventPage.commands [ind].order.divergence.dialogueConditionIDs[j];
							break;
						case RPGCondition.ValidType.GreaterThan:
							if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].order.divergence.conditions [j].keyName] > eventPage.commands [ind].order.divergence.conditions [j].value)
								nextCommand = eventPage.commands [ind].order.divergence.dialogueConditionIDs[j];
							break;
						case RPGCondition.ValidType.LessThan:
							if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.commands [ind].order.divergence.conditions [j].keyName] < eventPage.commands [ind].order.divergence.conditions [j].value)
								nextCommand = eventPage.commands [ind].order.divergence.dialogueConditionIDs[j];
							break;
						}
					}
				}
				eventPage.commands [ind].nextCommand = nextCommand;
				break;
			case DialogueOrderType.Option:
				if (GameManager.instance.m_gameState != GameState.Interaction) 
				{
					InteractiveState.lastKnownState = GameManager.instance.GetState (GameManager.instance.m_gameState);
					GameManager.instance.SwitchState (new InteractiveState ());
				}
				PrimeDialogueOrder (true);
				GUIFormatter.instance.CreateChoices (eventPage.commands [ind].order.option.choiceKeys.ToArray (), eventPage.commands [ind].order.option.choiceIDs.ToArray ());
				//yield return StartCoroutine(GUIFormatter.instance.SetupChoices(eventPage.commands [ind].order.option.choiceKeys.ToArray (),eventPage.commands [ind].order.option.choiceIDs.ToArray ()));
				while (!InteractiveState.proceed) 
				{
					yield return new WaitForEndOfFrame ();
				}
				GUIFormatter.instance.dialogueBox.ClearChoices ();
				eventPage.commands [ind].nextCommand = InteractiveState.currentNextCommandID;
				break;
			}
			break;
		}
		eventIndex = eventPage.commands [ind].nextCommand;
		yield return null;
	}

	void PrimeDialogueOrder(bool choice)
	{
		GUIFormatter.instance.dialogueBox.gameObject.SetActive(true);
		GUIFormatter.instance.dialogueBox.ToggleDialogueBox (!choice);
		InteractiveState.isChoice = choice;
		InteractiveState.proceed = false;
	}

	void ResetVariables()
	{
		m_Triggered = false;
		eventIndex = 0;
	}

	public bool ProcessConditions()
	{
		int validConditions = 0;
		for (int i = 0; i < eventPage.conditions.Count; i++) 
		{
			if (PlayerDataManager.instance.saveFile.gamePersistenceValues.ContainsKey (eventPage.conditions [i].keyName))
			{
				switch (eventPage.conditions [i].validType) 
				{
				case RPGCondition.ValidType.EqualTo:
					if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.conditions [i].keyName] == eventPage.conditions [i].value)
						validConditions++;
						break;
				case RPGCondition.ValidType.GreaterOrEqualTo:
					if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.conditions [i].keyName] >= eventPage.conditions [i].value)
						validConditions++;
					break;
				case RPGCondition.ValidType.LessOrEqualTo:
					if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.conditions [i].keyName] <= eventPage.conditions [i].value)
						validConditions++;
					break;
				case RPGCondition.ValidType.GreaterThan:
					if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.conditions [i].keyName] > eventPage.conditions [i].value)
						validConditions++;
					break;
				case RPGCondition.ValidType.LessThan:
					if (PlayerDataManager.instance.saveFile.gamePersistenceValues [eventPage.conditions [i].keyName] < eventPage.conditions [i].value)
						validConditions++;
					break;
				}
			}
		}
		if (eventPage.commands != null && eventPage.commands.Count != 0)
			validConditions++;
		return (validConditions == eventPage.conditions.Count + 1);
	}

	public void PlayRPGEvent()
	{
		if (ProcessConditions () && !m_Triggered) 
		{
			if (trigger == RPGTrigger.OnInteract) 
			{
				if (GameManager.instance.m_gameState == GameState.Interaction) 
				{
					Debug.Log ("Already in an interactive state. nullifying event.");
					return;
				}
			}
			StartCoroutine (RunRPGEventCo ());
		}
	}


	//commands
	public void Wait(float time)
	{
		waitTime = time;
	}

	public void ChangeToOverworldState()
	{
		GameManager.instance.SwitchState (new OverworldState ());
	}

	public void ChangeToInteractiveState()
	{
		GameManager.instance.SwitchState (new InteractiveState ());
	}

	public void SpawnPlayerCamera()
	{
		Instantiate (Resources.Load ("Camera Rig"), transform.position, Quaternion.identity);
	}

	public void InstantiatePartyOverworldCharacters(Transform orientation)
	{
		PlayerDataManager.instance.InstantiatePartyOverworldCharacters (orientation);
	}

	public void InstantiateCharactersByLastPosition()
	{
		PlayerDataManager.instance.InstantiatePartyOverworldCharacters (null);
		Debug.Log ("last position");
	}

	public void AddNewPartyMember(PartyMemberObject partyMember)
	{
		PlayerDataManager.instance.saveFile.partyMembers.Add (new PartyMember(partyMember.overworldCharacterPath, partyMember.memberData));
	}

	private void InitializeBattle(Enemy[] battleEnemies)
	{
		BattleManager.instance.InitializeBattle (battleEnemies);
	}

	public void ChangeScene(string sceneName)
	{
		RoomManager.instance.ChangeScene (sceneName);
	}

	public void ToggleMainMenu(bool active)
	{
		GUIFormatter.instance.mainMenu.SetActive (active);
	}

	public void PlayMusicIntantly(AudioClip music)
	{
		MusicManager.instance.PlayMusicInstantly (music);
	}

	public void PlayMusicFadeIn(AudioClip music)
	{
		MusicManager.instance.PlayMusicFadeIn (music);
	}

	public void FadeMusicOut()
	{
		MusicManager.instance.FadeMusicOut();
	}

	public void ChangeMusicStateToOverworld()
	{
		MusicManager.instance.ChangeMusicType (MusicType.OverworldMain);
	}

	public void ChangeMusicStateToSimple()
	{
		MusicManager.instance.ChangeMusicType (MusicType.Simple);
	}

	public void SaveData()
	{
		PlayerDataManager.instance.SaveGame ();
	}
}
