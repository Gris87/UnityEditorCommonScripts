using System.Diagnostics;



namespace Common
{
    /// <summary>
    /// Debug extension.
    /// </summary>
    public static class DebugEx
    {
        /// <summary>
        /// Logs verbose message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_VERBOSE")]
        public static void Verbose(object message, UnityEngine.Object context = null)
        {
            Common.Debug.Log(message, context);
        }

        /// <summary>
        /// Logs a formatted verbose message to the Unity Console.
        /// </summary>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_VERBOSE")]
        public static void VerboseFormat(UnityEngine.Object context, string format, params object[] args)
        {
            Common.Debug.LogFormat(context, format, args);
        }

        /// <summary>
        /// Logs a formatted verbose message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_VERBOSE")]
        public static void VerboseFormat(string format, params object[] args)
        {
            Common.Debug.LogFormat(format, args);
        }

        /// <summary>
        /// Logs debug message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_DEBUG")]
        public static void Debug(object message, UnityEngine.Object context = null)
        {
            Common.Debug.Log(message, context);
        }

        /// <summary>
        /// Logs a formatted debug message to the Unity Console.
        /// </summary>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_DEBUG")]
        public static void DebugFormat(UnityEngine.Object context, string format, params object[] args)
        {
            Common.Debug.LogFormat(context, format, args);
        }

        /// <summary>
        /// Logs a formatted debug message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_DEBUG")]
        public static void DebugFormat(string format, params object[] args)
        {
            Common.Debug.LogFormat(format, args);
        }

        /// <summary>
        /// Logs info message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_INFO")]
        public static void Info(object message, UnityEngine.Object context = null)
        {
            Common.Debug.Log(message, context);
        }

        /// <summary>
        /// Logs a formatted info message to the Unity Console.
        /// </summary>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_INFO")]
        public static void InfoFormat(UnityEngine.Object context, string format, params object[] args)
        {
            Common.Debug.LogFormat(context, format, args);
        }

        /// <summary>
        /// Logs a formatted info message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_INFO")]
        public static void InfoFormat(string format, params object[] args)
        {
            Common.Debug.LogFormat(format, args);
        }

        /// <summary>
        /// Logs warning message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_WARNING")]
        public static void Warning(object message, UnityEngine.Object context = null)
        {
            Common.Debug.LogWarning(message, context);
        }

        /// <summary>
        /// Logs a formatted warning message to the Unity Console.
        /// </summary>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_WARNING")]
        public static void WarningFormat(UnityEngine.Object context, string format, params object[] args)
        {
            Common.Debug.LogWarningFormat(context, format, args);
        }

        /// <summary>
        /// Logs a formatted warning message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_WARNING")]
        public static void WarningFormat(string format, params object[] args)
        {
            Common.Debug.LogWarningFormat(format, args);
        }

        /// <summary>
        /// Logs error message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_ERROR")]
        public static void Error(object message, UnityEngine.Object context = null)
        {
            Common.Debug.LogError(message, context);
        }

        /// <summary>
        /// Logs a formatted error message to the Unity Console.
        /// </summary>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_ERROR")]
        public static void ErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            Common.Debug.LogErrorFormat(context, format, args);
        }

        /// <summary>
        /// Logs a formatted error message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_ERROR")]
        public static void ErrorFormat(string format, params object[] args)
        {
            Common.Debug.LogErrorFormat(format, args);
        }

		/// <summary>
		/// Logs user interaction to the Unity Console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("LOGGING_UI")]
		public static void UserInteraction(object message, UnityEngine.Object context = null)
		{
			Common.Debug.Log(message, context);
		}
		
		/// <summary>
		/// Logs a formatted user interaction to the Unity Console.
		/// </summary>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_UI")]
		public static void UserInteractionFormat(UnityEngine.Object context, string format, params object[] args)
		{
			Common.Debug.LogFormat(context, format, args);
		}
		
		/// <summary>
		/// Logs a formatted user interaction to the Unity Console.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_UI")]
		public static void UserInteractionFormat(string format, params object[] args)
		{
			Common.Debug.LogFormat(format, args);
		}
    }
}
