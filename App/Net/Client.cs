using System.IO;
using UnityEngine;
using UnityEngine.Networking;



namespace Common.App.Net
{
    /// <summary>
    /// Class that represents client.
    /// </summary>
    public static class Client
    {
		private static byte         sChannelId;
		private static HostTopology sTopology;
		private static int          sHostId;
		private static int          sConnectionId;
		
		
		
		/// <summary>
		/// Initializes the <see cref="Common.App.Net.Client"/> class.
		/// </summary>
		static Client()
		{
			DebugEx.Verbose("Static class Client initialized");
			
			NetworkTransport.Init();
			
			ConnectionConfig config = new ConnectionConfig();
			sChannelId = config.AddChannel(QosType.ReliableSequenced);
            
            sTopology     = new HostTopology(config, 1);
			sHostId       = NetworkTransport.AddHost(sTopology);
			sConnectionId = -1;
        }
        
        /// <summary>
        /// Connects to the server.
		/// </summary>
		/// <returns><c>true</c>, if connection successfully started, <c>false</c> otherwise.</returns>
        public static bool Connect()
		{
			byte error;

			sConnectionId = NetworkTransport.Connect(sHostId, "127.0.0.1", CommonConstants.SERVER_PORT, 0, out error);

			if (error != 0)
			{
				DebugEx.ErrorFormat("Impossible to connect to server, error: {0}", error);
			}

			return (error == 0);
		}

		/// <summary>
		/// Sends byte array to server.
		/// </summary>
		/// <returns><c>true</c>, if successfully sent, <c>false</c> otherwise.</returns>
		/// <param name="bytes">Byte array.</param>
		public static bool Send(byte[] bytes)
		{
			DebugEx.VerboseFormat("Client.Send(bytes = {0})", Utils.BytesInHex(bytes));
			
			byte error;
			NetworkTransport.Send(sHostId, sConnectionId, sChannelId, bytes, bytes.Length, out error);

			if (error == 0)
			{
				DebugEx.DebugFormat("Message sent to server: {0}", Utils.BytesInHex(bytes));
			}
			else
			{
				DebugEx.ErrorFormat("Impossible to send message to server, error: {0}", error);
            }

			return (error == 0);
        }

        /// <summary>
        /// Builds RevisionRequest message.
        /// </summary>
        /// <returns>Byte array that represents RevisionRequest message.</returns>
        public static byte[] BuildRevisionRequestMessage()
        {
            DebugEx.Verbose("Client.BuildRevisionRequestMessage()");

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

			NetUtils.WriteMessageHeader(writer, MessageType.RevisionRequest);

            return stream.ToArray();
        }
    }
}
