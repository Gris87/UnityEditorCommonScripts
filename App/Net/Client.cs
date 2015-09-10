using System.IO;
using UnityEngine;



namespace Common.App.Net
{
	/// <summary>
	/// Class that represents client.
	/// </summary>
    public static class Client
    {
		private static NetworkView sNetworkView;



		/// <summary>
		/// Initializes the <see cref="Common.App.Net.Client"/> class.
		/// </summary>
		static Client()
		{
			sNetworkView = new NetworkView();
		}

        /// <summary>
        /// Requests the host list.
        /// </summary>
        public static void RequestHostList()
        {
            MasterServer.RequestHostList(CommonConstants.SERVER_NAME);
        }

        /// <summary>
        /// Polls host list from master server.
        /// </summary>
        /// <returns>Host list.</returns>
		public static HostData[] PollHostList()
        {
            return MasterServer.PollHostList();
        }

		/// <summary>
		/// Fills the message header.
		/// </summary>
		/// <param name="writer">Binary writer.</param>
		/// <param name="type">Message type.</param>
		private static void FillMessageHeader(BinaryWriter writer, MessageType type)
		{
			writer.Write((byte)type);
		}

		/// <summary>
		/// Builds RevisionRequest message.
		/// </summary>
		/// <returns>Byte array that represents RevisionRequest message.</returns>
		public static byte[] BuildRevisionRequestMessage()
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);

			FillMessageHeader(writer, MessageType.RevisionRequest);

			return stream.GetBuffer();
		}

		/// <summary>
		/// Sends byte array to server.
		/// </summary>
		/// <param name="bytes">Byte array.</param>
		public static void Send(byte[] bytes)
		{
			sNetworkView.RPC("RPC_SendToServer", RPCMode.Server, bytes);
		}
    }
}
