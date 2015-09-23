using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Common.UI.Listeners;



namespace Common.UI.Popups
{
    /// <summary>
    /// Script that realize behaviour for PopupMenus controller.
    /// </summary>
    public class PopupMenuAreaScript : MonoBehaviour, IEscapeButtonHandler
    {
        private const float TIMER_NOT_ACTIVE = -10000f;



        /// <summary>
        /// Gets the instance geometry.
        /// </summary>
        /// <value>Instance geometry.</value>
        public static Transform geometry
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("PopupMenuAreaScript.geometry = {0}", sInstance.transform);

                return sInstance.transform;
            }
        }



        private static PopupMenuAreaScript sInstance = null;



        private List<PopupMenu>     mPopupMenus;
        private AutoPopupItemScript mAutoPopupItem;
        private float               mDelay;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
            DebugEx.Verbose("PopupMenuAreaScript.Start()");

            if (sInstance == null)
            {
                sInstance = this;
            }
            else
            {
                DebugEx.Error("Two instances of PopupMenuAreaScript not supported");
            }

            mPopupMenus    = new List<PopupMenu>();
            mAutoPopupItem = null;
            mDelay         = TIMER_NOT_ACTIVE;

            enabled = false;
        }

        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        void OnDestroy()
        {
            DebugEx.Verbose("PopupMenuAreaScript.OnDestroy()");

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
            DebugEx.VeryVeryVerbose("PopupMenuAreaScript.Update()");

            if (InputControl.GetMouseButtonDown(MouseButton.Left))
            {
                List<RaycastResult> hits = new List<RaycastResult>();
                Mouse.RaycastAll(hits);

                bool hitPopupMenu = false;

                if (hits.Count > 0)
                {
                    Transform curTransform = hits[0].gameObject.transform;

                    while (curTransform != null)
                    {
                        if (curTransform == transform)
                        {
                            hitPopupMenu = true;
                            break;
                        }

                        curTransform = curTransform.parent;
                    }
                }

                if (!hitPopupMenu)
                {
                    mPopupMenus[0].Destroy();
                }
            }

            if (IsTimerActive())
            {
                mDelay -= Time.deltaTime;

                if (mDelay <= 0f)
                {
                    mAutoPopupItem.Click();
                    StopTimer();
                }
            }
        }

        /// <summary>
        /// Handles escape button press event.
        /// </summary>
        /// <returns><c>true</c>, if escape button was handled, <c>false</c> otherwise.</returns>
        public bool OnEscapeButtonPressed()
        {
            DebugEx.UserInteraction("PopupMenuAreaScript.OnEscapeButtonPressed()");

            mPopupMenus[mPopupMenus.Count - 1].Destroy();

            return true;
        }

        /// <summary>
        /// Handler for auto popup item destroy event.
        /// </summary>
        /// <param name="item">Popup menu item.</param>
        public static void OnAutoPopupItemDestroy(AutoPopupItemScript item)
        {
            DebugEx.VerboseFormat("PopupMenuAreaScript.OnAutoPopupItemDestroy(item = {0})", item);

            if (sInstance != null)
            {
                if (sInstance.mPopupMenus.Count > 0)
                {
                    if (sInstance.mAutoPopupItem == item)
                    {
                        sInstance.mAutoPopupItem = null;
                        sInstance.StopTimer();
                    }
                }
            }
        }

        /// <summary>
        /// Handler for auto popup item disable event.
        /// </summary>
        /// <param name="item">Popup menu item.</param>
        public static void OnAutoPopupItemDisable(AutoPopupItemScript item)
        {
            DebugEx.VerboseFormat("PopupMenuAreaScript.OnAutoPopupItemDisable(item = {0})", item);

            if (sInstance != null)
            {
                if (sInstance.mPopupMenus.Count > 0)
                {
                    if (sInstance.mAutoPopupItem == item)
                    {
                        sInstance.mAutoPopupItem = null;
                        sInstance.StopTimer();
                    }
                }
            }
            else
            {
                DebugEx.Error("There is no PopupMenuAreaScript instance");
            }
        }

        /// <summary>
        /// Handler for auto popup item enter event.
        /// </summary>
        /// <param name="item">Popup menu item.</param>
        public static void OnAutoPopupItemEnter(AutoPopupItemScript item)
        {
            DebugEx.VerboseFormat("PopupMenuAreaScript.OnAutoPopupItemEnter(item = {0})", item);

            if (sInstance != null)
            {
                if (sInstance.mPopupMenus.Count > 0)
                {
                    sInstance.mAutoPopupItem = item;
                    sInstance.StartTimer(sInstance.mAutoPopupItem.delay);
                }
            }
            else
            {
                DebugEx.Error("There is no PopupMenuAreaScript instance");
            }
        }

        /// <summary>
        /// Handler for auto popup item exit event.
        /// </summary>
        /// <param name="item">Popup menu item.</param>
        public static void OnAutoPopupItemExit(AutoPopupItemScript item)
        {
            DebugEx.VerboseFormat("PopupMenuAreaScript.OnAutoPopupItemExit(item = {0})", item);

            if (sInstance != null)
            {
                if (sInstance.mPopupMenus.Count > 0)
                {
                    sInstance.mAutoPopupItem = null;
                    sInstance.StopTimer();
                }
            }
            else
            {
                DebugEx.Error("There is no PopupMenuAreaScript instance");
            }
        }

        /// <summary>
        /// Registers specified popup menu.
        /// </summary>
        /// <param name="menu">Popup menu.</param>
        public static void RegisterPopupMenu(PopupMenu menu)
        {
            DebugEx.VerboseFormat("PopupMenuAreaScript.RegisterPopupMenu(menu = {0})", menu);

            if (sInstance != null)
            {
                sInstance.mPopupMenus.Add(menu);
                sInstance.enabled = true;

                EscapeButtonListenerScript.PushHandlerToTop(sInstance);
            }
            else
            {
                DebugEx.Error("There is no PopupMenuAreaScript instance");
            }
        }

        /// <summary>
        /// Deregisters specified popup menu.
        /// </summary>
        /// <param name="menu">Popup menu.</param>
        public static void DeregisterPopupMenu(PopupMenu menu)
        {
            DebugEx.VerboseFormat("PopupMenuAreaScript.DeregisterPopupMenu(menu = {0})", menu);

            if (sInstance != null)
            {
                if (sInstance.mPopupMenus.Remove(menu))
                {
                    if (sInstance.mPopupMenus.Count == 0)
                    {
                        sInstance.enabled = false;
                        sInstance.mAutoPopupItem = null;
                        sInstance.StopTimer();

                        EscapeButtonListenerScript.RemoveHandler(sInstance);
                    }
                }
                else
                {
                    DebugEx.Error("Failed to deregister popup menu");
                }
            }
            else
            {
                DebugEx.Error("There is no PopupMenuAreaScript instance");
            }
        }

        /// <summary>
        /// This method will destroy all popup menus.
        /// </summary>
        public static void DestroyAll()
        {
            DebugEx.Verbose("PopupMenuAreaScript.DestroyAll()");

            if (sInstance != null)
            {
                if (sInstance.mPopupMenus.Count > 0)
                {
                    sInstance.mPopupMenus[0].Destroy();
                }
            }
            else
            {
                DebugEx.Error("There is no PopupMenuAreaScript instance");
            }
        }

        /// <summary>
        /// Starts timer with specified delay.
        /// </summary>
        /// <param name="ms">Delay in ms.</param>
        private void StartTimer(float ms)
        {
            DebugEx.VerboseFormat("PopupMenuAreaScript.StartTimer(ms = {0})", ms);

            if (ms < 0f)
            {
                DebugEx.ErrorFormat("Incorrect delay value: {0}", ms);
            }

            mDelay = ms / 1000f;
        }

        /// <summary>
        /// Stops timer.
        /// </summary>
        private void StopTimer()
        {
            DebugEx.Verbose("PopupMenuAreaScript.StopTimer()");

            mDelay = TIMER_NOT_ACTIVE;
        }

        /// <summary>
        /// Determines whether timer is active.
        /// </summary>
        /// <returns><c>true</c> if timer is active; otherwise, <c>false</c>.</returns>
        private bool IsTimerActive()
        {
            DebugEx.Verbose("PopupMenuAreaScript.IsTimerActive()");

            return mDelay != TIMER_NOT_ACTIVE;
        }
    }
}
