#define LOOPBACK_SERVER



using System.Collections.Generic;
using UnityEngine;



namespace Common.App.Net
{
    /// <summary>
    /// Client script.
    /// </summary>
    public class ClientScript : MonoBehaviour
    {
		/// <summary>
		/// Client state.
		/// </summary>
        private class IClientState
		{
			/// <summary>
			/// Handler for enter event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="previousState">Previous state.</param>
			public virtual void OnEnter(ClientScript script, ClientState previousState)
			{
				// Nothing
			}

			/// <summary>
			/// Handler for exit event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="nextState">Next state.</param>
			public virtual void OnExit(ClientScript script, ClientState nextState)
			{
				// Nothing
            }

			/// <summary>
			/// Handler for request timeout event.
            /// </summary>
			/// <param name="script">Script.</param>
			public virtual void OnRequestTimeout(ClientScript script)
			{
				Debug.LogError("Unexpected OnRequestTimeout in " + script.mState + " state");
			}

			/// <summary>
			/// Handler for poll timeout event.
			/// </summary>
			/// <param name="script">Script.</param>
			public virtual void OnPollTimeout(ClientScript script)
            {
				Debug.LogError("Unexpected OnPollTimeout in " + script.mState + " state");
            }

			/// <summary>
			/// Handler for master server event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="msEvent">Master server event.</param>
			public virtual void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
			{
				Debug.LogError("Unexpected OnMasterServerEvent in " + script.mState + " state");
			}
		}
            
        /// <summary>
		/// Client state when client requesting host list.
		/// </summary>
		private class RequestingState: IClientState
		{
			/// <summary>
			/// Handler for enter event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="previousState">Previous state.</param>
			public override void OnEnter(ClientScript script, ClientState previousState)
            {
				Client.RequestHostList();
				script.mAskedHosts.Clear();

				script.mRequestTimer.Start();
            }
        }
        
		/// <summary>
		/// Client state when client polling host list.
		/// </summary>
		private class PollingState: IClientState
		{
			/// <summary>
			/// Handler for enter event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="previousState">Previous state.</param>
			public override void OnEnter(ClientScript script, ClientState previousState)
			{

			}
        }

		/// <summary>
		/// Client state when client asking hosts about files revision.
		/// </summary>
		private class AskingState: IClientState
		{
			
        }
        

        
        private const float TIMER_NOT_ACTIVE         = -10000f;
        private const float DEFAULT_REQUEST_DURATION = 60000f / 1000f;
        private const float DEFAULT_POLL_DURATION    = 1000f  / 1000f;



		/// <summary>
		/// Gets or sets client state.
		/// </summary>
		/// <value>Client state.</value>
		public ClientState state
		{
			get
			{
				return mState; 
			}

			set
			{
				if (mState != value)
				{
					mCurrentState.OnExit(this, value);
					mCurrentState = mAllStates[(int)value];
					mCurrentState.OnEnter(this, mState);

					Debug.Log("Client state changed: " + mState + " => " + value);

					mState = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets the duration of the request.
        /// </summary>
        /// <value>The duration of the request.</value>
        public float requestDuration
        {
            get
            {
                return mRequestTimer.duration;
            }

            set
            {
				mRequestTimer.duration = value;
            }
        }



		private IClientState    mAllStates;
		private ClientState     mState;
		private IClientState    mCurrentState;

		private Timer           mRequestTimer;
        private Timer           mPollTimer;
		private HashSet<string> mAskedHosts;
        private HostData[]      mHosts;
		private int             mCurrentHost;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
			mAllStates                              = new IClientState[(int)ClientState.Count];
			mAllStates[(int)ClientState.Requesting] = new RequestingState();
			mAllStates[(int)ClientState.Polling]    = new PollingState();
			mAllStates[(int)ClientState.Asking]     = new AskingState();

			mState        = ClientState.Requesting;
			mCurrentState = mAllStates[(int)mState];

			mRequestTimer = new Timer(OnRequestTimeout, DEFAULT_REQUEST_DURATION);
			mPollTimer    = new Timer(OnPollTimeout);
			mAskedHosts   = new HashSet<string>();
            mHosts        = null;
			mCurrentHost  = -1;

#if LOOPBACK_SERVER
			MasterServer.ipAddress     = "127.0.0.1";
			MasterServer.port          = 23466;
			Network.natFacilitatorIP   = "127.0.0.1";
			Network.natFacilitatorPort = 50005;
#endif

			mCurrentState.OnEnter(ClientState.Count);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
			mRequestTimer.Update();                
			mPollTimer.Update();
        }

        /// <summary>
        /// Handler for request timeout event.
        /// </summary>
        private void OnRequestTimeout()
        {
			mCurrentState.OnRequestTimeout(this);

			if (mHosts == null)
			{
				Client.RequestHostList();
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
			mCurrentState.OnPollTimeout(this);

			mHosts       = Client.PollHostList();
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

			Debug.LogError(mHosts.Length);

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
					state = ClientState.Polling;
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
