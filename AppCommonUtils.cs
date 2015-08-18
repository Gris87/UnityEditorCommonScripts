using UnityEngine;
using UnityTranslation;

using Common.UI.Toasts;



namespace Common
{
	/// <summary>
	/// Class for Unity Editor usefull functions.
	/// </summary>
	public static class AppCommonUtils
	{
	    /// <summary>
	    /// Show text notification with the contribute message.
	    /// </summary>
	    public static void ShowContributeMessage()
	    {
	        ShowContributeMessage(Global.dockingAreaScript.transform);
	    }

	    /// <summary>
	    /// Show text notification with the contribute message.
	    /// </summary>
	    /// <param name="parent">Parent transform.</param>
	    public static void ShowContributeMessage(Transform parent)
	    {
	        Toast.Show(parent, R.sections.Toasts.strings.contribute, Toast.LENGTH_LONG, CommonConstants.SOURCE_CODE_URL);
	    }
	}
}
