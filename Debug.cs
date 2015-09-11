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

	public static void Log(object obj, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.Log(GetTimestamp() + obj, context);
	}

	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.Log(GetTimestamp() + string.Format(format, args), context);
	}
	
	public static void LogFormat(string format, params object[] args)
	{
		UnityEngine.Debug.Log(GetTimestamp() + string.Format(format, args));
	}

	public static void LogWarning(object obj, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogWarning(GetTimestamp() + obj, context);
	}
	
	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogWarning(GetTimestamp() + string.Format(format, args), context);
	}
	
	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarning(GetTimestamp() + string.Format(format, args));
	}

	public static void LogError(object obj, UnityEngine.Object context = null)
	{
		UnityEngine.Debug.LogError(GetTimestamp() + obj, context);
	}
	
	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogError(GetTimestamp() + string.Format(format, args), context);
	}
	
	public static void LogErrorFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogError(GetTimestamp() + string.Format(format, args));
	}


	
	// =============================================================
	// Keep the original functionality from UnityEngine.Debug class
	// =============================================================



	public static bool developerConsoleVisible
	{
		get { return UnityEngine.Debug.developerConsoleVisible;  }
		set { UnityEngine.Debug.developerConsoleVisible = value; }
	}
	
	public static bool isDebugBuild
	{
		get { return UnityEngine.Debug.isDebugBuild; }
	}

	[Conditional ("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, string format, params object[] args)
	{
		UnityEngine.Debug.Assert(condition, format, args);
	}
	
	[Conditional ("UNITY_ASSERTIONS")]
	public static void Assert(bool condition)
	{
		UnityEngine.Debug.Assert(condition);
	}
	
	[Conditional ("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, string message)
	{
		UnityEngine.Debug.Assert(condition, message);
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	public static void ClearDeveloperConsole()
	{
		UnityEngine.Debug.ClearDeveloperConsole();
	}

	public static void DebugBreak()
	{
		UnityEngine.Debug.DebugBreak();
	}
	
	public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue ("Color.white")] Color color, [DefaultValue ("0.0f")] float duration, [DefaultValue ("true")] bool depthTest)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		UnityEngine.Debug.DrawLine(start, end, color);
	}

	public static void DrawLine(Vector3 start, Vector3 end)
	{
		UnityEngine.Debug.DrawLine(start, end);
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration);
	}

	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		UnityEngine.Debug.DrawRay(start, dir);
	}
	
	public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue ("Color.white")] Color color, [DefaultValue ("0.0f")] float duration, [DefaultValue ("true")] bool depthTest)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration);
	}

	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		UnityEngine.Debug.DrawRay(start, dir, color);
	}
		
	public static void LogException(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}
	
	public static void LogException(Exception exception, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogException(exception, context);
	}
}
