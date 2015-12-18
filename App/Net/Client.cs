using System.IO;
using System.Text;
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
			config.PacketSize       = CommonConstants.PACKET_SIZE;
			sChannelId              = config.AddChannel(QosType.ReliableSequenced);

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

            bool res = (error == (byte)NetworkError.Ok);

            if (!res)
            {
                DebugEx.ErrorFormat("Impossible to connect to the server, error: {0}({1})", (NetworkError)error, error);
            }

            DebugEx.VerboseFormat("Client.Connect() = {0}", res);

            return res;
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <returns><c>true</c>, if successfully disconnected, <c>false</c> otherwise.</returns>
        public static bool Disconnect()
        {
            byte error;
            NetworkTransport.Disconnect(sHostId, sConnectionId, out error);

            bool res = (error == (byte)NetworkError.Ok);

            if (!res)
            {
                DebugEx.ErrorFormat("Impossible to disconnect from the server, error: {0}({1})", (NetworkError)error, error);
            }

            DebugEx.VerboseFormat("Client.Disconnect() = {0}", res);

            return res;
        }

        /// <summary>
        /// Sends byte array to the server.
        /// </summary>
        /// <returns><c>true</c>, if successfully sent, <c>false</c> otherwise.</returns>
        /// <param name="bytes">Byte array.</param>
        public static bool Send(byte[] bytes)
        {
            byte error;
            NetworkTransport.Send(sHostId, sConnectionId, sChannelId, bytes, bytes.Length, out error);

            bool res = (error == (byte)NetworkError.Ok);

            if (res)
            {
                DebugEx.VeryVerboseFormat("Message sent to the server: {0}", Utils.BytesInHex(bytes));
            }
            else
            {
                DebugEx.ErrorFormat("Impossible to send message to the server, error: {0}({1})", (NetworkError)error, error);
            }

            DebugEx.VerboseFormat("Client.Send(bytes = {0}) = {1}", bytes, res);

            return res;
        }

        /// <summary>
        /// Builds RevisionRequest message.
        /// </summary>
        /// <returns>Byte array that represents RevisionRequest message.</returns>
        public static byte[] BuildRevisionRequestMessage()
        {
            DebugEx.Verbose("Client.BuildRevisionRequestMessage()");

            MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);

            NetUtils.WriteMessageHeader(writer, MessageType.RevisionRequest);

            return stream.ToArray();
        }

        /// <summary>
        /// Builds MD5HashesRequest message.
        /// </summary>
        /// <returns>Byte array that represents MD5HashesRequest message.</returns>
        public static byte[] BuildMD5HashesRequestMessage()
        {
            DebugEx.Verbose("Client.BuildMD5HashesRequestMessage()");

            MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);

            NetUtils.WriteMessageHeader(writer, MessageType.MD5HashesRequest);

            return stream.ToArray();
        }
    }
}
