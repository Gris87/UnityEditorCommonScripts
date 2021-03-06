using System.Collections.Generic;
using UnityEngine;
using UnityTranslation;



namespace Common.UI.Toasts
{
    /// <summary>
    /// Class for displaying text notifications.
    /// </summary>
    public static class Toast
    {
        /// <summary>
        /// Show text notification for a long period of time.
        /// </summary>
        public const float LENGTH_LONG  = 3000f;

        /// <summary>
        /// Show text notification for a short period of time.
        /// </summary>
        public const float LENGTH_SHORT = 1000f;



        private static List<ToastScript> sToasts;



        /// <summary>
        /// Initializes the <see cref="Common.UI.Toasts.Toast"/> class.
        /// </summary>
        static Toast()
        {
            DebugEx.Verbose("Static class Toast initialized");

            sToasts = new List<ToastScript>();
        }

        /// <summary>
        /// Show text notification with specified duration.
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        /// <param name="message">Message text.</param>
        /// <param name="duration">Duration.</param>
        public static void Show(Transform parent, string message, float duration)
        {
            DebugEx.VerboseFormat("Toast.Show(parent = {0}, message = {1}, duration = {2})", parent, message, duration);

            AddToast(parent, message, R.sections.Toasts.strings.Count, null, duration);
        }

        /// <summary>
        /// Show text notification with specified duration.
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        /// <param name="tokenId">Token ID for translation.</param>
        /// <param name="duration">Duration.</param>
        public static void Show(Transform parent, R.sections.Toasts.strings tokenId, float duration)
        {
            DebugEx.VerboseFormat("Toast.Show(parent = {0}, tokenId = {1}, duration = {2})", parent, tokenId, duration);

            AddToast(parent, null, tokenId, null, duration);
        }

        /// <summary>
        /// Show text notification with specified duration.
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        /// <param name="tokenId">Token ID for translation.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="tokenArguments">Token arguments.</param>
        public static void Show(Transform parent, R.sections.Toasts.strings tokenId, float duration, params object[] tokenArguments)
        {
            DebugEx.VerboseFormat("Toast.Show(parent = {0}, tokenId = {1}, duration = {2}, tokenArguments = {3})", parent, tokenId, duration, tokenArguments);

            AddToast(parent, null, tokenId, tokenArguments, duration);
        }

        /// <summary>
        /// Adds toast to the list.
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        /// <param name="text">Text.</param>
        /// <param name="tokenId">Token ID for translation.</param>
        /// <param name="tokenArguments">Token arguments.</param>
        /// <param name="duration">Duration.</param>
        private static void AddToast(
                                       Transform                 parent
                                     , string                    text
                                     , R.sections.Toasts.strings tokenId
                                     , object[]                  tokenArguments
                                     , float                     duration
                                    )
        {
            DebugEx.VerboseFormat("Toast.AddToast(parent = {0}, text = {1}, tokenId = {2}, tokenArguments = {3}, duration = {4})"
                                  , parent
                                  , text
                                  , tokenId
                                  , tokenArguments
                                  , duration);

            #region Toast GameObject
            GameObject toast = new GameObject("Toast");
            Utils.InitUIObject(toast, parent);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            RectTransform toastTransform = toast.AddComponent<RectTransform>();
            Utils.AlignRectTransformStretchStretch(toastTransform, 20f, 20f, 20f, 20f);
            #endregion

            //===========================================================================
            // ToastScript Component
            //===========================================================================
            #region ToastScript Component
            ToastScript toastScript = toast.AddComponent<ToastScript>();

            toastScript.text           = text;
            toastScript.tokenId        = tokenId;
            toastScript.tokenArguments = tokenArguments;
            toastScript.duration       = duration;
            #endregion
            #endregion

            sToasts.Add(toastScript);

            if (sToasts.Count == 1)
            {
                ShowNextToast();
            }
        }

        /// <summary>
        /// Displays first toast from the list.
        /// </summary>
        private static void ShowNextToast()
        {
            DebugEx.Verbose("Toast.ShowNextToast()");

            sToasts[0].Show();
        }

        /// <summary>
        /// Handler for toast destroy event.
        /// </summary>
        /// <param name="toast">Toast script.</param>
        public static void ToastDestroyed(ToastScript toast)
        {
            DebugEx.VerboseFormat("Toast.ToastDestroyed(toast = {0})", toast);

            int index = sToasts.IndexOf(toast);

            if (index >= 0)
            {
                sToasts.RemoveAt(index);

                if (index == 0 && sToasts.Count > 0)
                {
                    ShowNextToast();
                }
            }
            else
            {
                DebugEx.Fatal("Unexpected behaviour in Toast.ToastDestroyed()");
            }
        }
    }
}
