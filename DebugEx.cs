using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;



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



		private static string       sFileName;
		private static StreamWriter sFileWriter;



#if LOGGING_TO_FILE
		static DebugEx()
		{
			// TODO: [Major] Local app dir
			sFileName = Application.persistentDataPath + "/" + Application.productName + ".log";
			Common.Debug.Log("Log file: " + sFileName);

			try
            {
				sFileWriter = new StreamWriter(sFileName, false, Encoding.UTF8);
			}
			catch(IOException e)
			{
				Common.Debug.LogError("Impossible to create file: " + sFileName);
                Common.Debug.LogError(e);
            }
		}
#endif
        
        
        
        [Conditional("LOGGING_TO_FILE")]
		private static void WriteToFile(string prefix, object message)
		{
			if (sFileWriter != null)
			{
				try
				{
					sFileWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff: ") + prefix + message);
					sFileWriter.Flush();
				}
				catch(IOException e)
				{
					Common.Debug.LogError("Impossible to write to file: " + sFileName);
                    Common.Debug.LogError(e);
				}
			}
		}

		[Conditional("LOGGING_TO_FILE")]
		private static void WriteToFileFormat(string prefix, string format, params object[] args)
		{
			WriteToFile(prefix, string.Format(format, args));
		}

        /// <summary>
        /// Logs verbose message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("LOGGING_VERY_VERY_VERBOSE")]
        public static void VeryVeryVerbose(object message, UnityEngine.Object context = null)
        {
			WriteToFile(VERBOSE_PREFIX, message);

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
			WriteToFileFormat(VERBOSE_PREFIX, format, args);
			
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
			WriteToFileFormat(VERBOSE_PREFIX, format, args);
			
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
			WriteToFile(VERBOSE_PREFIX, message);
			
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
			WriteToFileFormat(VERBOSE_PREFIX, format, args);
			
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
			WriteToFileFormat(VERBOSE_PREFIX, format, args);
			
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
			WriteToFile(VERBOSE_PREFIX, message);
			
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
			WriteToFileFormat(VERBOSE_PREFIX, format, args);
			
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
			WriteToFileFormat(VERBOSE_PREFIX, format, args);
			
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
			WriteToFile(DEBUG_PREFIX, message);
			
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
			WriteToFileFormat(DEBUG_PREFIX, format, args);
			
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
			WriteToFileFormat(DEBUG_PREFIX, format, args);
			
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
			WriteToFile(INFO_PREFIX, message);
			
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
			WriteToFileFormat(INFO_PREFIX, format, args);
			
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
			WriteToFileFormat(INFO_PREFIX, format, args);
			
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
			WriteToFile(WARNING_PREFIX, message);
			
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
			WriteToFileFormat(WARNING_PREFIX, format, args);
			
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
			WriteToFileFormat(WARNING_PREFIX, format, args);
			
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
			WriteToFile(ERROR_PREFIX, message);
			
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
			WriteToFileFormat(ERROR_PREFIX, format, args);
			
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
			WriteToFileFormat(ERROR_PREFIX, format, args);
			
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
			WriteToFile(FATAL_PREFIX, message);
			
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
			WriteToFileFormat(FATAL_PREFIX, format, args);
			
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
			WriteToFileFormat(FATAL_PREFIX, format, args);
			
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
			WriteToFile(USER_INTERACTION_PREFIX, message);
			
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
			WriteToFileFormat(USER_INTERACTION_PREFIX, format, args);
			
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
			WriteToFileFormat(USER_INTERACTION_PREFIX, format, args);
			
			Common.Debug.LogFormat(USER_INTERACTION_PREFIX + format, args);
        }
    }
}
