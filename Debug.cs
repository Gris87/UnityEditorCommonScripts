using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Internal;



/// <summary>
/// Static class for debugging.
/// </summary>
public static class Debug
{
	/// <summary>
	/// Gets the timestamp.
	/// </summary>
	/// <returns>String representation of timestamp.</returns>
	private static string GetTimestamp()
	{
		return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff: ");
	}

	/// <summary>
	/// Logs message to the Unity Console.
	/// </summary>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public static void Log(object message, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.Log(GetTimestamp() + message, context);
	}

	/// <summary>
	/// Logs a formatted message to the Unity Console.
	/// </summary>
	/// <param name="context">Object to which the message applies.</param>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.Log(GetTimestamp() + string.Format(format, args), context);
	}

	/// <summary>
	/// Logs a formatted message to the Unity Console.
	/// </summary>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public static void LogFormat(string format, params object[] args)
	{
		UnityEngine.Debug.Log(GetTimestamp() + string.Format(format, args));
	}

	/// <summary>
	/// A variant of Debug.Log that logs a warning message to the console.
	/// </summary>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public static void LogWarning(object message, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogWarning(GetTimestamp() + message, context);
	}

	/// <summary>
	/// Logs a formatted warning message to the Unity Console.
	/// </summary>
	/// <param name="context">Object to which the message applies.</param>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogWarning(GetTimestamp() + string.Format(format, args), context);
	}

	/// <summary>
	/// Logs a formatted warning message to the Unity Console.
	/// </summary>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarning(GetTimestamp() + string.Format(format, args));
	}

	/// <summary>
	/// A variant of Debug.Log that logs a error message to the console.
	/// </summary>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public static void LogError(object message, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogError(GetTimestamp() + message, context);
	}

	/// <summary>
	/// Logs a formatted error message to the Unity Console.
	/// </summary>
	/// <param name="context">Object to which the message applies.</param>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogError(GetTimestamp() + string.Format(format, args), context);
	}

	/// <summary>
	/// Logs a formatted error message to the Unity Console.
	/// </summary>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public static void LogErrorFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogError(GetTimestamp() + string.Format(format, args));
	}


	
	// =============================================================
	// Keep the original functionality from UnityEngine.Debug class
	// =============================================================



	/// <summary>
	/// Gets or sets a value indicating whether developer console visible.
	/// </summary>
	/// <value><c>true</c> if developer console visible; otherwise, <c>false</c>.</value>
	public static bool developerConsoleVisible
	{
		get { return UnityEngine.Debug.developerConsoleVisible;  }
		set { UnityEngine.Debug.developerConsoleVisible = value; }
	}

	/// <summary>
	/// Gets a value indicating whether this is debug build.
	/// </summary>
	/// <value><c>true</c> if debug build; otherwise, <c>false</c>.</value>
	public static bool isDebugBuild
	{
		get { return UnityEngine.Debug.isDebugBuild; }
	}

	/// <summary>
	/// Assert the condition.
	/// </summary>
	/// <param name="condition">Condition you expect to be true.</param>
	/// <param name="format">Formatted string for display.</param>
	/// <param name="args">Arguments for the formatted string.</param>
	[Conditional ("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, string format, params object[] args)
	{
		UnityEngine.Debug.Assert(condition, format, args);
	}

	/// <summary>
	/// Assert the condition.
	/// </summary>
	/// <param name="condition">Condition you expect to be true.</param>
	[Conditional ("UNITY_ASSERTIONS")]
	public static void Assert(bool condition)
	{
		UnityEngine.Debug.Assert(condition);
	}

	/// <summary>
	/// Assert the condition.
	/// </summary>
	/// <param name="condition">Condition you expect to be true.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	[Conditional ("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, string message)
	{
		UnityEngine.Debug.Assert(condition, message);
	}

	/// <summary>
	/// Pauses the editor.
	/// </summary>
	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	/// <summary>
	/// Clears errors from the developer console.
	/// </summary>
	public static void ClearDeveloperConsole()
	{
		UnityEngine.Debug.ClearDeveloperConsole();
	}

	/// <summary>
	/// Pauses the editor.
	/// </summary>
	public static void DebugBreak()
	{
		UnityEngine.Debug.DebugBreak();
	}

	/// <summary>
	/// Draws a line from the point start to end with color.
	/// </summary>
	/// <param name="start">Point in world space where the line should start.</param>
	/// <param name="end">Point in world space where the line should end.</param>
	/// <param name="color">Color of the line.</param>
	/// <param name="duration">How long the line should be visible for.</param>
	/// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
	public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue ("Color.white")] Color color, [DefaultValue ("0.0f")] float duration, [DefaultValue ("true")] bool depthTest)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	/// <summary>
	/// Draws a line from the point start to end with color.
	/// </summary>
	/// <param name="start">Point in world space where the line should start.</param>
	/// <param name="end">Point in world space where the line should end.</param>
	/// <param name="color">Color of the line.</param>
	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		UnityEngine.Debug.DrawLine(start, end, color);
	}

	/// <summary>
	/// Draws a line from the point start to end with color.
	/// </summary>
	/// <param name="start">Point in world space where the line should start.</param>
	/// <param name="end">Point in world space where the line should end.</param>
	public static void DrawLine(Vector3 start, Vector3 end)
	{
		UnityEngine.Debug.DrawLine(start, end);
	}

	/// <summary>
	/// Draws a line from the point start to end with color.
	/// </summary>
	/// <param name="start">Point in world space where the line should start.</param>
	/// <param name="end">Point in world space where the line should end.</param>
	/// <param name="color">Color of the line.</param>
	/// <param name="duration">How long the line should be visible for.</param>
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration);
	}

	/// <summary>
	/// Draws a line from start to start + dir in world coordinates.
	/// </summary>
	/// <param name="start">Point in world space where the ray should start.</param>
	/// <param name="dir">Direction and length of the ray.</param>
	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		UnityEngine.Debug.DrawRay(start, dir);
	}

	/// <summary>
	/// Draws a line from start to start + dir in world coordinates.
	/// </summary>
	/// <param name="start">Point in world space where the ray should start.</param>
	/// <param name="dir">Direction and length of the ray.</param>
	/// <param name="color">Color of the drawn line.</param>
	/// <param name="duration">How long the line will be visible for (in seconds).</param>
	/// <param name="depthTest">Should the line be obscured by other objects closer to the camera?</param>
	public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue ("Color.white")] Color color, [DefaultValue ("0.0f")] float duration, [DefaultValue ("true")] bool depthTest)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	/// <summary>
	/// Draws a line from start to start + dir in world coordinates.
	/// </summary>
	/// <param name="start">Point in world space where the ray should start.</param>
	/// <param name="dir">Direction and length of the ray.</param>
	/// <param name="color">Color of the drawn line.</param>
	/// <param name="duration">How long the line will be visible for (in seconds).</param>
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration);
	}

	/// <summary>
	/// Draws a line from start to start + dir in world coordinates.
	/// </summary>
	/// <param name="start">Point in world space where the ray should start.</param>
	/// <param name="dir">Direction and length of the ray.</param>
	/// <param name="color">Color of the drawn line.</param>
	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		UnityEngine.Debug.DrawRay(start, dir, color);
	}

	/// <summary>
	/// A variant of Debug.Log that logs an error message to the console.
	/// </summary>
	/// <param name="exception">Exception.</param>
	/// <param name="context">Object to which the message applies.</param>
	public static void LogException(Exception exception, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogException(exception, context);
	}
}
