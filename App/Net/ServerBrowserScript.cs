using UnityEngine;



namespace Common.App.Net
{
	/// <summary>
	/// Server browser script.
	/// </summary>
	public class ServerBrowserScript : MonoBehaviour
	{
		private const float TIMER_NOT_ACTIVE         = -10000f;
		private const float DEFAULT_REQUEST_DURATION = 60000f / 1000f;
		private const float DEFAULT_POLL_DURATION    = 1000f  / 1000f;



		/// <summary>
		/// Gets or sets the duration of the request.
		/// </summary>
		/// <value>The duration of the request.</value>
		public float requestDuration
		{
			get
			{
				return mRequestDuration;
			}

			set
			{
				if (value >= 0f)
				{
					if (mRequestDuration != value)
					{
						mRequestDuration = value;
						
						if (mRequestDelay != TIMER_NOT_ACTIVE)
						{
							mRequestDelay = mRequestDuration;
						}
					}
				}
				else
				{
					Debug.LogError("Incorrect delay value: " + value);
				}
			}
		}



		private float      mRequestDuration;
		private float      mRequestDelay;
		private float      mPollDelay;
		private HostData[] mHosts;



		/// <summary>
		/// Script starting callback.
		/// </summary>
		void Start()
		{
			mRequestDuration = DEFAULT_REQUEST_DURATION;
			mRequestDelay    = 0f;
			mPollDelay       = TIMER_NOT_ACTIVE;
			mHosts           = null;
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		void Update()
		{
			if (mRequestDelay != TIMER_NOT_ACTIVE)
			{
				mRequestDelay -= Time.deltaTime;
				
				if (mRequestDelay <= 0f)
				{
					OnRequestTimeout();

					return;
				}
			}

			if (mPollDelay != TIMER_NOT_ACTIVE)
			{
				mPollDelay -= Time.deltaTime;
				
				if (mPollDelay <= 0f)
				{
					OnPollTimeout();

					return;
				}
			}
		}

		/// <summary>
		/// Handler for request timeout event.
		/// </summary>
		private void OnRequestTimeout()
		{
			Global.clientScript.RequestHostList();

			mRequestDelay = mRequestDuration;
			mPollDelay    = DEFAULT_POLL_DURATION;
		}

		/// <summary>
		/// Handler for poll timeout event.
		/// </summary>
		private void OnPollTimeout()
		{
			mHosts = Global.clientScript.PollHostList();


			mPollDelay = DEFAULT_POLL_DURATION; // TODO: Should be TIMER_NOT_ACTIVE and we need to handle each server
		}
	}
}
