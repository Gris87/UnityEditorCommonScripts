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
		private Timer               mTimer;



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
			mTimer         = new Timer(OnAutoPopupTimeout);

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

			mTimer.Update();
        }

		/// <summary>
		/// Handler for auto popup timeout event.
		/// </summary>
		private void OnAutoPopupTimeout()
		{
			DebugEx.Verbose("PopupMenuAreaScript.OnAutoPopupTimeout()");

			mAutoPopupItem.Click();

			mTimer.Stop();
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
                        sInstance.mTimer.Stop();
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
                        sInstance.mTimer.Stop();
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
                    sInstance.mTimer.Start(item.delay);
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
                    sInstance.mTimer.Stop();
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
                        sInstance.mTimer.Stop();

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
    }
}
