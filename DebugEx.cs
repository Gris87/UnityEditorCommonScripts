using System.Diagnostics;



namespace Common
{
    /// <summary>
    /// Debug extension.
    /// </summary>
    public static class DebugEx
    {
		private const string VERBOSE_PREFIX          = "|V| ";
		private const string DEBUG_PREFIX            = "|D| ";
		private const string INFO_PREFIX             = "|I| ";
		private const string WARNING_PREFIX          = "|W| ";
		private const string ERROR_PREFIX            = "|E| ";
		private const string FATAL_PREFIX            = "|F| ";
		private const string USER_INTERACTION_PREFIX = "|U| ";



		/// <summary>
		/// Logs verbose message to the Unity Console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("LOGGING_VERY_VERY_VERBOSE")]
		public static void VeryVeryVerbose(object message, UnityEngine.Object context = null)
		{
			Common.Debug.Log(VERBOSE_PREFIX + message, context);
		}
		
		/// <summary>
		/// Logs a formatted verbose message to the Unity Console.
		/// </summary>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_VERY_VERY_VERBOSE")]
		public static void VeryVeryVerboseFormat(UnityEngine.Object context, string format, params object[] args)
		{
			Common.Debug.LogFormat(context, VERBOSE_PREFIX + format, args);
		}
		
		/// <summary>
		/// Logs a formatted verbose message to the Unity Console.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_VERY_VERY_VERBOSE")]
		public static void VeryVeryVerboseFormat(string format, params object[] args)
		{
			Common.Debug.LogFormat(VERBOSE_PREFIX + format, args);
		}

		/// <summary>
		/// Logs verbose message to the Unity Console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("LOGGING_VERY_VERBOSE")]
		public static void VeryVerbose(object message, UnityEngine.Object context = null)
		{
			Common.Debug.Log(VERBOSE_PREFIX + message, context);
		}
		
		/// <summary>
		/// Logs a formatted verbose message to the Unity Console.
		/// </summary>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_VERY_VERBOSE")]
		public static void VeryVerboseFormat(UnityEngine.Object context, string format, params object[] args)
		{
			Common.Debug.LogFormat(context, VERBOSE_PREFIX + format, args);
		}
		
		/// <summary>
		/// Logs a formatted verbose message to the Unity Console.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_VERY_VERBOSE")]
		public static void VeryVerboseFormat(string format, params object[] args)
		{
			Common.Debug.LogFormat(VERBOSE_PREFIX + format, args);
		}

        /// <summary>
        /// Logs verbose message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_VERBOSE")]
        public static void Verbose(object message, UnityEngine.Object context = null)
        {
			Common.Debug.Log(VERBOSE_PREFIX + message, context);
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
			Common.Debug.LogFormat(context, VERBOSE_PREFIX + format, args);
        }

        /// <summary>
        /// Logs a formatted verbose message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_VERBOSE")]
        public static void VerboseFormat(string format, params object[] args)
        {
			Common.Debug.LogFormat(VERBOSE_PREFIX + format, args);
        }

        /// <summary>
        /// Logs debug message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_DEBUG")]
        public static void Debug(object message, UnityEngine.Object context = null)
        {
			Common.Debug.Log(DEBUG_PREFIX + message, context);
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
			Common.Debug.LogFormat(context, DEBUG_PREFIX + format, args);
        }

        /// <summary>
        /// Logs a formatted debug message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_DEBUG")]
        public static void DebugFormat(string format, params object[] args)
        {
			Common.Debug.LogFormat(DEBUG_PREFIX + format, args);
        }

        /// <summary>
        /// Logs info message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_INFO")]
        public static void Info(object message, UnityEngine.Object context = null)
        {
			Common.Debug.Log(INFO_PREFIX + message, context);
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
			Common.Debug.LogFormat(context, INFO_PREFIX + format, args);
        }

        /// <summary>
        /// Logs a formatted info message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_INFO")]
        public static void InfoFormat(string format, params object[] args)
        {
			Common.Debug.LogFormat(INFO_PREFIX + format, args);
        }

        /// <summary>
        /// Logs warning message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_WARNING")]
        public static void Warning(object message, UnityEngine.Object context = null)
        {
			Common.Debug.LogWarning(WARNING_PREFIX + message, context);
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
			Common.Debug.LogWarningFormat(context, WARNING_PREFIX + format, args);
        }

        /// <summary>
        /// Logs a formatted warning message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_WARNING")]
        public static void WarningFormat(string format, params object[] args)
        {
			Common.Debug.LogWarningFormat(WARNING_PREFIX + format, args);
        }

        /// <summary>
        /// Logs error message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_ERROR")]
        public static void Error(object message, UnityEngine.Object context = null)
        {
			Common.Debug.LogError(ERROR_PREFIX + message, context);
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
			Common.Debug.LogErrorFormat(context, ERROR_PREFIX + format, args);
        }

        /// <summary>
        /// Logs a formatted error message to the Unity Console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        [Conditional("LOGGING_ERROR")]
        public static void ErrorFormat(string format, params object[] args)
        {
			Common.Debug.LogErrorFormat(ERROR_PREFIX + format, args);
        }

		/// <summary>
		/// Logs fatal message to the Unity Console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("LOGGING_FATAL")]
		public static void Fatal(object message, UnityEngine.Object context = null)
		{
			Common.Debug.LogError(FATAL_PREFIX + message, context);
		}
		
		/// <summary>
		/// Logs a formatted fatal message to the Unity Console.
		/// </summary>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_FATAL")]
		public static void FatalFormat(UnityEngine.Object context, string format, params object[] args)
		{
			Common.Debug.LogErrorFormat(context, FATAL_PREFIX + format, args);
		}
		
		/// <summary>
		/// Logs a formatted fatal message to the Unity Console.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_FATAL")]
		public static void FatalFormat(string format, params object[] args)
		{
			Common.Debug.LogErrorFormat(FATAL_PREFIX + format, args);
		}

		/// <summary>
		/// Logs user interaction to the Unity Console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		[Conditional("LOGGING_UI")]
		public static void UserInteraction(object message, UnityEngine.Object context = null)
		{
			Common.Debug.Log(USER_INTERACTION_PREFIX + message, context);
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
			Common.Debug.LogFormat(context, USER_INTERACTION_PREFIX + format, args);
		}
		
		/// <summary>
		/// Logs a formatted user interaction to the Unity Console.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">Format arguments.</param>
		[Conditional("LOGGING_UI")]
		public static void UserInteractionFormat(string format, params object[] args)
		{
			Common.Debug.LogFormat(USER_INTERACTION_PREFIX + format, args);
		}
    }
}
