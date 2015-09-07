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
    }
}
