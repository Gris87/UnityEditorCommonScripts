using System.IO;



namespace Common.App.Net
{
    /// <summary>
    /// Class for network usefull functions.
    /// </summary>
    public static class NetUtils
    {
        /// <summary>
        /// Writes message header to binary writer.
        /// </summary>
        /// <param name="writer">Binary writer.</param>
        /// <param name="type">Message type.</param>
        public static void WriteMessageHeader(BinaryWriter writer, MessageType type)
        {
            DebugEx.VerboseFormat("NetUtils.WriteMessageHeader(writer = {0}, type = {1})", writer, type);

            writer.Write((byte)type);
        }

        /// <summary>
        /// Reads message header from binary reader.
        /// </summary>
        /// <returns>Message header.</returns>
        /// <param name="reader">Binary reader.</param>
        public static MessageType ReadMessageHeader(BinaryReader reader)
        {
            DebugEx.VerboseFormat("NetUtils.ReadMessageHeader(reader = {0})", reader);

            return (MessageType)reader.ReadByte();
        }

    }
}

