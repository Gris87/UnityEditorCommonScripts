#pragma warning disable 618



#define LOOPBACK_SERVER



using System.Collections.Generic;
using System.IO;
using UnityEngine;



namespace Common.App.Net
{
    /// <summary>
    /// Client script.
    /// </summary>
    public class ClientScript : MonoBehaviour
    {
        #region States
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
                DebugEx.VerboseFormat("ClientScript.IClientState.OnEnter(script = {0}, previousState = {1})", script, previousState);
            }

            /// <summary>
            /// Handler for exit event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="nextState">Next state.</param>
            public virtual void OnExit(ClientScript script, ClientState nextState)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnExit(script = {0}, nextState = {1})", script, nextState);
            }

            /// <summary>
            /// Handler for request timeout event.
            /// </summary>
            /// <param name="script">Script.</param>
            public virtual void OnRequestTimeout(ClientScript script)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnRequestTimeout(script = {0})", script);

                DebugEx.FatalFormat("Unexpected OnRequestTimeout() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for poll timeout event.
            /// </summary>
            /// <param name="script">Script.</param>
            public virtual void OnPollTimeout(ClientScript script)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnPollTimeout(script = {0})", script);

                DebugEx.FatalFormat("Unexpected OnPollTimeout() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for event on establishing connection.
            /// </summary>
            /// <param name="script">Script.</param>
            public virtual void OnConnectedToServer(ClientScript script)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnConnectedToServer(script = {0})", script);

                DebugEx.FatalFormat("Unexpected OnConnectedToServer() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for event on disconnection.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="info">Disconnection info.</param>
            public virtual void OnDisconnectedFromServer(ClientScript script, NetworkDisconnection info)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnDisconnectedFromServer(script = {0}, info = {1})", script, info);

                DebugEx.FatalFormat("Unexpected OnDisconnectedFromServer() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for connecting failure event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error description.</param>
            public virtual void OnFailedToConnect(ClientScript script, NetworkConnectionError error)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnFailedToConnect(script = {0}, error = {1})", script, error);

                DebugEx.FatalFormat("Unexpected OnFailedToConnect() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for connecting failure event to the master server.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error description.</param>
            public virtual void OnFailedToConnectToMasterServer(ClientScript script, NetworkConnectionError error)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnFailedToConnectToMasterServer(script = {0}, error = {1})", script, error);

                DebugEx.FatalFormat("Unexpected OnFailedToConnectToMasterServer() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for master server event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="msEvent">Master server event.</param>
            public virtual void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnMasterServerEvent(script = {0}, msEvent = {1})", script, msEvent);

                DebugEx.FatalFormat("Unexpected OnMasterServerEvent() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for message received from server.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="bytes">Byte array.</param>
            public virtual void OnMessageReceivedFromServer(ClientScript script, byte[] bytes)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnMessageReceivedFromServer(script = {0}, bytes = {1})", script, Utils.BytesInHex(bytes));

                DebugEx.FatalFormat("Unexpected OnMessageReceivedFromServer() in {0} state", script.mState);
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
                DebugEx.VerboseFormat("ClientScript.RequestingState.OnEnter(script = {0}, previousState = {1})", script, previousState);

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
                DebugEx.VerboseFormat("ClientScript.RequestingState.OnRequestTimeout(script = {0})", script);

                Client.RequestHostList();

                script.mRequestTimer.Stop();
            }

			/// <summary>
			/// Handler for event on disconnection.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="info">Disconnection info.</param>
			public override void OnDisconnectedFromServer(ClientScript script, NetworkDisconnection info)
			{
				DebugEx.VerboseFormat("ClientScript.RequestingState.OnDisconnectedFromServer(script = {0}, info = {1})", script, info);
			}

            /// <summary>
            /// Handler for connecting failure event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error description.</param>
            public override void OnFailedToConnect(ClientScript script, NetworkConnectionError error)
            {
                DebugEx.VerboseFormat("ClientScript.RequestingState.OnFailedToConnect(script = {0}, error = {1})", script, error);
            }

            /// <summary>
            /// Handler for connecting failure event to the master server.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error description.</param>
            public override void OnFailedToConnectToMasterServer(ClientScript script, NetworkConnectionError error)
            {
                DebugEx.VerboseFormat("ClientScript.RequestingState.OnFailedToConnectToMasterServer(script = {0}, error = {1})", script, error);

                DebugEx.ErrorFormat("Could not connect to master server: {0}", error);

                script.mRequestTimer.Start();
            }

            /// <summary>
            /// Handler for master server event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="msEvent">Master server event.</param>
            public override void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
            {
                DebugEx.VerboseFormat("ClientScript.RequestingState.OnMasterServerEvent(script = {0}, msEvent = {1})", script, msEvent);

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
                        DebugEx.ErrorFormat("Unknown master server event: {0}", msEvent);
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
                DebugEx.VerboseFormat("ClientScript.PollingState.OnEnter(script = {0}, previousState = {1})", script, previousState);

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
                DebugEx.VerboseFormat("ClientScript.PollingState.OnRequestTimeout(script = {0})", script);

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
                DebugEx.VerboseFormat("ClientScript.PollingState.OnPollTimeout(script = {0})", script);

                script.mHosts       = Client.PollHostList();
                script.mCurrentHost = 0;

                script.mPollTimer.Stop();

				if (script.mHosts.Length > script.mAskedHosts.Count)
                {
                    script.state = ClientState.Asking;
                }
            }

			/// <summary>
			/// Handler for event on disconnection.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="info">Disconnection info.</param>
			public override void OnDisconnectedFromServer(ClientScript script, NetworkDisconnection info)
			{
				DebugEx.VerboseFormat("ClientScript.PollingState.OnDisconnectedFromServer(script = {0}, info = {1})", script, info);
			}

            /// <summary>
            /// Handler for master server event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="msEvent">Master server event.</param>
            public override void OnMasterServerEvent(ClientScript script, MasterServerEvent msEvent)
            {
                DebugEx.VerboseFormat("ClientScript.PollingState.OnMasterServerEvent(script = {0}, msEvent = {1})", script, msEvent);
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
                DebugEx.VerboseFormat("ClientScript.AskingState.OnEnter(script = {0}, previousState = {1})", script, previousState);

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
                DebugEx.VerboseFormat("ClientScript.ConnectingState.OnEnter(script = {0}, previousState = {1})", script, previousState);

                Network.Connect(script.mHosts[script.mCurrentHost]);
            }

            /// <summary>
            /// Handler for request timeout event.
            /// </summary>
            /// <param name="script">Script.</param>
            public override void OnRequestTimeout(ClientScript script)
            {
                DebugEx.VeryVeryVerboseFormat("ClientScript.ConnectingState.OnRequestTimeout(script = {0})", script);
            }

            /// <summary>
            /// Handler for event on establishing connection.
            /// </summary>
            /// <param name="script">Script.</param>
            public override void OnConnectedToServer(ClientScript script)
            {
                DebugEx.VerboseFormat("ClientScript.ConnectingState.OnConnectedToServer(script = {0})", script);

                script.state = ClientState.Connected;
            }

			/// <summary>
			/// Handler for event on disconnection.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="info">Disconnection info.</param>
			public override void OnDisconnectedFromServer(ClientScript script, NetworkDisconnection info)
			{
				DebugEx.VerboseFormat("ClientScript.ConnectingState.OnDisconnectedFromServer(script = {0}, info = {1})", script, info);
			}

            /// <summary>
            /// Handler for connecting failure event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error description.</param>
            public override void OnFailedToConnect(ClientScript script, NetworkConnectionError error)
            {
                DebugEx.VerboseFormat("ClientScript.ConnectingState.OnFailedToConnect(script = {0}, error = {1})", script, error);

                DebugEx.ErrorFormat("Could not connect to server: {0}", error);

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
                DebugEx.VerboseFormat("ClientScript.ConnectingState.OnMasterServerEvent(script = {0}, msEvent = {1})", script, msEvent);
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
                DebugEx.VerboseFormat("ClientScript.ConnectedState.OnEnter(script = {0}, previousState = {1})", script, previousState);

                script.Send(Client.BuildRevisionRequestMessage());
            }

			/// <summary>
			/// Handler for message received from server.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="bytes">Byte array.</param>
			public override void OnMessageReceivedFromServer(ClientScript script, byte[] bytes)
			{
				DebugEx.VerboseFormat("ClientScript.ConnectedState.OnMessageReceivedFromServer(script = {0}, bytes = {1})", script, Utils.BytesInHex(bytes));
				
				MemoryStream stream = new MemoryStream(bytes);
				BinaryReader reader = new BinaryReader(stream);
				
				MessageType messageType = NetUtils.ReadMessageHeader(reader);
				
                DebugEx.DebugFormat("Message type = {0}", messageType);

				switch (messageType)
				{
					case MessageType.RevisionResponse:
					{
						HandleRevisionResponse(reader);
					}
					break;
					
					case MessageType.RevisionRequest:
					{
						DebugEx.ErrorFormat("Unexpected message type: {0}", messageType);
	                }
                    break;
                    
	                default:
	                {
	                    DebugEx.ErrorFormat("Unknown message type: {0}", messageType);
	                }
                    break;
				}
            }

			/// <summary>
			/// Handles RevisionResponse message.
			/// </summary>
			/// <param name="reader">Binary reader.</param>
			private void HandleRevisionResponse(BinaryReader reader)
			{
				DebugEx.VerboseFormat("ClientScript.ConnectedState.HandleRevisionResponse(reader = {0})", reader);
                
				// TODO: Implement it
			}
            
            /// <summary>
            /// Handler for request timeout event.
            /// </summary>
            /// <param name="script">Script.</param>
            public override void OnRequestTimeout(ClientScript script)
            {
                DebugEx.VeryVeryVerboseFormat("ClientScript.ConnectedState.OnRequestTimeout(script = {0})", script);
            }

            /// <summary>
            /// Handler for event on disconnection.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="info">Disconnection info.</param>
            public override void OnDisconnectedFromServer(ClientScript script, NetworkDisconnection info)
            {
                DebugEx.VerboseFormat("ClientScript.ConnectedState.OnDisconnectedFromServer(script = {0}, info = {1})", script, info);

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
                DebugEx.VerboseFormat("ClientScript.ConnectedState.OnMasterServerEvent(script = {0}, msEvent = {1})", script, msEvent);
            }
        }
        #endregion

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
                DebugEx.VeryVeryVerboseFormat("ClientScript.state = {0}", mState);

                return mState;
            }

            set
            {
                DebugEx.VeryVerboseFormat("ClientScript.state: {0} => {1}", mState, value);

                if (mState != value)
                {
                    ClientState oldState = mState;
                    mState = value;

                    DebugEx.DebugFormat("Client state changed: {0} => {1}", oldState, mState);

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
                float res = mRequestTimer.duration;

                DebugEx.VeryVeryVerboseFormat("ClientScript.requestDuration = {0}", res);

                return res;
            }

            set
            {
                DebugEx.VeryVerboseFormat("ClientScript.requestDuration: {0} => {1}", mRequestTimer.duration, value);

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
            DebugEx.Verbose("ClientScript.Start()");

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
            DebugEx.VeryVeryVerbose("ClientScript.Update()");

            mRequestTimer.Update();
            mPollTimer.Update();
        }

        /// <summary>
        /// Handler for request timeout event.
        /// </summary>
        private void OnRequestTimeout()
        {
            DebugEx.VeryVeryVerbose("ClientScript.OnRequestTimeout()");

            mCurrentState.OnRequestTimeout(this);
        }

        /// <summary>
        /// Handler for poll timeout event.
        /// </summary>
        private void OnPollTimeout()
        {
            DebugEx.Verbose("ClientScript.OnPollTimeout()");

            mCurrentState.OnPollTimeout(this);
        }

        /// <summary>
        /// Handler for event on establishing connection.
        /// </summary>
        void OnConnectedToServer()
        {
            DebugEx.Verbose("ClientScript.OnConnectedToServer()");

            mCurrentState.OnConnectedToServer(this);
        }

        /// <summary>
        /// Handler for event on disconnection.
        /// </summary>
        /// <param name="info">Disconnection info.</param>
        void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            DebugEx.VerboseFormat("ClientScript.OnDisconnectedFromServer(info = {0})", info);

            mCurrentState.OnDisconnectedFromServer(this, info);
        }

        /// <summary>
        /// Handler for connecting failure event.
        /// </summary>
        /// <param name="error">Error description.</param>
        void OnFailedToConnect(NetworkConnectionError error)
        {
            DebugEx.VerboseFormat("ClientScript.OnFailedToConnect(error = {0})", error);

            mCurrentState.OnFailedToConnect(this, error);
        }

        /// <summary>
        /// Handler for connecting failure event to the master server.
        /// </summary>
        /// <param name="error">Error description.</param>
        void OnFailedToConnectToMasterServer(NetworkConnectionError error)
        {
            DebugEx.VerboseFormat("ClientScript.OnFailedToConnectToMasterServer(error = {0})", error);

            mCurrentState.OnFailedToConnectToMasterServer(this, error);
        }

        /// <summary>
        /// Handler for master server event.
        /// </summary>
        /// <param name="msEvent">Master server event.</param>
        void OnMasterServerEvent(MasterServerEvent msEvent)
        {
            DebugEx.VerboseFormat("ClientScript.OnMasterServerEvent(msEvent = {0})", msEvent);

            mCurrentState.OnMasterServerEvent(this, msEvent);
        }

        /// <summary>
        /// Sends byte array to server.
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        public void Send(byte[] bytes)
        {
            DebugEx.VerboseFormat("ClientScript.Send(bytes = {0})", Utils.BytesInHex(bytes));

            mNetworkView.RPC("RPC_SendToServer", RPCMode.Server, Network.player.guid, bytes);
        }

		/*
        /// <summary>
        /// RPC for sending message to server.
        /// </summary>
        /// <param name="id">Network view ID.</param>
        /// <param name="bytes">Byte array.</param>
        [RPC]
        private void RPC_SendToServer(NetworkViewID id, byte[] bytes)
        {
            DebugEx.VerboseFormat("ClientScript.RPC_SendToServer(id = {0}, bytes = {1})", id, Utils.BytesInHex(bytes));

            DebugEx.Fatal("Unexpected behaviour in ClientScript.RPC_SendToServer()");
        }
		*/

        /// <summary>
        /// RPC for receiving message from server.
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        [RPC]
        private void RPC_SendToClient(byte[] bytes)
        {
            DebugEx.VerboseFormat("ClientScript.RPC_SendToClient(bytes = {0})", Utils.BytesInHex(bytes));

            mCurrentState.OnMessageReceivedFromServer(this, bytes);
        }
    }
}
