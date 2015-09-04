using System.Collections.Generic;
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



        private float           mRequestDuration;
        private float           mRequestDelay;
        private float           mPollDelay;
		private HashSet<string> mAskedHosts;
        private HostData[]      mHosts;
		private int             mCurrentHost;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
            mRequestDuration = DEFAULT_REQUEST_DURATION;
            mRequestDelay    = 0f;
            mPollDelay       = TIMER_NOT_ACTIVE;
			mAskedHosts      = new HashSet<string>();
            mHosts           = null;
			mCurrentHost     = -1;
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
			if (mHosts == null)
			{
				Global.clientScript.RequestHostList();
				mAskedHosts.Clear();
				
				mRequestDelay = mRequestDuration;
				mPollDelay    = DEFAULT_POLL_DURATION;
			}
        }

        /// <summary>
        /// Handler for poll timeout event.
        /// </summary>
        private void OnPollTimeout()
        {
            mHosts       = Global.clientScript.PollHostList();
			mCurrentHost = 0;

            mPollDelay = TIMER_NOT_ACTIVE;

			ConnectToHost();
        }

		/// <summary>
		/// Connects to current host in the array.
		/// </summary>
		private void ConnectToHost()
		{
			// TODO: [Trivial] Maybe move it to somewhere
			if (
				mRequestDelay != TIMER_NOT_ACTIVE
				&&
				mRequestDelay < 0f
			   )
			{
				StopPolling();
				return;
			}

			while (
				   mCurrentHost < mHosts.Length
				   &&
				   mAskedHosts.Contains(mHosts[mCurrentHost].guid)
			      )
			{
				++mCurrentHost;
			}

			if (mCurrentHost < mHosts.Length)
			{
				Network.Connect(mHosts[mCurrentHost]);
			}
			else
			{
				StartPolling();
			}
		}

		/// <summary>
		/// Starts hosts polling.
		/// </summary>
		private void StartPolling()
		{
			StopPolling();
			
			mPollDelay = DEFAULT_POLL_DURATION;
		}

		/// <summary>
		/// Stops hosts polling.
		/// </summary>
		private void StopPolling()
		{
			mHosts       = null;
			mCurrentHost = -1;
		}

		/// <summary>
		/// Handler for event on establishing connection.
		/// </summary>
		void OnConnectedToServer()
		{
			Debug.Log("Connected to server"); // TODO: Change it
		}

		/// <summary>
		/// Handler for event on disconnection.
		/// </summary>
		/// <param name="info">Disconnection info.</param>
		void OnDisconnectedFromServer(NetworkDisconnection info)
		{
			Debug.Log("Disconnected from server: " + info); // TODO: Change it
		}

		/// <summary>
		/// Handler for connecting failure event.
		/// </summary>
		/// <param name="error">Error description.</param>
		void OnFailedToConnect(NetworkConnectionError error)
		{
			Debug.LogError("Could not connect to server: " + error);
		}

		/// <summary>
		/// Handler for connecting failure event to the master server.
		/// </summary>
		/// <param name="error">Error description.</param>
		void OnFailedToConnectToMasterServer(NetworkConnectionError error)
		{
			Debug.LogError("Could not connect to master server: " + error);
		}

		/// <summary>
		/// Handler for master server event.
		/// </summary>
		/// <param name="msEvent">Master server event.</param>
		void OnMasterServerEvent(MasterServerEvent msEvent)
		{
			switch (msEvent)
			{
				case MasterServerEvent.HostListReceived:
				{
					// TODO: Need to handle it
				}
				break;

				case MasterServerEvent.RegistrationSucceeded:
				case MasterServerEvent.RegistrationFailedGameName:
				case MasterServerEvent.RegistrationFailedGameType:
				case MasterServerEvent.RegistrationFailedNoServer:
				{
					// Nothing
				}
				break;
				
				default:
				{
					Debug.LogError("Unknown master server event: " + msEvent);
				}
				break;
			}
		}
    }
}
