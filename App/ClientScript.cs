using UnityEngine;



namespace Common.App
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
            // TODO: [Major] Implement ClientScript
			PollHostList();
        }

		public void PollHostList()
		{
			HostData[] hosts = MasterServer.PollHostList();

			Debug.Log(hosts.Length);
		}
    }
}
