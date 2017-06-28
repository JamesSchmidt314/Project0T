using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class OverworldPlayerCharacter : MonoBehaviour 
{
	public float speed = 10.0f;
	public float movingTurnSpeed = 360.0f;
	public float gravity = 20.0f;

	private static OverworldPlayerCharacter m_playerCharacter;

	public static OverworldPlayerCharacter playerCharacter
	{
		get { return m_playerCharacter;}
	}

	public static List<OverworldPlayerCharacter> m_currentCharacters = new List<OverworldPlayerCharacter>();

	public static List<OverworldPlayerCharacter> currentCharacters
	{
		get{ return m_currentCharacters;}
	}

	public bool isPlayer
	{
		get{ return (m_playerCharacter == this);}
	}

	private CharacterController m_CharacterController;

	//optional AI Control
	private NavMeshAgent m_Agent;

//	private static float groundCheckDistance = 0.3f;
	//private float stationaryTurnSpeed = 180.0f;
	private float turnAmount;
	private float forwardAmount;
	private Vector3 m_InputVelocity;

	private const float AI_INPUT_LERP_SPEED = 30.0f;

	void Awake()
	{
		m_CharacterController = GetComponent<CharacterController> ();
		m_Agent = GetComponent<NavMeshAgent> ();
		m_Agent.updateRotation = false;
		m_Agent.updatePosition = false;

		m_currentCharacters.Add (this);
	}

	void OnDestroy()
	{
		m_currentCharacters.Remove (this);
	}

	void FixedUpdate()
	{
		if (m_playerCharacter != this)
			m_Agent.nextPosition = transform.position;
		Move (m_InputVelocity);
	}

	public void Move(Vector3 move)
	{
		if (move.magnitude > 1f)
			move.Normalize ();
		move = transform.InverseTransformDirection (move);
		move *= speed;
		turnAmount = Mathf.Atan2 (move.x, move.z);
		forwardAmount = move.z;

		//float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
		transform.Rotate (0, turnAmount * movingTurnSpeed * Time.deltaTime, 0);

		move.y -= gravity * Time.deltaTime;
		m_CharacterController.Move (new Vector3(transform.forward.x * forwardAmount, move.y, transform.forward.z * forwardAmount) * Time.deltaTime);
	}

	public void StartAsPlayer()
	{
		m_playerCharacter = this;
		CameraController.instance.SetInstantTarget (m_playerCharacter);
	}

	public void SetAsPlayer()
	{
		m_playerCharacter = this;
		CameraController.instance.SetTarget (m_playerCharacter);
	}

	public void RelayInput(Vector3 input)
	{
		m_InputVelocity = input;
	}

	public void FollowPlayer()
	{
		if (playerCharacter != null && playerCharacter != this)
			m_Agent.SetDestination (playerCharacter.transform.position);

		m_InputVelocity = Vector3.Lerp (m_Agent.velocity, m_Agent.desiredVelocity, AI_INPUT_LERP_SPEED * Time.deltaTime);
	}

	public Vector3 GetMiddlePosition()
	{
		return transform.position + m_CharacterController.center;
	}

	public void StopCharacter()
	{
		m_InputVelocity = Vector3.zero;
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.collider.GetComponent<CharacterController> () && !hit.collider.GetComponent<OverworldPlayerCharacter>().isPlayer) 
		{
			hit.collider.GetComponent<CharacterController>().Move ((hit.collider.transform.position - transform.position) * Time.deltaTime * 5);
		}
	}
}
