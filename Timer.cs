using UnityEngine;
using UnityEngine.Events;



namespace Common
{
	/// <summary>
	/// Timer.
	/// </summary>
	public class Timer
	{
		private const float TIMER_NOT_ACTIVE = -10000f;



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
					mDuration = value;
				}
				else
				{
					Debug.LogError("Invalid duration value: " + duration);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Common.Timer"/> is active.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public bool active
		{
			get
			{
				return mTime != TIMER_NOT_ACTIVE;
			}

			set
			{
				if (value)
				{
					if (mTime == TIMER_NOT_ACTIVE)
					{
						Start();
					}
				}
				else
				{
					if (mTime != TIMER_NOT_ACTIVE)
					{
						Stop();
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Common.Timer"/> is about to shot.
		/// </summary>
		/// <value><c>true</c> if timer is about to shot; otherwise, <c>false</c>.</value>
		public bool isAboutToShot
		{
			get { return mTime >= mDuration; }
		}



		private float       mDuration;
		private float       mTime;
		private UnityAction mOnTimeout;



		/// <summary>
		/// Initializes a new instance of the <see cref="Common.Timer"/> class.
		/// </summary>
		/// <param name="onTimeout">On timeout action.</param>
		/// <param name="duration">Duration.</param>
		public Timer(UnityAction onTimeout, float duration = 0f)
		{
			mDuration  = duration;
			mTime      = TIMER_NOT_ACTIVE;
			mOnTimeout = onTimeout;
		}

		/// <summary>
		/// Update should be called once per frame.
		/// </summary>
        public void Update()
		{
			if (mTime != TIMER_NOT_ACTIVE)
			{
				mTime += Time.deltaTime;

				if (mTime >= mDuration)
				{
					mOnTimeout.Invoke();
				}
			}
		}

		/// <summary>
		/// Starts timer.
		/// </summary>
		public void Start()
		{
			mTime = 0f;
		}

		/// <summary>
		/// Starts timer with specified duration.
		/// </summary>
		/// <param name="duration">Duration.</param>
		public void Start(float duration)
		{
			if (duration >= 0f)
			{
				mDuration = duration;
				mTime     = 0f;
			}
			else
			{
				Debug.LogError("Invalid duration value: " + duration);
			}
		}

		/// <summary>
		/// Stops timer.
		/// </summary>
		public void Stop()
		{
			mTime = TIMER_NOT_ACTIVE;
		}
	}
}
