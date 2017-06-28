using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class LoadingModal : MonoBehaviour
{
	private static LoadingModal s_Instance;

	public float fadeSpeed = 3.0f;
	private Image m_Image;
	private CanvasGroup m_CanvasGroup;
	private Image waitImage;

	private Image heartImage;
	private Image heartMoveToImage;

	private FadeMode m_FadeMode = FadeMode.Waiting;
	private bool b_TransitionIsReady = false;

	public static LoadingModal instance
	{
		get{ return s_Instance;}
	}

	public bool transitionIsReady
	{
		get{ return b_TransitionIsReady; }
	}

	void Awake()
	{
		if (s_Instance != null)
			Destroy (this.gameObject);
		else
			s_Instance = this;

		m_Image = GetComponent<Image> ();
		m_Image.color = Color.black;
		m_CanvasGroup = GetComponent<CanvasGroup> ();

		waitImage = transform.Find ("Wait Image").GetComponent<Image> ();

		heartImage = transform.Find ("Heart").GetComponent<Image> ();
		heartImage.enabled = false;
		heartMoveToImage = transform.Find ("HeartMoveToPosition").GetComponent<Image> ();
		heartMoveToImage.color = Color.clear;
	}

	void Update()
	{
		switch (m_FadeMode)
		{
		case FadeMode.In:
			m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, 0, Time.deltaTime * fadeSpeed);
			if (m_CanvasGroup.alpha == 0)
			{
				m_CanvasGroup.blocksRaycasts = false;
				b_TransitionIsReady = true;
				m_FadeMode = FadeMode.Off;
			}
			break;
		case FadeMode.Out:
			m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, 1, Time.deltaTime * fadeSpeed);
			if (m_CanvasGroup.alpha == 1)
			{
				b_TransitionIsReady = true;
				waitImage.enabled = true;
				m_FadeMode = FadeMode.Waiting;
			}
			break;
		default:
			break;
		}

	}

	//fade to clear
	public void FadeIn()
	{
		waitImage.enabled = false;
		b_TransitionIsReady = false;
		m_FadeMode = FadeMode.In;
	}

	//fade to black
	public void FadeOut(RoomLoadType roomLoadType = RoomLoadType.Normal)
	{
		m_CanvasGroup.blocksRaycasts = true;
		b_TransitionIsReady = false;
		switch (roomLoadType)
		{
		case RoomLoadType.Normal:
			m_FadeMode = FadeMode.Out;
			break;
		case RoomLoadType.Battle:
			m_FadeMode = FadeMode.Waiting;
			PrimeBattleTransition ();
			break;
		}
	}

	public void PrimeBattleTransition()
	{
		m_FadeMode = FadeMode.Waiting;
		m_CanvasGroup.alpha = 1;
		StartCoroutine (BattleTransitionAnimation ());
	}
	private float scaler = 6;

	private IEnumerator BattleTransitionAnimation()
	{
		Vector2 viewportPoint = Camera.main.WorldToViewportPoint (OverworldPlayerCharacter.playerCharacter.GetMiddlePosition());
		heartImage.rectTransform.anchorMin = viewportPoint;
		heartImage.rectTransform.anchorMax = viewportPoint;
		heartImage.enabled = true;
		yield return new WaitForSeconds(0.3f);
		heartImage.enabled = false;
		yield return new WaitForSeconds(0.3f);
		heartImage.enabled = true;
		yield return new WaitForSeconds(0.3f);
		heartImage.enabled = false;
		yield return new WaitForSeconds(0.3f);
		heartImage.enabled = true;
		while((heartImage.rectTransform.anchorMax != heartMoveToImage.rectTransform.anchorMax) && (heartImage.rectTransform.anchorMin != heartMoveToImage.rectTransform.anchorMin))
		{
			heartImage.rectTransform.anchorMax = Vector2.MoveTowards (heartImage.rectTransform.anchorMax, heartMoveToImage.rectTransform.anchorMax, Time.deltaTime * 0.2f * scaler); 
			heartImage.rectTransform.anchorMin = Vector2.MoveTowards (heartImage.rectTransform.anchorMin, heartMoveToImage.rectTransform.anchorMin, Time.deltaTime * 0.2f * scaler); 
			yield return new WaitForEndOfFrame ();
		}
		heartImage.enabled = false;
		b_TransitionIsReady = true;
		yield return null;
	}
}

public enum FadeMode
{
	In,
	Out,
	Waiting,
	Off,
}