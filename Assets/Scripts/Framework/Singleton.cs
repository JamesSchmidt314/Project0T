using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T s_Instance;

	public static T instance
	{
		get
		{
			return s_Instance;
		}
		protected set
		{
			s_Instance = value;
		}
	}

	protected virtual void Awake()
	{
		if (s_Instance != null)
			Destroy (this.gameObject);
		else
			s_Instance = (T)this;
	}

	protected virtual void OnDestroy()
	{
		if (s_Instance == this)
			s_Instance = null;
	}

}