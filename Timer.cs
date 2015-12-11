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
                DebugEx.VeryVeryVerboseFormat("Timer.duration = {0}", mDuration);

                return mDuration;
            }

            set
            {
                DebugEx.VeryVerboseFormat("Timer.duration: {0} => {1}", mDuration, value);

                if (value >= 0f)
                {
                    mDuration = value;
                }
                else
                {
                    DebugEx.ErrorFormat("Invalid duration value: {0}", duration);
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
                bool res = (mTime != TIMER_NOT_ACTIVE);

                DebugEx.VeryVeryVerboseFormat("Timer.active = {0}", res);

                return res;
            }

            set
            {
                bool isActive = (mTime != TIMER_NOT_ACTIVE);

                DebugEx.VeryVerboseFormat("Timer.active: {0} => {1}", isActive, value);

                if (isActive != value)
                {
                    if (value)
                    {
                        Start();
                    }
                    else
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
            get
            {
                bool res = mTime >= mDuration;

                DebugEx.VeryVeryVerboseFormat("Timer.isAboutToShot = {0}", res);

                return res;
            }
        }



        private float       mDuration;
        private float       mTime;
        private UnityAction mOnTimeout;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.Timer"/> class.
        /// </summary>
        /// <param name="onTimeout">On timeout action.</param>
        /// <param name="duration">Duration.</param>
        public Timer(UnityAction onTimeout = null, float duration = 0f)
        {
            DebugEx.VerboseFormat("Created Timer(onTimeout = {0}, duration = {1}) object", onTimeout, duration);

            mDuration  = duration;
            mTime      = TIMER_NOT_ACTIVE;
            mOnTimeout = onTimeout;
        }

        /// <summary>
        /// Update should be called once per frame.
        /// </summary>
        public void Update()
        {
            DebugEx.VeryVeryVerbose("Timer.Update()");

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
            DebugEx.VeryVerbose("Timer.Start()");

            mTime = 0f;
        }

        /// <summary>
        /// Starts timer with specified timeout action.
        /// </summary>
        /// <param name="onTimeout">On timeout action.</param>
        public void Start(UnityAction onTimeout)
        {
            DebugEx.VeryVerboseFormat("Timer.Start(onTimeout = {0})", onTimeout);

            mOnTimeout = onTimeout;
            mTime      = 0f;
        }

        /// <summary>
        /// Starts timer with specified duration.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public void Start(float duration)
        {
            DebugEx.VeryVerboseFormat("Timer.Start(duration = {0})", duration);

            if (duration >= 0f)
            {
                mDuration = duration;
                mTime     = 0f;
            }
            else
            {
                DebugEx.ErrorFormat("Invalid duration value: {0}", duration);
            }
        }

        /// <summary>
        /// Starts timer with specified duration and timeout action.
        /// </summary>
        /// <param name="onTimeout">On timeout action.</param>
        /// <param name="duration">Duration.</param>
        public void Start(UnityAction onTimeout, float duration)
        {
            DebugEx.VeryVerboseFormat("Timer.Start(onTimeout = {0}, duration = {1})", onTimeout, duration);

            if (duration >= 0f)
            {
                mDuration  = duration;
                mOnTimeout = onTimeout;
                mTime      = 0f;
            }
            else
            {
                DebugEx.ErrorFormat("Invalid duration value: {0}", duration);
            }
        }

        /// <summary>
        /// Stops timer.
        /// </summary>
        public void Stop()
        {
            DebugEx.Verbose("Timer.Stop()");

            mTime = TIMER_NOT_ACTIVE;
        }
    }
}
