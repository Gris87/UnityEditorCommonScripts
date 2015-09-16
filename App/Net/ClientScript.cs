#pragma warning disable 618



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
				DebugEx.Error("Unexpected OnRequestTimeout in " + script.mState + " state");
			}

			/// <summary>
			/// Handler for poll timeout event.
			/// </summary>
			/// <param name="script">Script.</param>
			public virtual void OnPollTimeout(ClientScript script)
            {
				DebugEx.Error("Unexpected OnPollTimeout in " + script.mState + " state");
            }

			/// <summary>
			/// Handler for event on establishing connection.
			/// </summary>
			/// <param name="script">Script.</param>
			public virtual void OnConnectedToServer(ClientScript script)
			{
				DebugEx.Error("Unexpected OnConnectedToServer in " + script.mState + " state");
			}
			
			/// <summary>
			/// Handler for event on disconnection.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="info">Disconnection info.</param>
			public virtual void OnDisconnectedFromServer(ClientScript script, NetworkDisconnection info)
			{
				DebugEx.Error("Unexpected OnDisconnectedFromServer in " + script.mState + " state");
			}
			
			/// <summary>
			/// Handler for connecting failure event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="error">Error description.</param>
			public virtual void OnFailedToConnect(ClientScript script, NetworkConnectionError error)
			{
				DebugEx.Error("Unexpected OnFailedToConnect in " + script.mState + " state");
			}
			
			/// <summary>
			/// Handler for connecting failure event to the master server.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="error">Error description.</param>
			public virtual void OnFailedToConnectToMasterServer(ClientScript script, NetworkConnectionError error)
			{
				DebugEx.Error("Unexpected OnFailedToConnectToMasterServer in " + script.mState + " state");
			}

			/// <summary>
			/// Handler for master server event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="msEvent">Master server event.</param>
			public virtual void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
			{
				DebugEx.Error("Unexpected OnMasterServerEvent in " + script.mState + " state");
			}

			/// <summary>
			/// Handler for message received from server.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="bytes">Byte array.</param>
			public virtual void OnMessageReceivedFromServer(ClientScript script, byte[] bytes)
			{
				DebugEx.Error("Unexpected OnMessageReceivedFromServer in " + script.mState + " state");
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
				if (previousState != ClientState.Count)
				{
					script.mAskedHosts.Clear();
					script.mHosts       = null;
					script.mCurrentHost = -1;
				}

				OnRequestTimeout(script);
            }

			/// <summary>
			/// Handler for request timeout event.
			/// </summary>
			/// <param name="script">Script.</param>
			public override void OnRequestTimeout(ClientScript script)
			{
				Client.RequestHostList();

				script.mRequestTimer.Stop();
			}

			/// <summary>
			/// Handler for connecting failure event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="error">Error description.</param>
			public override void OnFailedToConnect(ClientScript script, NetworkConnectionError error)
			{
				// Nothing
			}

			/// <summary>
			/// Handler for connecting failure event to the master server.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="error">Error description.</param>
			public override void OnFailedToConnectToMasterServer(ClientScript script, NetworkConnectionError error)
			{
				DebugEx.Error("Could not connect to master server: " + error);

				script.mRequestTimer.Start();
			}

			/// <summary>
			/// Handler for master server event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="msEvent">Master server event.</param>
			public override void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
			{
				switch (msEvent)
				{
					case MasterServerEvent.HostListReceived:
					{
						script.mRequestTimer.Start();

						script.state = ClientState.Polling;
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
						DebugEx.Error("Unknown master server event: " + msEvent);
					}
					break;
				}
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
				if (previousState == ClientState.Requesting)
				{
					OnPollTimeout(script);
				}
				else
				{
					script.mPollTimer.Start();
				}
			}

			/// <summary>
			/// Handler for request timeout event.
			/// </summary>
			/// <param name="script">Script.</param>
			public override void OnRequestTimeout(ClientScript script)
			{
				script.mRequestTimer.Stop();
				script.mPollTimer.Stop();

				script.state = ClientState.Requesting;
			}

			/// <summary>
			/// Handler for poll timeout event.
			/// </summary>
			/// <param name="script">Script.</param>
			public override void OnPollTimeout(ClientScript script)
			{
				script.mHosts       = Client.PollHostList();
				script.mCurrentHost = 0;

				script.mPollTimer.Stop();

				if (script.mHosts.Length > 0)
				{
					script.state = ClientState.Asking;
				}
			}

			/// <summary>
			/// Handler for master server event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="msEvent">Master server event.</param>
			public override void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
			{
				// Nothing
			}
        }

		/// <summary>
		/// Client state when client asking hosts about files revision.
		/// </summary>
		private class AskingState: IClientState
		{
			/// <summary>
			/// Handler for enter event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="previousState">Previous state.</param>
			public override void OnEnter(ClientScript script, ClientState previousState)
			{
				while (
				       script.mCurrentHost < script.mHosts.Length
					   &&
					   script.mAskedHosts.Contains(script.mHosts[script.mCurrentHost].guid)
					  )
				{
					++script.mCurrentHost;
				}

				if (script.mCurrentHost < script.mHosts.Length)
				{
					script.state = ClientState.Connecting;
				}
				else
				{
					if (script.mRequestTimer.isAboutToShot)
					{
						script.state = ClientState.Requesting;
					}
					else
					{
						script.state = ClientState.Polling;
					}
				}
			}
        }

		/// <summary>
		/// Client state when client connecting to the host.
		/// </summary>
		private class ConnectingState: IClientState
		{
			/// <summary>
			/// Handler for enter event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="previousState">Previous state.</param>
			public override void OnEnter(ClientScript script, ClientState previousState)
			{
				Network.Connect(script.mHosts[script.mCurrentHost]);
			}

			/// <summary>
			/// Handler for request timeout event.
			/// </summary>
			/// <param name="script">Script.</param>
			public override void OnRequestTimeout(ClientScript script)
			{
				// Nothing
			}

			/// <summary>
			/// Handler for event on establishing connection.
			/// </summary>
			/// <param name="script">Script.</param>
			public override void OnConnectedToServer(ClientScript script)
			{
				script.state = ClientState.Connected;
			}

			/// <summary>
			/// Handler for connecting failure event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="error">Error description.</param>
			public override void OnFailedToConnect(ClientScript script, NetworkConnectionError error)
			{
				DebugEx.Error("Could not connect to server: " + error);

				++script.mCurrentHost;
				script.state = ClientState.Asking;
			}			
			
			/// <summary>
			/// Handler for master server event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="msEvent">Master server event.</param>
			public override void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
			{
				// Nothing
			}
		}

		/// <summary>
		/// Client state when client connected to the host.
		/// </summary>
		private class ConnectedState: IClientState
		{
			/// <summary>
			/// Handler for enter event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="previousState">Previous state.</param>
			public override void OnEnter(ClientScript script, ClientState previousState)
			{
				script.Send(Client.BuildRevisionRequestMessage());
			}
			
			/// <summary>
			/// Handler for request timeout event.
			/// </summary>
			/// <param name="script">Script.</param>
			public override void OnRequestTimeout(ClientScript script)
			{
				// Nothing
			}

			/// <summary>
			/// Handler for event on disconnection.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="info">Disconnection info.</param>
			public override void OnDisconnectedFromServer(ClientScript script, NetworkDisconnection info)
			{
				++script.mCurrentHost;
				script.state = ClientState.Asking;
			}
			
			/// <summary>
			/// Handler for master server event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="msEvent">Master server event.</param>
			public override void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
			{
				// Nothing
			}
		}
        
		// =======================================================================
        
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
				DebugEx.Verbose("mState: " + mState + " => " + value);

				if (mState != value)
				{
					ClientState oldState = mState;
					mState = value;

					DebugEx.Debug("Client state changed: " + oldState + " => " + mState);

					mCurrentState.OnExit(this, mState);
					mCurrentState = mAllStates[(int)mState];
					mCurrentState.OnEnter(this, oldState);
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



		private IClientState[]  mAllStates;
		private ClientState     mState;
		private IClientState    mCurrentState;

		private Timer           mRequestTimer;
        private Timer           mPollTimer;

		private HashSet<string> mAskedHosts;
        private HostData[]      mHosts;
		private int             mCurrentHost;

		private NetworkView     mNetworkView;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
#if LOOPBACK_SERVER
			MasterServer.ipAddress     = "127.0.0.1";
			MasterServer.port          = 23466;
			Network.natFacilitatorIP   = "127.0.0.1";
			Network.natFacilitatorPort = 50005;
#endif

			mAllStates                              = new IClientState[(int)ClientState.Count];
			mAllStates[(int)ClientState.Requesting] = new RequestingState();
			mAllStates[(int)ClientState.Polling]    = new PollingState();
			mAllStates[(int)ClientState.Asking]     = new AskingState();
			mAllStates[(int)ClientState.Connecting] = new ConnectingState();
			mAllStates[(int)ClientState.Connected]  = new ConnectedState();

			mState        = ClientState.Requesting;
			mCurrentState = mAllStates[(int)mState];

			mRequestTimer = new Timer(OnRequestTimeout, DEFAULT_REQUEST_DURATION);
			mPollTimer    = new Timer(OnPollTimeout,    DEFAULT_POLL_DURATION);

			mAskedHosts  = new HashSet<string>();
            mHosts       = null;
			mCurrentHost = -1;

			mNetworkView = gameObject.AddComponent<NetworkView>();



			mCurrentState.OnEnter(this, ClientState.Count);
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
        }

        /// <summary>
        /// Handler for poll timeout event.
        /// </summary>
        private void OnPollTimeout()
        {
			mCurrentState.OnPollTimeout(this);
        }

		/// <summary>
		/// Handler for event on establishing connection.
		/// </summary>
		void OnConnectedToServer()
		{
			mCurrentState.OnConnectedToServer(this);
		}

		/// <summary>
		/// Handler for event on disconnection.
		/// </summary>
		/// <param name="info">Disconnection info.</param>
		void OnDisconnectedFromServer(NetworkDisconnection info)
		{
			mCurrentState.OnDisconnectedFromServer(this, info);
		}

		/// <summary>
		/// Handler for connecting failure event.
		/// </summary>
		/// <param name="error">Error description.</param>
		void OnFailedToConnect(NetworkConnectionError error)
		{
			mCurrentState.OnFailedToConnect(this, error);
		}

		/// <summary>
		/// Handler for connecting failure event to the master server.
		/// </summary>
		/// <param name="error">Error description.</param>
		void OnFailedToConnectToMasterServer(NetworkConnectionError error)
		{
			mCurrentState.OnFailedToConnectToMasterServer(this, error);
		}

		/// <summary>
		/// Handler for master server event.
		/// </summary>
		/// <param name="msEvent">Master server event.</param>
		void OnMasterServerEvent(MasterServerEvent msEvent)
		{
			mCurrentState.OnMasterServerEvent(this, msEvent);
		}

		/// <summary>
		/// Sends byte array to server.
		/// </summary>
		/// <param name="bytes">Byte array.</param>
		public void Send(byte[] bytes)
		{
			mNetworkView.RPC("RPC_SendToServer", RPCMode.Server, mNetworkView.viewID, bytes);
		}

		/// <summary>
		/// RPC for sending message to server.
		/// </summary>
		/// <param name="id">Network view ID.</param>
		/// <param name="bytes">Byte array.</param>
		[RPC]
		private void RPC_SendToServer(NetworkViewID id, byte[] bytes)
		{
			// Nothing
		}

		/// <summary>
		/// RPC for receiving message from server.
		/// </summary>
		/// <param name="bytes">Byte array.</param>
		[RPC]
		private void RPC_SendToClient(byte[] bytes)
		{
			mCurrentState.OnMessageReceivedFromServer(this, bytes);
		}
    }
}
