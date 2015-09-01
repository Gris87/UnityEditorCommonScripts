using UnityEngine;



namespace Common.App.Net
{
	/// <summary>
	/// Server browser script.
	/// </summary>
	public class ServerBrowserScript : MonoBehaviour
	{
		private const float TIMER_NOT_ACTIVE = -10000f;
		private const float DEFAULT_DURATION = 1000f / 1000f;



		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public float duration
		{
			get
			{
				return mDuration;
			}

			set
			{
				if (value >= 0f)
				{
					if (mDuration != value)
					{
						mDuration = value;
						
						if (IsTimerActive())
						{
							StartTimer();
						}
					}
				}
				else
				{
					Debug.LogError("Incorrect delay value: " + value);
				}
			}
		}



		private float mDuration;
		private float mDelay;



		/// <summary>
		/// Script starting callback.
		/// </summary>
		void Start()
		{
			mDuration = DEFAULT_DURATION;
			mDelay    = 0f;

			StartTimer();
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		void Update()
		{
			if (IsTimerActive())
			{
				mDelay -= Time.deltaTime;
				
				if (mDelay <= 0f)
				{
					OnTimeout();
				}
			}
		}

		/// <summary>
		/// Handler for timeout event.
		/// </summary>
		private void OnTimeout()
		{
			HostData[] hosts = Global.clientScript.PollHostList();

			Debug.LogError(hosts.Length);

			StartTimer();
		}

		/// <summary>
		/// Starts timer.
		/// </summary>
		private void StartTimer()
		{
			mDelay = mDuration;
		}
		
		/// <summary>
		/// Stops timer.
		/// </summary>
		private void StopTimer()
		{
			mDelay = TIMER_NOT_ACTIVE;
		}
		
		/// <summary>
		/// Determines whether timer is active.
		/// </summary>
		/// <returns><c>true</c> if timer is active; otherwise, <c>false</c>.</returns>
		private bool IsTimerActive()
		{
			return mDelay != TIMER_NOT_ACTIVE;
		}
	}
}
