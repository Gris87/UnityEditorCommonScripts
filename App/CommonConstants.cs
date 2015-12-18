namespace Common.App
{
    /// <summary>
    /// Class with constants.
    /// </summary>
    public static class CommonConstants
    {
        /// <summary>
        /// Source code URL.
        /// </summary>
        public const string SOURCE_CODE_URL = "https://github.com/Gris87/UnityEditor";

        /// <summary>
        /// Server port.
        /// </summary>
        public const int SERVER_PORT = 52794;

		/// <summary>
		/// Packet size.
		/// </summary>
		public const int PACKET_SIZE = 5000; // Must be more than PACKET_SAFE_LIMIT

		/// <summary>
		/// The safe limit to avoid packet size exhaustion.
		/// </summary>
		public const int PACKET_SAFE_LIMIT = 2000;
    }
}
