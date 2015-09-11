namespace Common.App.Net
{
	/// <summary>
	/// Client state.
	/// </summary>
	public enum ClientState
	{
		/// <summary>
		/// Client state when client requesting host list.
		/// </summary>
		Requesting
		,
		/// <summary>
		/// Client state when client polling host list.
		/// </summary>
		Polling
		,
		/// <summary>
		/// Client state when client asking hosts about files revision.
		/// </summary>
		Asking
		,
		/// <summary>
		/// Client state when client connecting to the host.
		/// </summary>
		Connecting
		,
		/// <summary>
		/// Client state when client connected to the host.
		/// </summary>
		Connected
		,
		Count
	}
}
