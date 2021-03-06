using UnityEngine;
using UnityTranslation;

using Common.UI.Toasts;



namespace Common.App
{
    /// <summary>
    /// Class for Unity Editor usefull functions.
    /// </summary>
    public static class AppUtils
    {
        /// <summary>
        /// Returns version info.
        /// </summary>
        public static string GetVersionString()
        {
            DebugEx.Verbose("AppUtils.GetVersionString()");

            string res = Version.BUILD + " ";

            if (Version.POSTFIX != "")
            {
                res += Version.POSTFIX + " ";
            }

            switch (Version.buildType)
            {
                case Version.BuildType.Personal:
                {
                    res += Translator.GetString(R.sections.Version.strings.personal);
                }
                break;

                case Version.BuildType.Professional:
                {
                    res += Translator.GetString(R.sections.Version.strings.professional);
                }
                break;

                default:
                {
                    DebugEx.WarningFormat("Unknown localization for build type \"{0}\"", Version.buildType);
                    res += Version.buildType.ToString();
                }
                break;
            }

            return res;
        }

        /// <summary>
        /// Show text notification with the contribute message.
        /// </summary>
        public static void ShowContributeMessage()
        {
            DebugEx.Verbose("AppUtils.ShowContributeMessage()");

            ShowContributeMessage(Global.dockingAreaScript.transform);
        }

        /// <summary>
        /// Show text notification with the contribute message.
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        public static void ShowContributeMessage(Transform parent)
        {
            DebugEx.VerboseFormat("AppUtils.ShowContributeMessage(parent = {0})", parent);

            Toast.Show(parent, R.sections.Toasts.strings.contribute, Toast.LENGTH_LONG, CommonConstants.SOURCE_CODE_URL);
        }
    }
}
