using UnityEngine;



namespace Common.UI.DockWidgets
{
    /// <summary>
    /// Dummy dock widget.
    /// </summary>
    public class DummyDockWidgetScript : DockWidgetScript
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DummyDockWidgetScript instance
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DummyDockWidgetScript.instance = {0}", sInstance);

                return sInstance;
            }
        }



        private static DummyDockWidgetScript sInstance = null;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.DockWidgets.DummyDockWidgetScript"/> class.
        /// </summary>
        private DummyDockWidgetScript()
        {
            DebugEx.Verbose("Created DummyDockWidgetScript object");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.DockWidgets.DummyDockWidgetScript"/> class.
        /// </summary>
        /// <param name="baseScript">Base script.</param>
        public static DummyDockWidgetScript Create(DockWidgetScript baseScript)
        {
            DebugEx.VerboseFormat("DummyDockWidgetScript.Create(baseScript = {0})", baseScript);

            DestroyInstance();

            //***************************************************************************
            // Dummy GameObject
            //***************************************************************************
            #region Dummy GameObject
            GameObject dummy = new GameObject("Dummy");
            Utils.InitUIObject(dummy, Global.dockingAreaScript.transform);

            //===========================================================================
            // DummyDockWidgetScript Component
            //===========================================================================
            #region DummyDockWidgetScript Component
            sInstance = dummy.AddComponent<DummyDockWidgetScript>();

            sInstance.image           = baseScript.image;
            sInstance.tokenId         = baseScript.tokenId;
            sInstance.backgroundColor = Assets.Common.DockWidgets.Colors.dummyBackground;
            #endregion
            #endregion

            return sInstance;
        }

        /// <summary>
        /// Destroies the instance.
        /// </summary>
        public static void DestroyInstance()
        {
            DebugEx.Verbose("DummyDockWidgetScript.DestroyInstance()");

            if (sInstance != null)
            {
                sInstance.Destroy();
                sInstance = null;
            }
        }

        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        void OnDestroy()
        {
            DebugEx.Verbose("DummyDockWidgetScript.OnDestroy()");

            if (sInstance == this)
            {
                sInstance = null;
            }
        }
    }
}
