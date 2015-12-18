using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



namespace Common.App.Net
{
    /// <summary>
    /// Client script.
    /// </summary>
    public class ClientScript : MonoBehaviour
    {
		/// <summary>
		/// File info.
		/// </summary>
		private class FileInfo
		{
			/// <summary>
			/// File name.
			/// </summary>
			public string name;
			
			/// <summary>
			/// Is file a folder or not.
			/// </summary>
			public bool isFolder;
			
			/// <summary>
			/// MD5 hash.
			/// </summary>
			public string md5;
			
			
			
			/// <summary>
			/// Initializes a new instance of the <see cref="Common.App.Net.ClientScript+FileInfo"/> class.
			/// </summary>
			/// <param name="fileName">File name.</param>
			/// <param name="md5Hash">Md5 hash.</param>
			public FileInfo(string fileName, string md5Hash)
			{
				name     = fileName;
				isFolder = false;
				md5      = md5Hash;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Common.App.Net.ClientScript+FileInfo"/> class.
			/// </summary>
			/// <param name="fileName">File name.</param>
			public FileInfo(string fileName)
			{
				name     = fileName;
				isFolder = true;
				md5      = null;
			}
		}

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
                DebugEx.VerboseFormat("ClientScript.IClientState.OnEnter(script = {0}, previousState = {1}) in {2} state", script, previousState, script.mState);
            }

            /// <summary>
            /// Handler for exit event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="nextState">Next state.</param>
            public virtual void OnExit(ClientScript script, ClientState nextState)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnExit(script = {0}, nextState = {1}) in {2} state", script, nextState, script.mState);
            }

            /// <summary>
            /// Handler for reconnect timeout event.
            /// </summary>
            /// <param name="script">Script.</param>
            public virtual void OnReconnectTimeout(ClientScript script)
            {
                DebugEx.VeryVeryVerboseFormat("ClientScript.IClientState.OnReconnectTimeout(script = {0}) in {1} state", script, script.mState);
            }

            /// <summary>
            /// Handler for event on establishing connection.
            /// </summary>
            /// <param name="script">Script.</param>
            public virtual void OnConnectedToServer(ClientScript script)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnConnectedToServer(script = {0}) in {1} state", script, script.mState);

                DebugEx.FatalFormat("Unexpected OnConnectedToServer() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for event on disconnection.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error.</param>
            public virtual void OnDisconnectedFromServer(ClientScript script, byte error)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnDisconnectedFromServer(script = {0}, error = {1}({2})) in {3} state", script, (NetworkError)error, error, script.mState);

                DebugEx.FatalFormat("Unexpected OnDisconnectedFromServer() in {0} state", script.mState);
            }

            /// <summary>
            /// Handler for message received from the server.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="bytes">Byte array.</param>
            /// <param name="dataSize">Data size.</param>
            public virtual void OnMessageReceivedFromServer(ClientScript script, byte[] bytes, int dataSize)
            {
                DebugEx.VerboseFormat("ClientScript.IClientState.OnMessageReceivedFromServer(script = {0}, bytes = {1}, dataSize = {2}) in {3} state", script, bytes, dataSize, script.mState);

                DebugEx.FatalFormat("Unexpected OnMessageReceivedFromServer() in {0} state", script.mState);
            }
        }

        /// <summary>
        /// Client state when client disconnected from the host.
        /// </summary>
        private class DisconnectedState: IClientState
        {
            /// <summary>
            /// Handler for enter event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="previousState">Previous state.</param>
            public override void OnEnter(ClientScript script, ClientState previousState)
            {
                DebugEx.VerboseFormat("ClientScript.DisconnectedState.OnEnter(script = {0}, previousState = {1})", script, previousState);

                if (previousState != ClientState.Count)
                {
                    if (!Client.Disconnect())
                    {
                        DebugEx.Error("Failed to disconnect from the server");
                    }
                }
            }

            /// <summary>
            /// Handler for event on disconnection.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error.</param>
            public override void OnDisconnectedFromServer(ClientScript script, byte error)
            {
                DebugEx.VerboseFormat("ClientScript.DisconnectedState.OnDisconnectedFromServer(script = {0}, error = {1}({2}))", script, (NetworkError)error, error);

                if (error == (byte)NetworkError.Ok)
                {
                    DebugEx.Debug("Successfully disconnected from the server");
                }
                else
                {
                    DebugEx.ErrorFormat("Failed to disconnect from the server, error: {0}", error);
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

                if (!Client.Connect())
                {
                    script.mReconnectTimer.Start();
                }
            }

            /// <summary>
            /// Handler for reconnect timeout event.
            /// </summary>
            /// <param name="script">Script.</param>
            public override void OnReconnectTimeout(ClientScript script)
            {
                DebugEx.VeryVeryVerboseFormat("ClientScript.ConnectingState.OnReconnectTimeout(script = {0})", script);

                if (Client.Connect())
                {
                    script.mReconnectTimer.Stop();
                }
                else
                {
                    script.mReconnectTimer.Start();
                }
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
            /// <param name="error">Error.</param>
            public override void OnDisconnectedFromServer(ClientScript script, byte error)
            {
                DebugEx.VerboseFormat("ClientScript.ConnectingState.OnDisconnectedFromServer(script = {0}, error = {1}({2}))", script, (NetworkError)error, error);

                DebugEx.ErrorFormat("Could not connect to the server: {0}", error);

                script.mReconnectTimer.Start();
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

                if (!Client.Send(Client.BuildRevisionRequestMessage()))
                {
                    DebugEx.ErrorFormat("Failed to send RevisionRequest message to the server");

                    script.state = ClientState.Disconnected;
                }
            }

            /// <summary>
            /// Handler for message received from the server.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="bytes">Byte array.</param>
            /// <param name="dataSize">Data size.</param>
            public override void OnMessageReceivedFromServer(ClientScript script, byte[] bytes, int dataSize)
            {
                DebugEx.VerboseFormat("ClientScript.ConnectedState.OnMessageReceivedFromServer(script = {0}, bytes = {1}, dataSize = {2})", script, bytes, dataSize);

				DebugEx.VeryVerboseFormat("Message received from the server: {0}", Utils.BytesInHex(bytes, dataSize));

                MemoryStream stream = new MemoryStream(bytes);
                BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

                MessageType messageType = NetUtils.ReadMessageHeader(reader);

                DebugEx.DebugFormat("Message type = {0}", messageType);

                switch (messageType)
                {
                    case MessageType.RevisionResponse:
                    {
                        HandleRevisionResponse(script, reader);
                    }
                    break;

                    case MessageType.RevisionRequest:
                    case MessageType.MD5HashesRequest:
                    case MessageType.MD5HashesResponse:
                    {
                        DebugEx.ErrorFormat("Unexpected message type: {0}", messageType);

						script.state = ClientState.Disconnected;
                    }
                    break;

                    default:
                    {
                        DebugEx.ErrorFormat("Unknown message type: {0}", messageType);

						script.state = ClientState.Disconnected;
                    }
                    break;
                }
            }

            /// <summary>
            /// Handles RevisionResponse message.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="reader">Binary reader.</param>
            private void HandleRevisionResponse(ClientScript script, BinaryReader reader)
            {
                DebugEx.VerboseFormat("ClientScript.ConnectedState.HandleRevisionResponse(reader = {0})", reader);

                int remoteRevision = reader.ReadInt32();
                int localRevision  = 0;

                if (File.Exists(Application.persistentDataPath + "/Revision.txt"))
                {
                    string text   = File.ReadAllText(Application.persistentDataPath + "/Revision.txt", Encoding.UTF8);
                    localRevision = int.Parse(text);
                }

                DebugEx.DebugFormat("Revision at the client: {0}", localRevision);
                DebugEx.DebugFormat("Revision at the server: {0}", remoteRevision);

                if (remoteRevision > localRevision)
                {
                    script.state = ClientState.RequestingMD5Hashes;
                }
                else
                {
                    script.state = ClientState.Disconnected;
                }
            }

            /// <summary>
            /// Handler for event on disconnection.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error.</param>
            public override void OnDisconnectedFromServer(ClientScript script, byte error)
            {
                DebugEx.VerboseFormat("ClientScript.ConnectedState.OnDisconnectedFromServer(script = {0}, error = {1}({2}))", script, (NetworkError)error, error);

                script.state = ClientState.Connecting;
            }
        }

        /// <summary>
        /// Client state when client requesting MD5 hashes for files from the host.
        /// </summary>
        private class RequestingMD5HashesState: IClientState
        {
            /// <summary>
            /// Handler for enter event.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="previousState">Previous state.</param>
            public override void OnEnter(ClientScript script, ClientState previousState)
            {
                DebugEx.VerboseFormat("ClientScript.RequestingMD5HashesState.OnEnter(script = {0}, previousState = {1})", script, previousState);

                if (!Client.Send(Client.BuildMD5HashesRequestMessage()))
                {
                    DebugEx.ErrorFormat("Failed to send MD5HashesRequest message to the server");

                    script.state = ClientState.Disconnected;
                }
            }

            /// <summary>
            /// Handler for message received from the server.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="bytes">Byte array.</param>
            /// <param name="dataSize">Data size.</param>
            public override void OnMessageReceivedFromServer(ClientScript script, byte[] bytes, int dataSize)
            {
                DebugEx.VerboseFormat("ClientScript.RequestingMD5HashesState.OnMessageReceivedFromServer(script = {0}, bytes = {1}, dataSize = {2})", script, bytes, dataSize);

				DebugEx.VeryVerboseFormat("Message received from the server: {0}", Utils.BytesInHex(bytes, dataSize));

                MemoryStream stream = new MemoryStream(bytes);
				BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

                MessageType messageType = NetUtils.ReadMessageHeader(reader);

                DebugEx.DebugFormat("Message type = {0}", messageType);

                switch (messageType)
                {
                    case MessageType.MD5HashesResponse:
                    {
					HandleMD5HashesResponse(script, reader, dataSize);
                    }
                    break;

                    case MessageType.RevisionRequest:
                    case MessageType.RevisionResponse:
                    case MessageType.MD5HashesRequest:
                    {
                        DebugEx.ErrorFormat("Unexpected message type: {0}", messageType);

						script.mFiles.Clear();
						script.state = ClientState.Disconnected;
                    }
                    break;

                    default:
                    {
                        DebugEx.ErrorFormat("Unknown message type: {0}", messageType);

						script.mFiles.Clear();
						script.state = ClientState.Disconnected;
                    }
                    break;
                }
            }

            /// <summary>
            /// Handles MD5HashesResponse message.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="reader">Binary reader.</param>
			/// <param name="dataSize">Data size.</param>
			private void HandleMD5HashesResponse(ClientScript script, BinaryReader reader, int dataSize)
            {
                DebugEx.VerboseFormat("ClientScript.RequestingMD5HashesState.HandleMD5HashesResponse(reader = {0})", reader);

				bool isContinued = reader.ReadBoolean();

				while (reader.BaseStream.Position < dataSize)
				{
					DebugEx.VerboseFormat("AAAA: {0} / {1}", reader.BaseStream.Position, dataSize);

					string fileName = reader.ReadString();

					DebugEx.VerboseFormat("BBBB: fileName = {0}", fileName);
					DebugEx.VerboseFormat("CCCC: {0} / {1}", reader.BaseStream.Position, dataSize);

					bool   isFolder = reader.ReadBoolean();

					DebugEx.VerboseFormat("DDDD: isFolder = {0}", isFolder);
					DebugEx.VerboseFormat("EEEE: {0} / {1}", reader.BaseStream.Position, dataSize);

					if (isFolder)
					{
						script.mFiles.Add(new FileInfo(fileName));
					}
					else
					{
						string md5Hash = reader.ReadString();
						DebugEx.VerboseFormat("FFFF: md5Hash = {0}", md5Hash);
						DebugEx.VerboseFormat("GGGG: {0} / {1}", reader.BaseStream.Position, dataSize);
						script.mFiles.Add(new FileInfo(fileName, md5Hash));
					}
				}

				if (!isContinued)
				{
					script.state = ClientState.Downloading;
				}
            }

            /// <summary>
            /// Handler for event on disconnection.
            /// </summary>
            /// <param name="script">Script.</param>
            /// <param name="error">Error.</param>
            public override void OnDisconnectedFromServer(ClientScript script, byte error)
            {
                DebugEx.VerboseFormat("ClientScript.RequestingMD5HashesState.OnDisconnectedFromServer(script = {0}, error = {1}({2}))", script, (NetworkError)error, error);

				script.mFiles.Clear();
                script.state = ClientState.Connecting;
            }
        }

        /// <summary>
        /// Client state when client downloading files from the host.
        /// </summary>
        private class DownloadingState: IClientState
        {
			/// <summary>
			/// Handler for enter event.
			/// </summary>
			/// <param name="script">Script.</param>
			/// <param name="previousState">Previous state.</param>
			public override void OnEnter(ClientScript script, ClientState previousState)
			{
				DebugEx.VerboseFormat("ClientScript.DownloadingState.OnEnter(script = {0}, previousState = {1})", script, previousState);
				
				// TODO: Remove it
				for (int i = 0; i < script.mFiles.Count; ++i)
				{
					DebugEx.ErrorFormat("File: {0} {1} {2}", script.mFiles[i].name, script.mFiles[i].isFolder, script.mFiles[i].md5);
				}
			}

            // TODO: Implement it
        }
        #endregion

        // =======================================================================

        private const float DEFAULT_RECONNECT_DURATION = 3000f / 1000f;



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
                    mCurrentState.OnExit(this, value);



                    ClientState oldState = mState;
                    mState = value;

                    DebugEx.DebugFormat("Client state changed: {0} => {1}", oldState, mState);



                    mCurrentState = mAllStates[(int)mState];
                    mCurrentState.OnEnter(this, oldState);
                }
            }
        }



        private IClientState[] mAllStates;
        private ClientState    mState;
        private IClientState   mCurrentState;

        private Timer          mReconnectTimer;

        private byte[]         mBuffer;
		private List<FileInfo> mFiles;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
            DebugEx.Verbose("ClientScript.Start()");

            mAllStates                                       = new IClientState[(int)ClientState.Count];
            mAllStates[(int)ClientState.Disconnected]        = new DisconnectedState();
            mAllStates[(int)ClientState.Connecting]          = new ConnectingState();
            mAllStates[(int)ClientState.Connected]           = new ConnectedState();
            mAllStates[(int)ClientState.RequestingMD5Hashes] = new RequestingMD5HashesState();
            mAllStates[(int)ClientState.Downloading]         = new DownloadingState();

            mState        = ClientState.Connecting;
            mCurrentState = mAllStates[(int)mState];

            mReconnectTimer = new Timer(OnReconnectTimeout, DEFAULT_RECONNECT_DURATION);

			mBuffer = new byte[CommonConstants.PACKET_SIZE];
            mFiles  = new List<FileInfo>();



            mCurrentState.OnEnter(this, ClientState.Count);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
            DebugEx.VeryVeryVerbose("ClientScript.Update()");

            mReconnectTimer.Update();

            int hostId;
            int connectionId;
            int channelId;
            int dataSize;
            byte error;

			NetworkEventType eventType = NetworkTransport.Receive(out hostId, out connectionId, out channelId, mBuffer, CommonConstants.PACKET_SIZE, out dataSize, out error);

            switch (eventType)
            {
                case NetworkEventType.Nothing:
                {
                    // Nothing
                }
                break;

                case NetworkEventType.ConnectEvent:
                {
                    mCurrentState.OnConnectedToServer(this);
                }
                break;

                case NetworkEventType.DataEvent:
                {
                    mCurrentState.OnMessageReceivedFromServer(this, mBuffer, dataSize);
                }
                break;

                case NetworkEventType.DisconnectEvent:
                {
                    mCurrentState.OnDisconnectedFromServer(this, error);
                }
                break;

                case NetworkEventType.BroadcastEvent:
                {
                    DebugEx.ErrorFormat("Unexpected event type: {0}", eventType);
                }
                break;

                default:
                {
                    DebugEx.ErrorFormat("Unknown event type: {0}", eventType);
                }
                break;
            }
        }

        /// <summary>
        /// Handler for reconnect timeout event.
        /// </summary>
        private void OnReconnectTimeout()
        {
            DebugEx.VeryVeryVerbose("ClientScript.OnReconnectTimeout()");

            mCurrentState.OnReconnectTimeout(this);
        }
    }
}
