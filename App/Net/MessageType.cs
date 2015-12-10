namespace Common.App.Net
{
    /// <summary>
    /// Message type.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Message type for requesting revision.
        /// </summary>
        RevisionRequest
		,
		/// <summary>
		/// Message type that indicates response for RevisionRequest message.
		/// </summary>
		RevisionResponse
        ,
		/// <summary>
		/// Message type for requesting MD5 hashes.
		/// </summary>
		MD5HashesRequest
		,
		/// <summary>
		/// Message type that indicates response for MD5HashesRequest message.
		/// </summary>
		MD5HashesResponse
		,
        Count
    }
}
