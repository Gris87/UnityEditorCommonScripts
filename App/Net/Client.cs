#pragma warning disable 618



using System.IO;
using UnityEngine;



namespace Common.App.Net
{
    /// <summary>
    /// Class that represents client.
    /// </summary>
    public static class Client
    {
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

            return stream.ToArray();
        }
    }
}
