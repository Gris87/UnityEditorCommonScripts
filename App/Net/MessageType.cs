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
        Count
    }
}
