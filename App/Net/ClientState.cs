namespace Common.App.Net
{
    /// <summary>
    /// Client state.
    /// </summary>
    public enum ClientState
    {
        /// <summary>
        /// Client state when client disconnected from the host.
        /// </summary>
        Disconnected
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
        /// <summary>
        /// Client state when client requesting MD5 hashes for files from the host.
        /// </summary>
        RequestingMD5Hashes
        ,
        /// <summary>
        /// Client state when client downloading files from the host.
        /// </summary>
        Downloading
        ,
        Count
    }
}

