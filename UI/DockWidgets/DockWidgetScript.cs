using UnityEngine;
using UnityEngine.UI;
using UnityTranslation;



namespace Common.UI.DockWidgets
{
    /// <summary>
    /// Script that realize dock widget behaviour.
    /// </summary>
    public class DockWidgetScript : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets parent docking group.
        /// </summary>
        /// <value>Parent docking group.</value>
        public DockingGroupScript parent
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DockWidgetScript.parent = {0}", mParent);

                return mParent;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DockWidgetScript.parent: {0} => {1}", mParent, value);

                mParent = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        /// <value>Background color.</value>
        public Color backgroundColor
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DockWidgetScript.backgroundColor = {0}", mBackgroundColor);

                return mBackgroundColor;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DockWidgetScript.backgroundColor: {0} => {1}", mBackgroundColor, value);

                if (mBackgroundColor != value)
                {
                    mBackgroundColor = value;

                    if (IsUICreated())
                    {
                        mContentBackgroundImage.color = mBackgroundColor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public Sprite image
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DockWidgetScript.image = {0}", mImage);

                return mImage;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DockWidgetScript.image: {0} => {1}", mImage, value);

                if (mImage != value)
                {
                    mImage = value;

                    if (mParent != null)
                    {
                        mParent.UpdateTabImage(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets token ID for translation.
        /// </summary>
        /// <value>Token ID for translation.</value>
        public R.sections.DockWidgets.strings tokenId
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DockWidgetScript.tokenId = {0}", mTokenId);

                return mTokenId;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DockWidgetScript.tokenId: {0} => {1}", mTokenId, value);

                if (mTokenId != value)
                {
                    mTokenId = value;

                    if (mParent != null)
                    {
                        mParent.UpdateTab(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string title
        {
            get
            {
                string res;

                if (mTokenId == R.sections.DockWidgets.strings.Count)
                {
                    res = "";
                }
                else
                {
                    res = Translator.GetString(mTokenId);
                }

                DebugEx.VeryVeryVerboseFormat("DockWidgetScript.title = {0}", res);

                return res;
            }
        }



        private DockingGroupScript             mParent;
        private Color                          mBackgroundColor;
        private Sprite                         mImage;
        private R.sections.DockWidgets.strings mTokenId;

        private RectTransform mContentTransform;
        private Image         mContentBackgroundImage;



        public DockWidgetScript()
            : base()
        {
            DebugEx.Verbose("Created DockWidgetScript object");

            mParent          = null;
            mBackgroundColor = Assets.Common.DockWidgets.Colors.background;
            mImage           = Assets.Common.DockWidgets.Textures.icon.sprite;
            mTokenId         = R.sections.DockWidgets.strings.Count;

            mContentTransform       = null;
            mContentBackgroundImage = null;
        }

        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
            DebugEx.Verbose("DockWidgetScript.Start()");

            CreateUI();
        }

        /// <summary>
        /// Creates user interface.
        /// </summary>
        private void CreateUI()
        {
            DebugEx.Verbose("DockWidgetScript.CreateUI()");

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            mContentTransform = gameObject.AddComponent<RectTransform>();
            Utils.AlignRectTransformStretchStretch(mContentTransform);
            #endregion

            //===========================================================================
            // CanvasRenderer Component
            //===========================================================================
            #region CanvasRenderer Component
            gameObject.AddComponent<CanvasRenderer>();
            #endregion

            //===========================================================================
            // Image Component
            //===========================================================================
            #region Image Component
            mContentBackgroundImage = gameObject.AddComponent<Image>();
            mContentBackgroundImage.color = mBackgroundColor;
            #endregion

            //===========================================================================
            // Mask Component
            //===========================================================================
            #region Mask Component
            gameObject.AddComponent<Mask>();
            #endregion

            CreateContent(mContentTransform);
        }

        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="contentTransform">Content transform.</param>
        protected virtual void CreateContent(Transform contentTransform)
        {
            DebugEx.VerboseFormat("DockWidgetScript.CreateContent(contentTransform = {0})", contentTransform);

            DebugEx.Fatal("Unexpected behaviour in DockWidgetScript.CreateContent()");
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            DebugEx.Verbose("DockWidgetScript.Destroy()");

            UnityEngine.Object.DestroyObject(gameObject);

            if (mParent != null)
            {
                mParent.RemoveDockWidget(this);
            }
        }

        /// <summary>
        /// Handler for resize event.
        /// </summary>
        public virtual void OnResize()
        {
            DebugEx.Verbose("DockWidgetScript.OnResize()");
        }

        /// <summary>
        /// Insert dock widget into specified docking area.
        /// </summary>
        /// <param name="dockingArea">Docking area.</param>
        /// <param name="orientation">Orientation.</param>
        /// <param name="index">Index.</param>
        public void InsertToDockingArea(DockingAreaScript dockingArea, DockingAreaOrientation orientation = DockingAreaOrientation.Horizontal, int index = 0)
        {
            DebugEx.VerboseFormat("DockWidgetScript.InsertToDockingArea(dockingArea = {0}, orientation = {1}, index = {2})", dockingArea, orientation, index);

            dockingArea.InsertDockWidget(this, orientation, index);
        }

        /// <summary>
        /// Insert dock widget into specified docking area.
        /// </summary>
        /// <param name="dockingArea">Docking area.</param>
        /// <param name="orientation">Orientation.</param>
        /// <param name="index">Index.</param>
        public void InsertToDockingGroup(DockingGroupScript dockingGroup, int index = 0)
        {
            DebugEx.VerboseFormat("DockWidgetScript.InsertToDockingGroup(dockingGroup = {0}, index = {1})", dockingGroup, index);

            dockingGroup.InsertDockWidget(this, index);
        }

        /// <summary>
        /// Selects this dock widget.
        /// </summary>
        public void Select()
        {
            DebugEx.Verbose("DockWidgetScript.Select()");

            mParent.OnSelectTab(this);
        }

        /// <summary>
        /// Determines whether user interface created or not.
        /// </summary>
        /// <returns><c>true</c> if user interface created; otherwise, <c>false</c>.</returns>
        private bool IsUICreated()
        {
            DebugEx.Verbose("DockWidgetScript.IsUICreated()");

            return (mContentTransform != null);
        }
    }
}
