using System.Collections.Generic;
using UnityEngine;



namespace Common.UI.Listeners
{
    /// <summary>
    /// Script that listen for escape button press events.
    /// </summary>
    public class EscapeButtonListenerScript : MonoBehaviour
    {
        private static EscapeButtonListenerScript sInstance = null;



        private List<IEscapeButtonHandler> mHandlers;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
			DebugEx.Verbose("EscapeButtonListenerScript.Start()");

            if (sInstance == null)
            {
                sInstance = this;
            }
            else
            {
                DebugEx.Error("Two instances of EscapeButtonListener not supported");
            }

            mHandlers = new List<IEscapeButtonHandler>();

            enabled = false;
        }

        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        void OnDestroy()
        {
			DebugEx.Verbose("EscapeButtonListenerScript.OnDestroy()");

            if (sInstance == this)
            {
                sInstance = null;
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
			DebugEx.VeryVeryVerbose("EscapeButtonListenerScript.Update()");

            if (InputControl.GetButtonDown(Controls.buttons.escape, true))
            {
                for (int i = mHandlers.Count - 1; i >= 0; --i)
                {
                    if (mHandlers[i].OnEscapeButtonPressed())
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Push handler to the top.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public static void PushHandlerToTop(IEscapeButtonHandler handler)
        {
			DebugEx.VerboseFormat("EscapeButtonListenerScript.PushHandlerToTop(handler = {0})", handler);

            if (sInstance != null)
            {
                sInstance.mHandlers.Remove(handler);
                sInstance.mHandlers.Add(handler);

                sInstance.enabled = true;
            }
            else
            {
                DebugEx.Error("There is no EscapeButtonListener instance");
            }
        }

        /// <summary>
        /// Removes the handler.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public static void RemoveHandler(IEscapeButtonHandler handler)
        {
			DebugEx.VerboseFormat("EscapeButtonListenerScript.RemoveHandler(handler = {0})", handler);

            if (sInstance != null)
            {
                if (sInstance.mHandlers.Remove(handler))
                {
                    if (sInstance.mHandlers.Count == 0)
                    {
                        sInstance.enabled = false;
                    }
                }
                else
                {
                    DebugEx.Error("Failed to remove handler");
                }
            }
        }
    }
}
