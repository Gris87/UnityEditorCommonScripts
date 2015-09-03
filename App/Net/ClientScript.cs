#define LOOPBACK_SERVER



using UnityEngine;



namespace Common.App.Net
{
    /// <summary>
    /// Client script.
    /// </summary>
    public class ClientScript : MonoBehaviour
    {
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
        }

		/// <summary>
		/// Requests the host list.
		/// </summary>
		public void RequestHostList()
		{
			MasterServer.RequestHostList(CommonConstants.SERVER_NAME);
		}

		/// <summary>
		/// Polls host list from master server.
		/// </summary>
		/// <returns>Host list.</returns>
		public HostData[] PollHostList()
		{
			return MasterServer.PollHostList();
		}
    }
}
