#if !UNITY_EDITOR
#if UNITY_ANDROID
#define CURSORLESS_PLATFORM
#endif
#endif



using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityTranslation;

using Common;
using Common.UI.Listeners;
using Common.UI.Popups;



namespace Common.UI.Windows
{
    /// <summary>
    /// Script that realize behaviour for window.
    /// </summary>
    public class WindowScript : MonoBehaviour, ICanvasRaycastFilter, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Class for data that used while left mouse button pressed.
        /// </summary>
        private class MouseContext
        {
            public float previousMouseX;
            public float previousMouseY;
            public float previousX;
            public float previousY;
            public float previousWidth;
            public float previousHeight;
            public float previousRectX;
            public float previousRectY;



            /// <summary>
            /// Initializes a new instance of the <see cref="Common.UI.Windows.WindowScript+MouseContext"/> class.
            /// </summary>
            /// <param name="mouseX">Previous mouse x.</param>
            /// <param name="mouseY">Previous mouse y.</param>
            /// <param name="x">Previous x coordinate.</param>
            /// <param name="y">Previous y coordinate.</param>
            /// <param name="width">Previous Width.</param>
            /// <param name="height">Previous Height.</param>
            /// <param name="rectX">Previous rectangle x.</param>
            /// <param name="rectY">Previous rectangle y.</param>
            public MouseContext(
                                  float mouseX
                                , float mouseY
                                , float x
                                , float y
                                , float width
                                , float height
                                , float rectX
                                , float rectY
                               )
            {
                DebugEx.Verbose("Created WindowScript.MouseContext object");

                previousMouseX = mouseX;
                previousMouseY = mouseY;
                previousX      = x;
                previousY      = y;
                previousWidth  = width;
                previousHeight = height;
                previousRectX  = rectX;
                previousRectY  = rectY;
            }
        }

        /// <summary>
        /// Mouse location.
        /// </summary>
        private enum MouseLocation
        {
              Outside
            , Header
            , North
            , South
            , West
            , East
            , NorthWest
            , NorthEast
            , SouthWest
            , SouthEast
            , Inside
        }

        /// <summary>
        /// Mouse state.
        /// </summary>
        private enum MouseState
        {
              NoState
            , Dragging
            , Resizing
        }



        private const float SHADOW_WIDTH           = 15f;
        private const float BUTTON_GLOW_WIDTH      = 8f;
        private const float TOOL_BUTTON_GLOW_WIDTH = 4f;
        private const float MAXIMIZED_OFFSET       = 3f;
        private const float RESIZING_GAP           = 8f;
        private const float DRAGGING_GAP           = 15f;

        private const float MINIMAL_WIDTH  = 120f;
        private const float MINIMAL_HEIGHT = 38f;

        private const float MINIMIZED_OFFSET_LEFT   = 8f;
        private const float MINIMIZED_OFFSET_BOTTOM = 8f;
        private const float MINIMIZED_WIDTH         = 120f;
        private const float MINIMIZED_HEIGHT        = 20f;



        /// <summary>
        /// Gets instances.
        /// </summary>
        /// <value>List of all window instances.</value>
        public static ReadOnlyCollection<WindowScript> instances
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.instances = List({0})", sInstances.Count);

                return sInstances.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the selected window.
        /// </summary>
        /// <value>Selected window.</value>
        public static WindowScript selectedWindow
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.selectedWindow = {0}", sSelectedWindow);

                return sSelectedWindow;
            }
        }



        private static List<WindowScript>                           sInstances         = new List<WindowScript>();
        private static WindowScript                                 sSelectedWindow    = null;
        private static List<Pair<WindowScript, List<WindowScript>>> sFullscreenWindows = new List<Pair<WindowScript, List<WindowScript>>>();



        /// <summary>
        /// Gets or sets the window frame.
        /// </summary>
        /// <value>Window frame.</value>
        public WindowFrameType frame
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.frame = {0}", mFrame);

                return mFrame;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.frame: {0} => {1}", mFrame, value);

                if (mFrame != value)
                {
                    WindowFrameType oldValue = mFrame;
                    bool wasFramePresent = IsFramePresent();

                    mFrame = value;

                    if (IsUICreated())
                    {
                        if (wasFramePresent != IsFramePresent())
                        {
                            if (mFrame == WindowFrameType.Frameless)
                            {
                                DestroyBorder();

                                if (mState == WindowState.NoState)
                                {
                                    mWindowTransform.offsetMin = new Vector2(mWindowTransform.offsetMin.x + SHADOW_WIDTH, mWindowTransform.offsetMin.y + SHADOW_WIDTH);
                                    mWindowTransform.offsetMax = new Vector2(mWindowTransform.offsetMax.x - SHADOW_WIDTH, mWindowTransform.offsetMax.y - SHADOW_WIDTH);
                                }
                            }
                            else
                            {
                                CreateBorder();

                                if (mState == WindowState.NoState)
                                {
                                    mWindowTransform.offsetMin = new Vector2(mWindowTransform.offsetMin.x - SHADOW_WIDTH, mWindowTransform.offsetMin.y - SHADOW_WIDTH);
                                    mWindowTransform.offsetMax = new Vector2(mWindowTransform.offsetMax.x + SHADOW_WIDTH, mWindowTransform.offsetMax.y + SHADOW_WIDTH);
                                }
                            }
                        }
                        else
                        {
                            if (wasFramePresent)
                            {
                                UpdateBorderImage();
                                UpdateBorders();

                                if (
                                    (oldValue == WindowFrameType.Drawer)      || (mFrame == WindowFrameType.Drawer)
                                    ||
                                    (oldValue == WindowFrameType.SingleFrame) || (mFrame == WindowFrameType.SingleFrame)
                                   )
                                {
                                    RebuildHeader();
                                }
                            }
                        }

                        mContentTransform.offsetMin = new Vector2(mBorderLeft,    mBorderBottom);
                        mContentTransform.offsetMax = new Vector2(-mBorderRight, -mBorderTop);
                    }

                    if ((oldValue == WindowFrameType.Frameless) || (mFrame == WindowFrameType.Frameless))
                    {
                        if (mFrame == WindowFrameType.Frameless)
                        {
                            mX      += SHADOW_WIDTH;
                            mY      += SHADOW_WIDTH;
                            mWidth  -= SHADOW_WIDTH * 2;
                            mHeight -= SHADOW_WIDTH * 2;
                        }
                        else
                        {
                            mX      -= SHADOW_WIDTH;
                            mY      -= SHADOW_WIDTH;
                            mWidth  += SHADOW_WIDTH * 2;
                            mHeight += SHADOW_WIDTH * 2;
                        }
                    }

                    if (IsUICreated() && mState != WindowState.Minimized)
                    {
                        OnResize();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
        /// <value>Window state.</value>
        public WindowState state
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.state = {0}", mState);

                return mState;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.state: {0} => {1}", mState, value);

                if (mState != value)
                {
                    WindowState oldValue = mState;
                    bool wasFramePresent = IsFramePresent();

                    mState = value;

                    if (IsUICreated())
                    {
                        if (wasFramePresent != IsFramePresent())
                        {
                            if (mState == WindowState.FullScreen)
                            {
                                DestroyBorder();
                            }
                            else
                            {
                                CreateBorder();
                            }

                            mContentTransform.offsetMin = new Vector2(mBorderLeft,    mBorderBottom);
                            mContentTransform.offsetMax = new Vector2(-mBorderRight, -mBorderTop);
                        }
                        else
                        {
                            if (wasFramePresent)
                            {
                                RebuildHeader();
                            }
                        }

                        UpdateState();

                        if (mState != WindowState.Minimized)
                        {
                            OnResize();
                        }
                    }

                    if ((oldValue == WindowState.FullScreen) || (mState == WindowState.FullScreen))
                    {
                        if (mState == WindowState.FullScreen)
                        {
                            AddToFullscreenList();
                        }
                        else
                        {
                            RemoveFromFullscreenList();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the x coordinate.
        /// </summary>
        /// <value>The x coordinate.</value>
        public float x
        {
            get
            {
                float res;

                if (mFrame != WindowFrameType.Frameless)
                {
                    res = mX + SHADOW_WIDTH;
                }
                else
                {
                    res = mX;
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.x = {0}", res);

                return res;
            }

            set
            {
                if (mFrame != WindowFrameType.Frameless)
                {
                    DebugEx.VeryVerboseFormat("WindowScript.x: {0} => {1}", mX + SHADOW_WIDTH, value);

                    value -= SHADOW_WIDTH;
                }
                else
                {
                    DebugEx.VeryVerboseFormat("WindowScript.x: {0} => {1}", mX, value);
                }

                if (mX != value)
                {
                    if (IsUICreated())
                    {
                        if (mState == WindowState.NoState)
                        {
                            mWindowTransform.offsetMin = new Vector2(mWindowTransform.offsetMin.x - mX + value, mWindowTransform.offsetMin.y);
                            mWindowTransform.offsetMax = new Vector2(mWindowTransform.offsetMax.x - mX + value, mWindowTransform.offsetMax.y);
                        }
                    }

                    mX = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the y coordinate.
        /// </summary>
        /// <value>The y coordinate.</value>
        public float y
        {
            get
            {
                float res;

                if (mFrame != WindowFrameType.Frameless)
                {
                    res = mY + SHADOW_WIDTH;
                }
                else
                {
                    res = mY;
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.y = {0}", res);

                return res;
            }

            set
            {
                if (mFrame != WindowFrameType.Frameless)
                {
                    DebugEx.VeryVerboseFormat("WindowScript.y: {0} => {1}", mY + SHADOW_WIDTH, value);

                    value -= SHADOW_WIDTH;
                }
                else
                {
                    DebugEx.VeryVerboseFormat("WindowScript.y: {0} => {1}", mY, value);
                }

                if (mY != value)
                {
                    if (IsUICreated())
                    {
                        if (mState == WindowState.NoState)
                        {
                            mWindowTransform.offsetMin = new Vector2(mWindowTransform.offsetMin.x, mWindowTransform.offsetMin.y + mY - value);
                            mWindowTransform.offsetMax = new Vector2(mWindowTransform.offsetMax.x, mWindowTransform.offsetMax.y + mY - value);
                        }
                    }

                    mY = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>Window width.</value>
        public float width
        {
            get
            {
                float res;

                if (mWidth == 0)
                {
                    res = 0;
                }
                else
                {
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        res = mWidth - SHADOW_WIDTH * 2;
                    }
                    else
                    {
                        res = mWidth;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.width = {0}", res);

                return res;
            }

            set
            {
#if LOGGING_VERY_VERBOSE
                if (mWidth == 0)
                {
                    DebugEx.VeryVerboseFormat("WindowScript.width: 0 => {0}", value);
                }
                else
                {
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        DebugEx.VeryVerboseFormat("WindowScript.width: {0} => {1}", mWidth - SHADOW_WIDTH * 2, value);
                    }
                    else
                    {
                        DebugEx.VeryVerboseFormat("WindowScript.width: {0} => {1}", mWidth, value);
                    }
                }
#endif

                if (value < MINIMAL_WIDTH)
                {
                    value = MINIMAL_WIDTH;
                }

                if (mMinimumWidth != 0 && value < mMinimumWidth)
                {
                    value = mMinimumWidth;
                }

                if (mMaximumWidth != 0 && value > mMaximumWidth)
                {
                    value = mMaximumWidth;
                }

                if (mFrame != WindowFrameType.Frameless)
                {
                    value += SHADOW_WIDTH * 2;
                }

                if (mWidth != value)
                {
                    float oldValue = mWidth;
                    mWidth = value;

                    if (IsUICreated())
                    {
                        if (mState == WindowState.NoState)
                        {
                            mWindowTransform.offsetMax = new Vector2(mWindowTransform.offsetMax.x - oldValue + value, mWindowTransform.offsetMax.y);

                            OnResize();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>Window height.</value>
        public float height
        {
            get
            {
                float res;

                if (mHeight == 0)
                {
                    res = 0;
                }
                else
                {
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        res = mHeight - SHADOW_WIDTH * 2;
                    }
                    else
                    {
                        res = mHeight;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.height = {0}", res);

                return res;
            }

            set
            {
#if LOGGING_VERY_VERBOSE
                if (mHeight == 0)
                {
                    DebugEx.VeryVerboseFormat("WindowScript.height: 0 => {0}", value);
                }
                else
                {
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        DebugEx.VeryVerboseFormat("WindowScript.height: {0} => {1}", mHeight - SHADOW_WIDTH * 2, value);
                    }
                    else
                    {
                        DebugEx.VeryVerboseFormat("WindowScript.height: {0} => {1}", mHeight, value);
                    }
                }
#endif

                if (value < MINIMAL_HEIGHT)
                {
                    value = MINIMAL_HEIGHT;
                }

                if (mMinimumHeight != 0 && value < mMinimumHeight)
                {
                    value = mMinimumHeight;
                }

                if (mMaximumHeight != 0 && value > mMaximumHeight)
                {
                    value = mMaximumHeight;
                }

                if (mFrame != WindowFrameType.Frameless)
                {
                    value += SHADOW_WIDTH * 2;
                }

                if (mHeight != value)
                {
                    float oldValue = mHeight;
                    mHeight = value;

                    if (IsUICreated())
                    {
                        if (mState == WindowState.NoState)
                        {
                            mWindowTransform.offsetMin = new Vector2(mWindowTransform.offsetMin.x, mWindowTransform.offsetMin.y + oldValue - value);

                            OnResize();
                        }
                    }
                }
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
                DebugEx.VeryVeryVerboseFormat("WindowScript.backgroundColor = {0}", mBackgroundColor);

                return mBackgroundColor;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.backgroundColor: {0} => {1}", mBackgroundColor, value);

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
        /// Gets the real x coordinate.
        /// </summary>
        /// <value>Real x coordinate.</value>
        public float realX
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = 0;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = -MAXIMIZED_OFFSET;
                        }
                        else
                        {
                            res = 0;
                        }
                    }
                    else
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        res = mWindowTransform.offsetMin.x + SHADOW_WIDTH;
                    }
                    else
                    {
                        res = mWindowTransform.offsetMin.x;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.realX = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the real y coordinate.
        /// </summary>
        /// <value>Real y coordinate.</value>
        public float realY
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = 0;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = -MAXIMIZED_OFFSET;
                        }
                        else
                        {
                            res = 0;
                        }
                    }
                    else
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        res = -mWindowTransform.offsetMax.y + SHADOW_WIDTH;
                    }
                    else
                    {
                        res = -mWindowTransform.offsetMax.y;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.realY = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the real width.
        /// </summary>
        /// <value>Real window width.</value>
        public float realWidth
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = Utils.scaledScreenWidth;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = Utils.scaledScreenWidth + MAXIMIZED_OFFSET * 2;
                        }
                        else
                        {
                            res = Utils.scaledScreenWidth;
                        }
                    }
                    else
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        res = mWindowTransform.sizeDelta.x - SHADOW_WIDTH * 2;
                    }
                    else
                    {
                        res = mWindowTransform.sizeDelta.x;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.realWidth = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the real height.
        /// </summary>
        /// <value>Real window height.</value>
        public float realHeight
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = Utils.scaledScreenHeight;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = Utils.scaledScreenHeight + MAXIMIZED_OFFSET * 2;
                        }
                        else
                        {
                            res = Utils.scaledScreenHeight;
                        }
                    }
                    else
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        res = mWindowTransform.sizeDelta.y - SHADOW_WIDTH * 2;
                    }
                    else
                    {
                        res = mWindowTransform.sizeDelta.y;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.realHeight = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the content x coordinate.
        /// </summary>
        /// <value>Content x coordinate.</value>
        public float contentX
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = 0;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = mBorderLeft - SHADOW_WIDTH - MAXIMIZED_OFFSET;
                        }
                        else
                        {
                            res = 0;
                        }
                    }
                    else
                    if (mState == WindowState.Minimized)
                    {
                        res = 0;
                    }
                    else
                    {
                        res = mX + mBorderLeft;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.contentX = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the content y coordinate.
        /// </summary>
        /// <value>Content y coordinate.</value>
        public float contentY
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = 0;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = mBorderTop - SHADOW_WIDTH - MAXIMIZED_OFFSET;
                        }
                        else
                        {
                            res = 0;
                        }
                    }
                    else
                    if (mState == WindowState.Minimized)
                    {
                        res = 0;
                    }
                    else
                    {
                        res = mY + mBorderTop;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.contentY = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the content width.
        /// </summary>
        /// <value>Content width.</value>
        public float contentWidth
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = Utils.scaledScreenWidth;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = Utils.scaledScreenWidth - mBorderLeft - mBorderRight + SHADOW_WIDTH * 2 + MAXIMIZED_OFFSET * 2;
                        }
                        else
                        {
                            res = Utils.scaledScreenWidth;
                        }
                    }
                    else
                    if (mState == WindowState.Minimized)
                    {
                        res = 0;
                    }
                    else
                    {
                        res = mWindowTransform.sizeDelta.x - mBorderLeft - mBorderRight;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.contentWidth = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the content height.
        /// </summary>
        /// <value>Content height.</value>
        public float contentHeight
        {
            get
            {
                float res;

                if (!IsUICreated())
                {
                    res = 0;
                }
                else
                {
                    if (mState == WindowState.FullScreen)
                    {
                        res = Utils.scaledScreenHeight;
                    }
                    else
                    if (mState == WindowState.Maximized)
                    {
                        if (mFrame != WindowFrameType.Frameless)
                        {
                            res = Utils.scaledScreenHeight - mBorderTop - mBorderBottom + SHADOW_WIDTH * 2 + MAXIMIZED_OFFSET * 2;
                        }
                        else
                        {
                            res = Utils.scaledScreenHeight;
                        }
                    }
                    else
                    if (mState == WindowState.Minimized)
                    {
                        res = 0;
                    }
                    else
                    {
                        res = mWindowTransform.sizeDelta.y - mBorderTop - mBorderBottom;
                    }
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.contentHeight = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets the content transform.
        /// </summary>
        /// <value>The content transform.</value>
        public RectTransform contentTransform
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.contentTransform = {0}", mContentTransform);

                return mContentTransform;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this window is resizable.
        /// </summary>
        /// <value><c>true</c> if window is resizable; otherwise, <c>false</c>.</value>
        public bool resizable
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.resizable = {0}", mResizable);

                return mResizable;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.resizable: {0} => {1}", mResizable, value);

                mResizable = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum width.
        /// </summary>
        /// <value>The minimum width.</value>
        public float minimumWidth
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.minimumWidth = {0}", mMinimumWidth);

                return mMinimumWidth;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.minimumWidth: {0} => {1}", mMinimumWidth, value);

                if (mMinimumWidth != value)
                {
                    mMinimumWidth = value;

                    if (mMinimumWidth != 0f)
                    {
                        if (mWidth != 0f)
                        {
                            if (width < mMinimumWidth)
                            {
                                width = mMinimumWidth;
                            }
                        }

                        if (mMaximumWidth != 0f)
                        {
                            if (mMaximumWidth < mMinimumWidth)
                            {
                                mMaximumWidth = mMinimumWidth;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum width.
        /// </summary>
        /// <value>The maximum width.</value>
        public float maximumWidth
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.maximumWidth = {0}", mMaximumWidth);

                return mMaximumWidth;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.maximumWidth: {0} => {1}", mMaximumWidth, value);

                if (mMaximumWidth != value)
                {
                    mMaximumWidth = value;

                    if (mMaximumWidth != 0f)
                    {
                        if (mWidth != 0f)
                        {
                            if (width > mMaximumWidth)
                            {
                                width = mMaximumWidth;
                            }
                        }

                        if (mMinimumWidth != 0f)
                        {
                            if (mMinimumWidth > mMaximumWidth)
                            {
                                mMinimumWidth = mMaximumWidth;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum height.
        /// </summary>
        /// <value>The minimum height.</value>
        public float minimumHeight
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.minimumHeight = {0}", mMinimumHeight);

                return mMinimumHeight;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.minimumHeight: {0} => {1}", mMinimumHeight, value);

                if (mMinimumHeight != value)
                {
                    mMinimumHeight = value;

                    if (mMinimumHeight != 0f)
                    {
                        if (mHeight != 0f)
                        {
                            if (height < mMinimumHeight)
                            {
                                height = mMinimumHeight;
                            }
                        }

                        if (mMaximumHeight != 0f)
                        {
                            if (mMaximumHeight < mMinimumHeight)
                            {
                                mMaximumHeight = mMinimumHeight;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum height.
        /// </summary>
        /// <value>The maximum height.</value>
        public float maximumHeight
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.maximumHeight = {0}", mMaximumHeight);

                return mMaximumHeight;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.maximumHeight: {0} => {1}", mMaximumHeight, value);

                if (mMaximumHeight != value)
                {
                    mMaximumHeight = value;

                    if (mMaximumHeight != 0f)
                    {
                        if (mHeight != 0f)
                        {
                            if (height > mMaximumHeight)
                            {
                                height = mMaximumHeight;
                            }
                        }

                        if (mMinimumHeight != 0f)
                        {
                            if (mMinimumHeight > mMaximumHeight)
                            {
                                mMinimumHeight = mMaximumHeight;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this window allow to be minimized.
        /// </summary>
        /// <value><c>true</c> if allow to minimize; otherwise, <c>false</c>.</value>
        public bool allowMinimize
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.allowMinimize = {0}", mAllowMinimize);

                return mAllowMinimize;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.allowMinimize: {0} => {1}", mAllowMinimize, value);

                if (mAllowMinimize != value)
                {
                    mAllowMinimize = value;

                    if (IsUICreated())
                    {
                        RebuildHeader();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this window allow to be maximized.
        /// </summary>
        /// <value><c>true</c> if allow to maximize; otherwise, <c>false</c>.</value>
        public bool allowMaximize
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.allowMaximize = {0}", mAllowMaximize);

                return mAllowMaximize;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.allowMaximize: {0} => {1}", mAllowMaximize, value);

                if (mAllowMaximize != value)
                {
                    mAllowMaximize = value;

                    if (IsUICreated())
                    {
                        RebuildHeader();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this window allow to close.
        /// </summary>
        /// <value><c>true</c> if allow to close; otherwise, <c>false</c>.</value>
        public bool allowClose
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.allowClose = {0}", mAllowClose);

                return mAllowClose;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.allowClose: {0} => {1}", mAllowClose, value);

                if (mAllowClose != value)
                {
                    mAllowClose = value;

                    if (IsUICreated())
                    {
                        RebuildHeader();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets token ID for translation.
        /// </summary>
        /// <value>Token ID for translation.</value>
        public R.sections.WindowTitles.strings tokenId
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("WindowScript.tokenId = {0}", mTokenId);

                return mTokenId;
            }

            set
            {
                DebugEx.VeryVerboseFormat("WindowScript.tokenId: {0} => {1}", mTokenId, value);

                if (mTokenId != value)
                {
                    mTokenId = value;

                    if (mTitleText != null)
                    {
                        UpdateTitleText();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the window title.
        /// </summary>
        /// <value>The window title.</value>
        public string windowTitle
        {
            get
            {
                string res;

                if (mTokenId == R.sections.WindowTitles.strings.Count)
                {
                    res = "";
                }
                else
                {
                    res = Translator.GetString(mTokenId);
                }

                DebugEx.VeryVeryVerboseFormat("WindowScript.windowTitle = {0}", res);

                return res;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Common.UI.Windows.WindowScript"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public virtual bool selected
        {
            get
            {
                bool res = (sSelectedWindow == this);

                DebugEx.VeryVeryVerboseFormat("WindowScript.selected = {0}", res);

                return res;
            }
        }



        private WindowFrameType                 mFrame;
        private WindowState                     mState;
        private float                           mX;
        private float                           mY;
        private float                           mWidth;
        private float                           mHeight;
        private Color                           mBackgroundColor;
        private bool                            mResizable;
        private float                           mMinimumWidth;
        private float                           mMinimumHeight;
        private float                           mMaximumWidth;
        private float                           mMaximumHeight;
        private bool                            mAllowMinimize;
        private bool                            mAllowMaximize;
        private bool                            mAllowClose;
        private R.sections.WindowTitles.strings mTokenId;

        private RectTransform                   mWindowTransform;
        private GameObject                      mBorderGameObject;
        private Image                           mBorderImage;
        private GameObject                      mTitleGameObject;
        private Text                            mTitleText;
        private GameObject                      mMinimizeGameObject;
        private Image                           mMinimizeImage;
        private GameObject                      mMaximizeGameObject;
        private Image                           mMaximizeImage;
        private GameObject                      mCloseGameObject;
        private Image                           mCloseImage;
        private RectTransform                   mContentTransform;
        private Image                           mContentBackgroundImage;
        private GameObject                      mReplacementGameObject;
        private RectTransform                   mReplacementTransform;
        private float                           mBorderLeft;
        private float                           mBorderTop;
        private float                           mBorderRight;
        private float                           mBorderBottom;
        private MouseLocation                   mMouseLocation;
        private MouseState                      mMouseState;
        private MouseContext                    mMouseContext;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.Windows.WindowScript"/> class.
        /// </summary>
        public WindowScript()
            : base()
        {
            DebugEx.Verbose("Created WindowScript object");

            sInstances.Add(this);

            mFrame           = WindowFrameType.Window;
            mState           = WindowState.NoState;
            mX               = -SHADOW_WIDTH;
            mY               = -SHADOW_WIDTH;
            mWidth           = 0f;
            mHeight          = 0f;
            mBackgroundColor = Assets.Common.Windows.Colors.background;
            mResizable       = true;
            mMinimumWidth    = 0f;
            mMinimumHeight   = 0f;
            mMaximumWidth    = 0f;
            mMaximumHeight   = 0f;
            mAllowMinimize   = true;
            mAllowMaximize   = true;
            mAllowClose      = true;
            mTokenId         = R.sections.WindowTitles.strings.Count;

            mWindowTransform        = null;
            mBorderGameObject       = null;
            mBorderImage            = null;
            mTitleGameObject        = null;
            mTitleText              = null;
            mMinimizeGameObject     = null;
            mMinimizeImage          = null;
            mMaximizeGameObject     = null;
            mMaximizeImage          = null;
            mCloseGameObject        = null;
            mCloseImage             = null;
            mContentTransform       = null;
            mContentBackgroundImage = null;
            mReplacementGameObject  = null;
            mReplacementTransform   = null;
            mBorderLeft             = 0f;
            mBorderTop              = 0f;
            mBorderRight            = 0f;
            mBorderBottom           = 0f;
            mMouseLocation          = MouseLocation.Outside;
            mMouseState             = MouseState.NoState;
            mMouseContext           = null;

            Hide();
        }

        /// <summary>
        /// Script starting callback.
        /// </summary>
        protected virtual void Start()
        {
            DebugEx.Verbose("WindowScript.Start()");

            ResizeListenerScript.AddListener(OnScreenResize);

            CreateUI();
        }

        /// <summary>
        /// Creates user interface.
        /// </summary>
        private void CreateUI()
        {
            DebugEx.Verbose("WindowScript.CreateUI()");

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            mWindowTransform = gameObject.AddComponent<RectTransform>();
            Utils.AlignRectTransformStretchStretch(mWindowTransform);
            #endregion

            if (IsFramePresent())
            {
                CreateBorder();
            }

            float contentWidth  = 0f;
            float contentHeight = 0f;

            //***************************************************************************
            // Content GameObject
            //***************************************************************************
            #region Content GameObject
            GameObject content = new GameObject("Content");
            Utils.InitUIObject(content, transform);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            mContentTransform = content.AddComponent<RectTransform>();
            Utils.AlignRectTransformStretchStretch(mContentTransform, mBorderLeft, mBorderTop, mBorderRight, mBorderBottom);
            #endregion

            //===========================================================================
            // CanvasRenderer Component
            //===========================================================================
            #region CanvasRenderer Component
            content.AddComponent<CanvasRenderer>();
            #endregion

            //===========================================================================
            // Image Component
            //===========================================================================
            #region Image Component
            mContentBackgroundImage = content.AddComponent<Image>();
            mContentBackgroundImage.color = mBackgroundColor;
            #endregion

            //===========================================================================
            // Mask Component
            //===========================================================================
            #region Mask Component
            content.AddComponent<Mask>();
            #endregion

            CreateContent(content.transform, out contentWidth, out contentHeight);
            #endregion

            if (mWidth == 0 || mHeight == 0)
            {
                mWidth  = mBorderLeft + contentWidth  + mBorderRight;
                mHeight = mBorderTop  + contentHeight + mBorderBottom;

                #region Apply constraints
                if (IsFramePresent())
                {
                    if (mWidth < MINIMAL_WIDTH + SHADOW_WIDTH * 2)
                    {
                        mWidth = MINIMAL_WIDTH + SHADOW_WIDTH * 2;
                    }

                    if (mMinimumWidth != 0 && mWidth < mMinimumWidth + SHADOW_WIDTH * 2)
                    {
                        mWidth = mMinimumWidth + SHADOW_WIDTH * 2;
                    }

                    if (mMaximumWidth != 0 && mWidth > mMaximumWidth + SHADOW_WIDTH * 2)
                    {
                        mWidth = mMaximumWidth + SHADOW_WIDTH * 2;
                    }

                    if (mHeight < MINIMAL_HEIGHT + SHADOW_WIDTH * 2)
                    {
                        mHeight = MINIMAL_HEIGHT + SHADOW_WIDTH * 2;
                    }

                    if (mMinimumHeight != 0 && mHeight < mMinimumHeight + SHADOW_WIDTH * 2)
                    {
                        mHeight = mMinimumHeight + SHADOW_WIDTH * 2;
                    }

                    if (mMaximumHeight != 0 && mHeight > mMaximumHeight + SHADOW_WIDTH * 2)
                    {
                        mHeight = mMaximumHeight + SHADOW_WIDTH * 2;
                    }
                }
                else
                {
                    if (mWidth < MINIMAL_WIDTH)
                    {
                        mWidth = MINIMAL_WIDTH;
                    }

                    if (mMinimumWidth != 0 && mWidth < mMinimumWidth)
                    {
                        mWidth = mMinimumWidth;
                    }

                    if (mMaximumWidth != 0 && mWidth > mMaximumWidth)
                    {
                        mWidth = mMaximumWidth;
                    }

                    if (mHeight < MINIMAL_HEIGHT)
                    {
                        mHeight = MINIMAL_HEIGHT;
                    }

                    if (mMinimumHeight != 0 && mHeight < mMinimumHeight)
                    {
                        mHeight = mMinimumHeight;
                    }

                    if (mMaximumHeight != 0 && mHeight > mMaximumHeight)
                    {
                        mHeight = mMaximumHeight;
                    }
                }
                #endregion

                mX = (Utils.scaledScreenWidth  - mWidth)  / 2; // Utils.scaledScreenWidth  / 2 - mWidth / 2;
                mY = (Utils.scaledScreenHeight - mHeight) / 2; // Utils.scaledScreenHeight / 2 - mHeight / 2;
            }

            UpdateState();

            if (mState != WindowState.Minimized)
            {
                OnResize();
            }
        }

        /// <summary>
        /// Creates border.
        /// </summary>
        private void CreateBorder()
        {
            DebugEx.Verbose("WindowScript.CreateBorder()");

            if (mBorderGameObject == null)
            {
                //***************************************************************************
                // Border GameObject
                //***************************************************************************
                #region Border GameObject
                mBorderGameObject = new GameObject("Border");
                Utils.InitUIObject(mBorderGameObject, transform);

                //===========================================================================
                // RectTransform Component
                //===========================================================================
                #region RectTransform Component
                RectTransform borderTransform = mBorderGameObject.AddComponent<RectTransform>();
                Utils.AlignRectTransformStretchStretch(borderTransform);
                #endregion

                //===========================================================================
                // CanvasRenderer Component
                //===========================================================================
                #region CanvasRenderer Component
                mBorderGameObject.AddComponent<CanvasRenderer>();
                #endregion

                //===========================================================================
                // Image Component
                //===========================================================================
                #region Image Component
                mBorderImage = mBorderGameObject.AddComponent<Image>();

                UpdateBorderImage();

                mBorderImage.type       = Image.Type.Sliced;
                mBorderImage.fillCenter = false;

                UpdateBorders();
                #endregion
                #endregion

                if (mFrame != WindowFrameType.SingleFrame)
                {
                    CreateHeader();
                }
            }
            else
            {
                DebugEx.Error("Border game object already created");
            }
        }

        /// <summary>
        /// Updates border image.
        /// </summary>
        private void UpdateBorderImage()
        {
            DebugEx.Verbose("WindowScript.UpdateBorderImage()");

            switch (mFrame)
            {
                case WindowFrameType.Window:
                {
                    if (sSelectedWindow == this)
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.window.sprite;
                    }
                    else
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.windowDeselected.sprite;
                    }
                }
                break;

                case WindowFrameType.SubWindow:
                {
                    if (sSelectedWindow == this)
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.subWindow.sprite;
                    }
                    else
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.subWindowDeselected.sprite;
                    }
                }
                break;

                case WindowFrameType.Drawer:
                {
                    if (sSelectedWindow == this)
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.drawer.sprite;
                    }
                    else
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.drawerDeselected.sprite;
                    }
                }
                break;

                case WindowFrameType.SingleFrame:
                {
                    if (sSelectedWindow == this)
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.singleFrame.sprite;
                    }
                    else
                    {
                        mBorderImage.sprite = Assets.Common.Windows.Textures.singleFrameDeselected.sprite;
                    }
                }
                break;

                default:
                {
                    DebugEx.ErrorFormat("Unknown window frame: {0}", mFrame);
                }
                break;
            }
        }

        /// <summary>
        /// Updates information about borders.
        /// </summary>
        private void UpdateBorders()
        {
            DebugEx.Verbose("WindowScript.UpdateBorders()");

            if (mBorderImage != null)
            {
                Vector4	borders = mBorderImage.sprite.border;

                mBorderLeft   = borders.x;
                mBorderTop    = borders.w;
                mBorderRight  = borders.z;
                mBorderBottom = borders.y;
            }
            else
            {
                mBorderLeft   = 0f;
                mBorderTop    = 0f;
                mBorderRight  = 0f;
                mBorderBottom = 0f;
            }
        }

        /// <summary>
        /// Destroies border.
        /// </summary>
        private void DestroyBorder()
        {
            DebugEx.Verbose("WindowScript.DestroyBorder()");

            DestroyHeader();

            UnityEngine.Object.DestroyObject(mBorderGameObject);
            mBorderGameObject = null;
            mBorderImage      = null;

            UpdateBorders();
        }

        /// <summary>
        /// Creates header.
        /// </summary>
        private void CreateHeader()
        {
            DebugEx.Verbose("WindowScript.CreateHeader()");

            switch (mFrame)
            {
                case WindowFrameType.Window:
                case WindowFrameType.SubWindow:
                {
                    float contentWidth = SHADOW_WIDTH + 4f;

                    if (
                        mAllowMinimize
                        ||
                        mAllowMaximize
                        ||
                        mAllowClose
                       )
                    {
                        float buttonWidth;
                        float buttonHeight = 20f;

                        buttonWidth = 48f;

                        //***************************************************************************
                        // Close GameObject
                        //***************************************************************************
                        #region Close GameObject
                        mCloseGameObject = new GameObject("Close");
                        Utils.InitUIObject(mCloseGameObject, mBorderGameObject.transform);

                        //===========================================================================
                        // RectTransform Component
                        //===========================================================================
                        #region RectTransform Component
                        RectTransform closeTransform = mCloseGameObject.AddComponent<RectTransform>();
                        Utils.AlignRectTransformTopRight(closeTransform, buttonWidth, buttonHeight, contentWidth, SHADOW_WIDTH);
                        #endregion

                        //***************************************************************************
                        // CloseImage GameObject
                        //***************************************************************************
                        #region CloseImage GameObject
                        GameObject closeImageObject = new GameObject("Image");
                        Utils.InitUIObject(closeImageObject, mCloseGameObject.transform);

                        //===========================================================================
                        // RectTransform Component
                        //===========================================================================
                        #region RectTransform Component
                        RectTransform closeImageTransform = closeImageObject.AddComponent<RectTransform>();
                        Utils.AlignRectTransformStretchStretch(closeImageTransform, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH);
                        #endregion

                        //===========================================================================
                        // CanvasRenderer Component
                        //===========================================================================
                        #region CanvasRenderer Component
                        closeImageObject.AddComponent<CanvasRenderer>();
                        #endregion

                        //===========================================================================
                        // Image Component
                        //===========================================================================
                        #region Image Component
                        mCloseImage = closeImageObject.AddComponent<Image>();

                        if (sSelectedWindow == this)
                        {
                            mCloseImage.sprite = Assets.Common.Windows.Textures.closeButton.sprite;
                        }
                        else
                        {
                            mCloseImage.sprite = Assets.Common.Windows.Textures.closeButtonDeselected.sprite;
                        }

                        mCloseImage.type = Image.Type.Sliced;
                        #endregion

                        //===========================================================================
                        // ButtonGlowScript Component
                        //===========================================================================
                        #region ButtonGlowScript Component
                        ButtonGlowScript closeButtonGlowScript = closeImageObject.AddComponent<ButtonGlowScript>();

                        closeButtonGlowScript.rectTransform = closeTransform;
                        #endregion
                        #endregion

                        //===========================================================================
                        // Button Component
                        //===========================================================================
                        #region Button Component
                        Button closeButton = mCloseGameObject.AddComponent<Button>();

                        closeButton.interactable  = mAllowClose;
                        closeButton.targetGraphic = mCloseImage;
                        closeButton.transition    = Selectable.Transition.SpriteSwap;
                        closeButton.spriteState   = Assets.Common.Windows.SpriteStates.closeButton.spriteState;
                        closeButton.onClick.AddListener(Close);
                        #endregion
                        #endregion

                        if (
                            mAllowMinimize
                            ||
                            mAllowMaximize
                           )
                        {
                            contentWidth += buttonWidth - 1; // One button overlaps another one
                            buttonWidth = 28f;

                            //***************************************************************************
                            // Maximize GameObject
                            //***************************************************************************
                            #region Maximize GameObject
                            mMaximizeGameObject = new GameObject("Maximize");
                            Utils.InitUIObject(mMaximizeGameObject, mBorderGameObject.transform);

                            //===========================================================================
                            // RectTransform Component
                            //===========================================================================
                            #region RectTransform Component
                            RectTransform maximizeTransform = mMaximizeGameObject.AddComponent<RectTransform>();
                            Utils.AlignRectTransformTopRight(maximizeTransform, buttonWidth, buttonHeight, contentWidth, SHADOW_WIDTH);
                            #endregion

                            //***************************************************************************
                            // MaximizeImage GameObject
                            //***************************************************************************
                            #region MaximizeImage GameObject
                            GameObject maximizeImageObject = new GameObject("Image");
                            Utils.InitUIObject(maximizeImageObject, mMaximizeGameObject.transform);

                            //===========================================================================
                            // RectTransform Component
                            //===========================================================================
                            #region RectTransform Component
                            RectTransform maximizeImageTransform = maximizeImageObject.AddComponent<RectTransform>();
                            Utils.AlignRectTransformStretchStretch(maximizeImageTransform, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH);
                            #endregion

                            //===========================================================================
                            // CanvasRenderer Component
                            //===========================================================================
                            #region CanvasRenderer Component
                            maximizeImageObject.AddComponent<CanvasRenderer>();
                            #endregion

                            //===========================================================================
                            // Image Component
                            //===========================================================================
                            #region Image Component
                            mMaximizeImage = maximizeImageObject.AddComponent<Image>();

                            if (mState != WindowState.Maximized)
                            {
                                if (sSelectedWindow == this)
                                {
                                    mMaximizeImage.sprite = Assets.Common.Windows.Textures.maximizeButton.sprite;
                                }
                                else
                                {
                                    mMaximizeImage.sprite = Assets.Common.Windows.Textures.maximizeButtonDeselected.sprite;
                                }
                            }
                            else
                            {
                                if (sSelectedWindow == this)
                                {
                                    mMaximizeImage.sprite = Assets.Common.Windows.Textures.normalizeButton.sprite;
                                }
                                else
                                {
                                    mMaximizeImage.sprite = Assets.Common.Windows.Textures.normalizeButtonDeselected.sprite;
                                }
                            }

                            mMaximizeImage.type = Image.Type.Sliced;
                            #endregion

                            //===========================================================================
                            // ButtonGlowScript Component
                            //===========================================================================
                            #region ButtonGlowScript Component
                            ButtonGlowScript maximizeButtonGlowScript = maximizeImageObject.AddComponent<ButtonGlowScript>();

                            maximizeButtonGlowScript.rectTransform = maximizeTransform;
                            #endregion
                            #endregion

                            //===========================================================================
                            // Button Component
                            //===========================================================================
                            #region Button Component
                            Button maximizeButton = mMaximizeGameObject.AddComponent<Button>();

                            maximizeButton.interactable  = mAllowMaximize;
                            maximizeButton.targetGraphic = mMaximizeImage;
                            maximizeButton.transition    = Selectable.Transition.SpriteSwap;

                            if (mState != WindowState.Maximized)
                            {
                                maximizeButton.spriteState = Assets.Common.Windows.SpriteStates.maximizeButton.spriteState;
                            }
                            else
                            {
                                maximizeButton.spriteState = Assets.Common.Windows.SpriteStates.normalizeButton.spriteState;
                            }

                            maximizeButton.onClick.AddListener(OnMaximizeClicked);
                            #endregion
                            #endregion

                            contentWidth += buttonWidth - 1; // One button overlaps another one
                            buttonWidth = 29f;

                            //***************************************************************************
                            // Minimize GameObject
                            //***************************************************************************
                            #region Minimize GameObject
                            mMinimizeGameObject = new GameObject("Minimize");
                            Utils.InitUIObject(mMinimizeGameObject, mBorderGameObject.transform);

                            //===========================================================================
                            // RectTransform Component
                            //===========================================================================
                            #region RectTransform Component
                            RectTransform minimizeTransform = mMinimizeGameObject.AddComponent<RectTransform>();
                            Utils.AlignRectTransformTopRight(minimizeTransform, buttonWidth, buttonHeight, contentWidth, SHADOW_WIDTH);
                            #endregion

                            //***************************************************************************
                            // MinimizeImage GameObject
                            //***************************************************************************
                            #region MinimizeImage GameObject
                            GameObject minimizeImageObject = new GameObject("Image");
                            Utils.InitUIObject(minimizeImageObject, mMinimizeGameObject.transform);

                            //===========================================================================
                            // RectTransform Component
                            //===========================================================================
                            #region RectTransform Component
                            RectTransform minimizeImageTransform = minimizeImageObject.AddComponent<RectTransform>();
                            Utils.AlignRectTransformStretchStretch(minimizeImageTransform, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH, -BUTTON_GLOW_WIDTH);
                            #endregion

                            //===========================================================================
                            // CanvasRenderer Component
                            //===========================================================================
                            #region CanvasRenderer Component
                            minimizeImageObject.AddComponent<CanvasRenderer>();
                            #endregion

                            //===========================================================================
                            // Image Component
                            //===========================================================================
                            #region Image Component
                            mMinimizeImage = minimizeImageObject.AddComponent<Image>();

                            if (mState != WindowState.Minimized)
                            {
                                if (sSelectedWindow == this)
                                {
                                    mMinimizeImage.sprite = Assets.Common.Windows.Textures.minimizeButton.sprite;
                                }
                                else
                                {
                                    mMinimizeImage.sprite = Assets.Common.Windows.Textures.minimizeButtonDeselected.sprite;
                                }
                            }
                            else
                            {
                                if (sSelectedWindow == this)
                                {
                                    mMinimizeImage.sprite = Assets.Common.Windows.Textures.normalizeButton.sprite;
                                }
                                else
                                {
                                    mMinimizeImage.sprite = Assets.Common.Windows.Textures.normalizeButtonDeselected.sprite;
                                }
                            }

                            mMinimizeImage.type = Image.Type.Sliced;
                            #endregion

                            //===========================================================================
                            // ButtonGlowScript Component
                            //===========================================================================
                            #region ButtonGlowScript Component
                            ButtonGlowScript minimizeButtonGlowScript = minimizeImageObject.AddComponent<ButtonGlowScript>();

                            minimizeButtonGlowScript.rectTransform = minimizeTransform;
                            #endregion
                            #endregion

                            //===========================================================================
                            // Button Component
                            //===========================================================================
                            #region Button Component
                            Button minimizeButton = mMinimizeGameObject.AddComponent<Button>();

                            minimizeButton.interactable  = mAllowMinimize;
                            minimizeButton.targetGraphic = mMinimizeImage;
                            minimizeButton.transition    = Selectable.Transition.SpriteSwap;

                            if (mState != WindowState.Minimized)
                            {
                                minimizeButton.spriteState = Assets.Common.Windows.SpriteStates.minimizeButton.spriteState;
                            }
                            else
                            {
                                minimizeButton.spriteState = Assets.Common.Windows.SpriteStates.normalizeButton.spriteState;
                            }

                            minimizeButton.onClick.AddListener(OnMinimizeClicked);
                            #endregion
                            #endregion
                        }

                        contentWidth += buttonWidth + 4f;
                    }

                    //***************************************************************************
                    // Title GameObject
                    //***************************************************************************
                    #region Title GameObject
                    mTitleGameObject = new GameObject("Title");
                    Utils.InitUIObject(mTitleGameObject, mBorderGameObject.transform);

                    //===========================================================================
                    // RectTransform Component
                    //===========================================================================
                    #region RectTransform Component
                    RectTransform titleTransform = mTitleGameObject.AddComponent<RectTransform>();
                    Utils.AlignRectTransformTopStretch(titleTransform, mBorderTop - SHADOW_WIDTH - 8f, SHADOW_WIDTH + 4f, SHADOW_WIDTH + 8f, contentWidth);
                    #endregion

                    #region Text Component
                    mTitleText = mTitleGameObject.AddComponent<Text>();

                    Assets.Common.Windows.TextStyles.title.Apply(mTitleText);

                    Translator.AddLanguageChangedListener(UpdateTitleText);
                    UpdateTitleText();
                    #endregion
                    #endregion
                }
                break;

                case WindowFrameType.Drawer:
                {
                    float contentWidth = SHADOW_WIDTH + 5f;

                    if (mAllowClose)
                    {
                        float buttonSize = 17f;

                        //***************************************************************************
                        // Close GameObject
                        //***************************************************************************
                        #region Close GameObject
                        mCloseGameObject = new GameObject("Close");
                        Utils.InitUIObject(mCloseGameObject, mBorderGameObject.transform);

                        //===========================================================================
                        // RectTransform Component
                        //===========================================================================
                        #region RectTransform Component
                        RectTransform closeTransform = mCloseGameObject.AddComponent<RectTransform>();
                        Utils.AlignRectTransformTopRight(closeTransform, buttonSize, buttonSize, contentWidth, SHADOW_WIDTH + 4f);
                        #endregion

                        //***************************************************************************
                        // CloseImage GameObject
                        //***************************************************************************
                        #region CloseImage GameObject
                        GameObject closeImageObject = new GameObject("Image");
                        Utils.InitUIObject(closeImageObject, mCloseGameObject.transform);

                        //===========================================================================
                        // RectTransform Component
                        //===========================================================================
                        #region RectTransform Component
                        RectTransform closeImageTransform = closeImageObject.AddComponent<RectTransform>();
                        Utils.AlignRectTransformStretchStretch(closeImageTransform, -TOOL_BUTTON_GLOW_WIDTH, -TOOL_BUTTON_GLOW_WIDTH, -TOOL_BUTTON_GLOW_WIDTH, -TOOL_BUTTON_GLOW_WIDTH);
                        #endregion

                        //===========================================================================
                        // CanvasRenderer Component
                        //===========================================================================
                        #region CanvasRenderer Component
                        closeImageObject.AddComponent<CanvasRenderer>();
                        #endregion

                        //===========================================================================
                        // Image Component
                        //===========================================================================
                        #region Image Component
                        mCloseImage = closeImageObject.AddComponent<Image>();

                        if (sSelectedWindow == this)
                        {
                            mCloseImage.sprite = Assets.Common.Windows.Textures.toolCloseButton.sprite;
                        }
                        else
                        {
                            mCloseImage.sprite = Assets.Common.Windows.Textures.toolCloseButtonDeselected.sprite;
                        }

                        mCloseImage.type = Image.Type.Sliced;
                        #endregion

                        //===========================================================================
                        // ButtonGlowScript Component
                        //===========================================================================
                        #region ButtonGlowScript Component
                        ButtonGlowScript closeButtonGlowScript = closeImageObject.AddComponent<ButtonGlowScript>();

                        closeButtonGlowScript.rectTransform = closeTransform;
                        #endregion
                        #endregion

                        //===========================================================================
                        // Button Component
                        //===========================================================================
                        #region Button Component
                        Button closeButton = mCloseGameObject.AddComponent<Button>();

                        closeButton.interactable  = mAllowClose;
                        closeButton.targetGraphic = mCloseImage;
                        closeButton.transition    = Selectable.Transition.SpriteSwap;
                        closeButton.spriteState   = Assets.Common.Windows.SpriteStates.toolCloseButton.spriteState;
                        closeButton.onClick.AddListener(Close);
                        #endregion
                        #endregion

                        contentWidth += buttonSize + 4f;
                    }

                    //***************************************************************************
                    // Title GameObject
                    //***************************************************************************
                    #region Title GameObject
                    mTitleGameObject = new GameObject("Title");
                    Utils.InitUIObject(mTitleGameObject, mBorderGameObject.transform);

                    //===========================================================================
                    // RectTransform Component
                    //===========================================================================
                    #region RectTransform Component
                    RectTransform titleTransform = mTitleGameObject.AddComponent<RectTransform>();
                    Utils.AlignRectTransformTopStretch(titleTransform, mBorderTop - SHADOW_WIDTH - 8f, SHADOW_WIDTH + 4f, SHADOW_WIDTH + 8f, contentWidth);
                    #endregion

                    #region Text Component
                    mTitleText = mTitleGameObject.AddComponent<Text>();

                    Assets.Common.Windows.TextStyles.title.Apply(mTitleText);

                    Translator.AddLanguageChangedListener(UpdateTitleText);
                    UpdateTitleText();
                    #endregion
                    #endregion
                }
                break;

                case WindowFrameType.SingleFrame:
                case WindowFrameType.Frameless:
                {
                    DebugEx.ErrorFormat("Incorrect window frame: {0}", mFrame);
                }
                break;

                default:
                {
                    DebugEx.ErrorFormat("Unknown window frame: {0}", mFrame);
                }
                break;
            }
        }

        /// <summary>
        /// Updates the title text.
        /// </summary>
        public void UpdateTitleText()
        {
            DebugEx.Verbose("WindowScript.UpdateTitleText()");

            mTitleText.text = windowTitle;
        }

        /// <summary>
        /// Destroies header.
        /// </summary>
        private void DestroyHeader()
        {
            DebugEx.Verbose("WindowScript.DestroyHeader()");

            if (mTitleGameObject != null)
            {
                UnityEngine.Object.DestroyObject(mTitleGameObject);
                mTitleGameObject = null;
                mTitleText       = null;

                Translator.RemoveLanguageChangedListener(UpdateTitleText);
            }

            if (mMinimizeGameObject != null)
            {
                UnityEngine.Object.DestroyObject(mMinimizeGameObject);
                mMinimizeGameObject = null;
                mMinimizeImage      = null;
            }

            if (mMaximizeGameObject != null)
            {
                UnityEngine.Object.DestroyObject(mMaximizeGameObject);
                mMaximizeGameObject = null;
                mMaximizeImage      = null;
            }

            if (mCloseGameObject != null)
            {
                UnityEngine.Object.DestroyObject(mCloseGameObject);
                mCloseGameObject = null;
                mCloseImage      = null;
            }
        }

        /// <summary>
        /// Rebuilds header.
        /// </summary>
        private void RebuildHeader()
        {
            DebugEx.Verbose("WindowScript.RebuildHeader()");

            DestroyHeader();

            if (IsFramePresent())
            {
                if (mFrame != WindowFrameType.SingleFrame)
                {
                    CreateHeader();
                }
            }
        }

        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="contentTransform">Content transform.</param>
        /// <param name="width">Width of content.</param>
        /// <param name="height">Height of content.</param>
        protected virtual void CreateContent(Transform contentTransform, out float width, out float height)
        {
            DebugEx.VerboseFormat("WindowScript.CreateContent(contentTransform = {0})", contentTransform);

            // Nothing
            width  = 0;
            height = 0;
        }

        /// <summary>
        /// Updates window position on state changes.
        /// </summary>
        private void UpdateState()
        {
            DebugEx.Verbose("WindowScript.UpdateState()");

            switch (mState)
            {
                case WindowState.NoState:
                {
                    Utils.AlignRectTransformTopLeft(mWindowTransform, mWidth, mHeight, mX, mY);
                }
                break;

                case WindowState.Minimized:
                {
                    Utils.AlignRectTransformTopLeft(
                                                      mWindowTransform
                                                    , MINIMIZED_WIDTH  + 2 * SHADOW_WIDTH
                                                    , MINIMIZED_HEIGHT + 2 * SHADOW_WIDTH
                                                    , MINIMIZED_OFFSET_LEFT - SHADOW_WIDTH
                                                    , Utils.scaledScreenHeight - MINIMIZED_OFFSET_BOTTOM - MINIMIZED_HEIGHT - SHADOW_WIDTH
                                                   );
                }
                break;

                case WindowState.Maximized:
                {
                    if (mFrame != WindowFrameType.Frameless)
                    {
                        Utils.AlignRectTransformStretchStretch(
                                                                 mWindowTransform
                                                               , -SHADOW_WIDTH - MAXIMIZED_OFFSET
                                                               , -SHADOW_WIDTH - MAXIMIZED_OFFSET
                                                               , -SHADOW_WIDTH - MAXIMIZED_OFFSET
                                                               , -SHADOW_WIDTH - MAXIMIZED_OFFSET
                                                              );
                    }
                    else
                    {
                        Utils.AlignRectTransformStretchStretch(mWindowTransform);
                    }
                }
                break;

                case WindowState.FullScreen:
                {
                    Utils.AlignRectTransformStretchStretch(mWindowTransform);
                }
                break;

                default:
                {
                    DebugEx.ErrorFormat("Unknown window state: {0}", mState);
                }
                break;
            }
        }

        /// <summary>
        /// Creates the replacement and stretch it to the left side.
        /// </summary>
        private void CreateReplacementStretchLeft()
        {
            DebugEx.Verbose("WindowScript.CreateReplacementStretchLeft()");

            CreateReplacement();
            Utils.AlignRectTransformStretchLeft(mReplacementTransform, Utils.scaledScreenWidth / 2 + SHADOW_WIDTH, -SHADOW_WIDTH / 2, -SHADOW_WIDTH / 2, -SHADOW_WIDTH / 2);
        }

        /// <summary>
        /// Creates the replacement and stretch it to the right side.
        /// </summary>
        private void CreateReplacementStretchRight()
        {
            DebugEx.Verbose("WindowScript.CreateReplacementStretchRight()");

            CreateReplacement();
            Utils.AlignRectTransformStretchRight(mReplacementTransform, Utils.scaledScreenWidth / 2 + SHADOW_WIDTH, -SHADOW_WIDTH / 2, -SHADOW_WIDTH / 2, -SHADOW_WIDTH / 2);
        }

        /// <summary>
        /// Creates the replacement and stretch it to full screen.
        /// </summary>
        private void CreateReplacementStretchStretch()
        {
            DebugEx.Verbose("WindowScript.CreateReplacementStretchStretch()");

            CreateReplacement();
            Utils.AlignRectTransformStretchStretch(mReplacementTransform, -SHADOW_WIDTH / 2, -SHADOW_WIDTH / 2, -SHADOW_WIDTH / 2, -SHADOW_WIDTH / 2);
        }

        /// <summary>
        /// Creates the replacement and stretch it vertically on the same x position.
        /// </summary>
        private void CreateReplacementStretchVertical()
        {
            DebugEx.Verbose("WindowScript.CreateReplacementStretchVertical()");

            CreateReplacement();
            Utils.AlignRectTransformTopLeft(mReplacementTransform, mWidth, Utils.scaledScreenHeight + SHADOW_WIDTH, mX, -SHADOW_WIDTH / 2);
        }

        /// <summary>
        /// Creates the replacement game object.
        /// </summary>
        /// <returns>The replacement RectTransform.</returns>
        private void CreateReplacement()
        {
            DebugEx.Verbose("WindowScript.CreateReplacement()");

            if (mReplacementGameObject == null)
            {
                //***************************************************************************
                // Replacement GameObject
                //***************************************************************************
                #region Replacement GameObject
                mReplacementGameObject = new GameObject("Replacement");
                Utils.InitUIObject(mReplacementGameObject, transform.parent);
                mReplacementGameObject.transform.SetSiblingIndex(transform.GetSiblingIndex());

                //===========================================================================
                // RectTransform Component
                //===========================================================================
                #region RectTransform Component
                mReplacementTransform = mReplacementGameObject.AddComponent<RectTransform>();
                #endregion

                //===========================================================================
                // CanvasRenderer Component
                //===========================================================================
                #region CanvasRenderer Component
                mReplacementGameObject.AddComponent<CanvasRenderer>();
                #endregion

                //===========================================================================
                // Image Component
                //===========================================================================
                #region Image Component
                Image replacementImage = mReplacementGameObject.AddComponent<Image>();

                replacementImage.sprite = Assets.Common.Windows.Textures.replacement.sprite;
                replacementImage.type   = Image.Type.Sliced;
                #endregion
                #endregion
            }
        }

        /// <summary>
        /// Destroies the replacement game object.
        /// </summary>
        private void DestroyReplacement()
        {
            DebugEx.Verbose("WindowScript.DestroyReplacement()");

            if (mReplacementGameObject != null)
            {
                UnityEngine.Object.DestroyObject(mReplacementGameObject);
                mReplacementGameObject = null;
                mReplacementTransform  = null;
            }
        }

        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        protected virtual void OnDestroy()
        {
            DebugEx.Verbose("WindowScript.OnDestroy()");

            ResizeListenerScript.RemoveListener(OnScreenResize);

            if (mTitleText != null)
            {
                Translator.RemoveLanguageChangedListener(UpdateTitleText);
            }



            DestroyReplacement();

#if !CURSORLESS_PLATFORM
            RemoveCursorIfNeeded();
#endif



            foreach (Pair<WindowScript, List<WindowScript>> pair in sFullscreenWindows)
            {
                pair.second.Remove(this);
            }

            if (mState == WindowState.FullScreen)
            {
                RemoveFromFullscreenList();
            }

            if (!sInstances.Remove(this))
            {
                DebugEx.Error("Failed to remove window");
            }

            if (sSelectedWindow == this)
            {
                sSelectedWindow = null;

                if (sInstances.Count > 0)
                {
                    sInstances[0].SetSelected(true);
                }
            }
        }

        /// <summary>
        /// Handler for click event on mimimize button.
        /// </summary>
        protected void OnMinimizeClicked()
        {
            DebugEx.UserInteraction("WindowScript.OnMinimizeClicked()");

            if (mState != WindowState.Minimized)
            {
                state = WindowState.Minimized;
            }
            else
            {
                state = WindowState.NoState;
            }
        }

        /// <summary>
        /// Handler for click event on maximize button.
        /// </summary>
        protected void OnMaximizeClicked()
        {
            DebugEx.UserInteraction("WindowScript.OnMaximizeClicked()");

            if (mState != WindowState.Maximized)
            {
                state = WindowState.Maximized;
            }
            else
            {
                state = WindowState.NoState;
            }
        }

        /// <summary>
        /// Close this window.
        /// </summary>
        public void Close()
        {
            DebugEx.UserInteraction("WindowScript.Close()");

            UnityEngine.Object.DestroyObject(gameObject);
        }

#if !CURSORLESS_PLATFORM
        /// <summary>
        /// Removes the cursor if needed.
        /// </summary>
        private void RemoveCursorIfNeeded()
        {
            DebugEx.Verbose("WindowScript.RemoveCursorIfNeeded()");

            if (
                mResizable
                &&
                (
                 mMouseLocation == MouseLocation.North
                 ||
                 mMouseLocation == MouseLocation.South
                 ||
                 mMouseLocation == MouseLocation.West
                 ||
                 mMouseLocation == MouseLocation.East
                 ||
                 mMouseLocation == MouseLocation.NorthWest
                 ||
                 mMouseLocation == MouseLocation.NorthEast
                 ||
                 mMouseLocation == MouseLocation.SouthWest
                 ||
                 mMouseLocation == MouseLocation.SouthEast
                )
               )
            {
                RemoveCursor();
            }
        }

        /// <summary>
        /// Removes the cursor.
        /// </summary>
        private void RemoveCursor()
        {
            DebugEx.Verbose("WindowScript.RemoveCursor()");

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
#endif

        /// <summary>
        /// Handler for raycast validation.
        /// </summary>
        /// <returns><c>true</c> if raycast handled by this window; otherwise, <c>false</c>.</returns>
        /// <param name="sp">Screen point</param>
        /// <param name="eventCamera">Event camera.</param>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            DebugEx.VeryVerboseFormat("WindowScript.IsRaycastLocationValid(sp = {0}, eventCamera = {1})", sp, eventCamera);

            if (IsFramePresent())
            {
                if (mState == WindowState.Maximized)
                {
                    return true;
                }

                float mouseX = Mouse.scaledX;
                float mouseY = Mouse.scaledY;

                float windowX      = mWindowTransform.offsetMin.x  + SHADOW_WIDTH;
                float windowY      = -mWindowTransform.offsetMax.y + SHADOW_WIDTH;
                float windowWidth  = mWindowTransform.sizeDelta.x - 2 * SHADOW_WIDTH;
                float windowHeight = mWindowTransform.sizeDelta.y - 2 * SHADOW_WIDTH;

                return (
                        mouseX >= windowX
                        &&
                        mouseX <= windowX + windowWidth
                        &&
                        mouseY >= windowY
                        &&
                        mouseY <= windowY + windowHeight
                       );
            }

            return true;
        }

        /// <summary>
        /// Handler for pointer enter event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            DebugEx.VerboseFormat("WindowScript.OnPointerEnter(eventData = {0})", eventData);

            if (mMouseState == MouseState.NoState)
            {
                mMouseLocation = MouseLocation.Inside;
            }
        }

        /// <summary>
        /// Handler for pointer exit event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            DebugEx.VerboseFormat("WindowScript.OnPointerExit(eventData = {0})", eventData);

            if (mMouseState == MouseState.NoState)
            {
#if !CURSORLESS_PLATFORM
                RemoveCursorIfNeeded();
#endif

                mMouseLocation = MouseLocation.Outside;
            }
        }

        /// <summary>
        /// Starts dragging mode.
        /// </summary>
        protected void StartDragging()
        {
            DebugEx.Verbose("WindowScript.StartDragging()");

            float mouseX = Mouse.scaledX;
            float mouseY = Mouse.scaledY;

            mMouseState = MouseState.Dragging;

            mMouseContext = new MouseContext(mouseX, mouseY, x, y, width, height, mWindowTransform.offsetMin.x, -mWindowTransform.offsetMax.y);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        protected virtual void Update()
        {
            DebugEx.VeryVeryVerbose("WindowScript.Update()");

            bool leftMouseButtonPressed = InputControl.GetMouseButtonDown(MouseButton.Left);

            if (IsFramePresent())
            {
                switch (mMouseState)
                {
                    case MouseState.NoState:
                    {
                        if (mMouseLocation != MouseLocation.Outside)
                        {
                            bool isInsideButtons = false;

                            float mouseX = Mouse.scaledX;
                            float mouseY = Mouse.scaledY;

                            if (mMinimizeGameObject != null)
                            {
                                RectTransform buttonTransform = mMinimizeGameObject.transform as RectTransform;
                                Vector3[] corners = Utils.GetWindowCorners(buttonTransform);

                                if (
                                    mouseX >= corners[0].x
                                    &&
                                    mouseX <= corners[3].x
                                    &&
                                    mouseY >= corners[0].y
                                    &&
                                    mouseY <= corners[3].y
                                   )
                                {
                                    isInsideButtons = true;

                                    mMinimizeGameObject.transform.SetAsLastSibling();
                                }
                            }

                            if (mMaximizeGameObject != null)
                            {
                                RectTransform buttonTransform = mMaximizeGameObject.transform as RectTransform;
                                Vector3[] corners = Utils.GetWindowCorners(buttonTransform);

                                if (
                                    mouseX >= corners[0].x
                                    &&
                                    mouseX <= corners[3].x
                                    &&
                                    mouseY >= corners[0].y
                                    &&
                                    mouseY <= corners[3].y
                                   )
                                {
                                    isInsideButtons = true;

                                    mMaximizeGameObject.transform.SetAsLastSibling();
                                }
                            }

                            if (mCloseGameObject != null)
                            {
                                RectTransform buttonTransform = mCloseGameObject.transform as RectTransform;
                                Vector3[] corners = Utils.GetWindowCorners(buttonTransform);

                                if (
                                    mouseX >= corners[0].x
                                    &&
                                    mouseX <= corners[3].x
                                    &&
                                    mouseY >= corners[0].y
                                    &&
                                    mouseY <= corners[3].y
                                   )
                                {
                                    isInsideButtons = true;

                                    mCloseGameObject.transform.SetAsLastSibling();
                                }
                            }

                            switch (mState)
                            {
                                case WindowState.NoState:
                                {
#if !CURSORLESS_PLATFORM
                                    MouseLocation oldLocation = mMouseLocation;
#endif

                                    if (isInsideButtons)
                                    {
                                        mMouseLocation = MouseLocation.Inside;
                                    }
                                    else
                                    {
                                        if (mResizable)
                                        {
                                            if (mouseY <= mY + SHADOW_WIDTH + RESIZING_GAP)
                                            {
                                                if (mouseX <= mX + mBorderLeft)
                                                {
                                                    mMouseLocation = MouseLocation.NorthWest;
                                                }
                                                else
                                                if (mouseX < mX + mWidth - mBorderRight)
                                                {
                                                    mMouseLocation = MouseLocation.North;
                                                }
                                                else
                                                {
                                                    mMouseLocation = MouseLocation.NorthEast;
                                                }
                                            }
                                            else
                                            if (mouseY <= mY + mBorderTop)
                                            {
                                                if (mouseX <= mX + mBorderLeft)
                                                {
                                                    mMouseLocation = MouseLocation.West;
                                                }
                                                else
                                                if (mouseX < mX + mWidth - mBorderRight)
                                                {
                                                    mMouseLocation = MouseLocation.Header;
                                                }
                                                else
                                                {
                                                    mMouseLocation = MouseLocation.East;
                                                }
                                            }
                                            else
                                            if (mouseY < mY + mHeight - mBorderBottom)
                                            {
                                                if (mouseX <= mX + mBorderLeft)
                                                {
                                                    mMouseLocation = MouseLocation.West;
                                                }
                                                else
                                                if (mouseX < mX + mWidth - mBorderRight)
                                                {
                                                    mMouseLocation = MouseLocation.Inside;
                                                }
                                                else
                                                {
                                                    mMouseLocation = MouseLocation.East;
                                                }
                                            }
                                            else
                                            {
                                                if (mouseX <= mX + mBorderLeft)
                                                {
                                                    mMouseLocation = MouseLocation.SouthWest;
                                                }
                                                else
                                                if (mouseX < mX + mWidth - mBorderRight)
                                                {
                                                    mMouseLocation = MouseLocation.South;
                                                }
                                                else
                                                {
                                                    mMouseLocation = MouseLocation.SouthEast;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (mouseY <= mY + mBorderTop)
                                            {
                                                mMouseLocation = MouseLocation.Header;
                                            }
                                            else
                                            {
                                                mMouseLocation = MouseLocation.Inside;
                                            }
                                        }
                                    }

#if !CURSORLESS_PLATFORM
                                    if (mResizable && oldLocation != mMouseLocation)
                                    {
                                        switch (mMouseLocation)
                                        {
                                            case MouseLocation.North:
                                            case MouseLocation.South:
                                            {
                                                Cursor.SetCursor(Assets.Common.Cursors.northSouth.texture, new Vector2(16f * Utils.canvasScale, 16f * Utils.canvasScale), CursorMode.Auto);
                                            }
                                            break;

                                            case MouseLocation.West:
                                            case MouseLocation.East:
                                            {
                                                Cursor.SetCursor(Assets.Common.Cursors.eastWest.texture, new Vector2(16f * Utils.canvasScale, 16f * Utils.canvasScale), CursorMode.Auto);
                                            }
                                            break;

                                            case MouseLocation.NorthWest:
                                            case MouseLocation.SouthEast:
                                            {
                                                Cursor.SetCursor(Assets.Common.Cursors.northWestSouthEast.texture, new Vector2(16f * Utils.canvasScale, 16f * Utils.canvasScale), CursorMode.Auto);
                                            }
                                            break;

                                            case MouseLocation.NorthEast:
                                            case MouseLocation.SouthWest:
                                            {
                                                Cursor.SetCursor(Assets.Common.Cursors.northEastSouthWest.texture, new Vector2(16f * Utils.canvasScale, 16f * Utils.canvasScale), CursorMode.Auto);
                                            }
                                            break;

                                            case MouseLocation.Header:
                                            case MouseLocation.Inside:
                                            {
                                                RemoveCursor();
                                            }
                                            break;

                                            case MouseLocation.Outside:
                                            {
                                                DebugEx.ErrorFormat("Incorrect mouse location: {0}", mMouseLocation);
                                            }
                                            break;

                                            default:
                                            {
                                                DebugEx.ErrorFormat("Unknown mouse location: {0}", mMouseLocation);
                                            }
                                            break;
                                        }
                                    }
#endif
                                }
                                break;

                                case WindowState.Minimized:
                                {
                                    if (isInsideButtons)
                                    {
                                        mMouseLocation = MouseLocation.Inside;
                                    }
                                    else
                                    {
                                        mMouseLocation = MouseLocation.Header;
                                    }
                                }
                                break;

                                case WindowState.Maximized:
                                {
                                    if (isInsideButtons)
                                    {
                                        mMouseLocation = MouseLocation.Inside;
                                    }
                                    else
                                    {
                                        if (mouseY <= mBorderTop - SHADOW_WIDTH - MAXIMIZED_OFFSET)
                                        {
                                            mMouseLocation = MouseLocation.Header;
                                        }
                                        else
                                        {
                                            mMouseLocation = MouseLocation.Inside;
                                        }
                                    }
                                }
                                break;

                                case WindowState.FullScreen:
                                {
                                    DebugEx.ErrorFormat("Incorrect window state: {0}", mState);
                                }
                                break;

                                default:
                                {
                                    DebugEx.ErrorFormat("Unknown window state: {0}", mState);
                                }
                                break;
                            }

                            switch (mMouseLocation)
                            {
                                case MouseLocation.Header:
                                {
                                    if (leftMouseButtonPressed)
                                    {
                                        StartDragging();
                                    }
                                }
                                break;

                                case MouseLocation.North:
                                case MouseLocation.South:
                                case MouseLocation.West:
                                case MouseLocation.East:
                                case MouseLocation.NorthWest:
                                case MouseLocation.SouthEast:
                                case MouseLocation.NorthEast:
                                case MouseLocation.SouthWest:
                                {
                                    if (mResizable && mState == WindowState.NoState)
                                    {
                                        if (leftMouseButtonPressed)
                                        {
                                            mMouseState = MouseState.Resizing;

                                            mMouseContext = new MouseContext(mouseX, mouseY, x, y, width, height, mWindowTransform.offsetMin.x, -mWindowTransform.offsetMax.y);
                                        }
                                    }
                                }
                                break;

                                case MouseLocation.Inside:
                                {
                                    // Nothing
                                }
                                break;

                                case MouseLocation.Outside:
                                {
                                    DebugEx.ErrorFormat("Incorrect mouse location: {0}", mMouseLocation);
                                }
                                break;

                                default:
                                {
                                    DebugEx.ErrorFormat("Unknown mouse location: {0}", mMouseLocation);
                                }
                                break;
                            }
                        }
                    }
                    break;

                    case MouseState.Dragging:
                    {
                        float mouseX = Mouse.scaledX;
                        float mouseY = Mouse.scaledY;

                        #region Calculate new position
                        float screenWidth  = Utils.scaledScreenWidth;
                        float screenHeight = Utils.scaledScreenHeight;

                        if (mState == WindowState.Maximized)
                        {
                            if (
                                mouseX == mMouseContext.previousMouseX
                                &&
                                mouseY == mMouseContext.previousMouseY
                               )
                            {
                                if (InputControl.GetMouseButtonUp(MouseButton.Left))
                                {
                                    mMouseState   = MouseState.NoState;
                                    mMouseContext = null;
                                }

                                return;
                            }

                            state = WindowState.NoState;
                            mMouseContext.previousX = mMouseContext.previousMouseX - (mMouseContext.previousMouseX / screenWidth) * width;
                            mMouseContext.previousY = 0;
                        }

                        float newX = 0f;
                        float newY = 0f;

                        switch (mState)
                        {
                            case WindowState.NoState:
                            {
                                newX = mMouseContext.previousX + mouseX - mMouseContext.previousMouseX;
                                newY = mMouseContext.previousY + mouseY - mMouseContext.previousMouseY;
                            }
                            break;

                            case WindowState.Minimized:
                            {
                                newX = mMouseContext.previousRectX + mouseX - mMouseContext.previousMouseX + SHADOW_WIDTH;
                                newY = mMouseContext.previousRectY + mouseY - mMouseContext.previousMouseY + SHADOW_WIDTH;
                            }
                            break;

                            case WindowState.Maximized:
                            case WindowState.FullScreen:
                            {
                                DebugEx.ErrorFormat("Incorrect window state: {0}", mState);
                            }
                            break;

                            default:
                            {
                                DebugEx.ErrorFormat("Unknown window state: {0}", mState);
                            }
                            break;
                        }

                        float windowWidth = mWindowTransform.sizeDelta.x - 2 * SHADOW_WIDTH;

                        if (newX + windowWidth < DRAGGING_GAP)
                        {
                            newX = -windowWidth + DRAGGING_GAP;
                        }
                        else
                        if (newX > screenWidth - DRAGGING_GAP)
                        {
                            newX = screenWidth - DRAGGING_GAP;
                        }

                        if (newY < -mBorderTop + DRAGGING_GAP + SHADOW_WIDTH)
                        {
                            newY = -mBorderTop + DRAGGING_GAP + SHADOW_WIDTH;
                        }
                        else
                        if (newY > screenHeight - DRAGGING_GAP)
                        {
                            newY = screenHeight - DRAGGING_GAP;
                        }

                        switch (mState)
                        {
                            case WindowState.NoState:
                            {
                                x = newX;
                                y = newY;
                            }
                            break;

                            case WindowState.Minimized:
                            {
                                newX -= SHADOW_WIDTH;
                                newY -= SHADOW_WIDTH;
                                windowWidth += 2 * SHADOW_WIDTH;
                                float windowHeight = mWindowTransform.sizeDelta.y;

                                mWindowTransform.offsetMin = new Vector2(
                                                                           newX
                                                                         , -newY - windowHeight
                                                                        );

                                mWindowTransform.offsetMax = new Vector2(
                                                                           newX + windowWidth
                                                                         , -newY
                                                                        );
                            }
                            break;

                            case WindowState.Maximized:
                            case WindowState.FullScreen:
                            {
                                DebugEx.ErrorFormat("Incorrect window state: {0}", mState);
                            }
                            break;

                            default:
                            {
                                DebugEx.ErrorFormat("Unknown window state: {0}", mState);
                            }
                            break;
                        }
                        #endregion

                        #region Show/Hide replacement
                        bool replacementVisible = false;

                        if (mouseX < DRAGGING_GAP)
                        {
                            if (mResizable)
                            {
                                replacementVisible = true;

                                if (
                                    mReplacementGameObject == null
                                    ||
                                    mReplacementTransform.anchorMin != new Vector2(0f, 0f)
                                    ||
                                    mReplacementTransform.anchorMax != new Vector2(0f, 1f)
                                   )
                                {
                                    CreateReplacementStretchLeft();
                                }
                            }
                        }
                        else
                        if (mouseX > screenWidth - DRAGGING_GAP)
                        {
                            if (mResizable)
                            {
                                replacementVisible = true;

                                if (
                                    mReplacementGameObject == null
                                    ||
                                    mReplacementTransform.anchorMin != new Vector2(1f, 0f)
                                    ||
                                    mReplacementTransform.anchorMax != new Vector2(1f, 1f)
                                    )
                                {
                                    CreateReplacementStretchRight();
                                }
                            }
                        }
                        else
                        if (mouseY < DRAGGING_GAP)
                        {
                            if (mAllowMaximize)
                            {
                                replacementVisible = true;

                                if (
                                    mReplacementGameObject == null
                                    ||
                                    mReplacementTransform.anchorMin != new Vector2(0f, 0f)
                                    ||
                                    mReplacementTransform.anchorMax != new Vector2(1f, 1f)
                                    )
                                {
                                    CreateReplacementStretchStretch();
                                }
                            }
                        }

                        if (!replacementVisible)
                        {
                            DestroyReplacement();
                        }
                        #endregion

                        if (InputControl.GetMouseButtonUp(MouseButton.Left))
                        {
                            if (mReplacementGameObject != null)
                            {
                                if (mouseX < DRAGGING_GAP)
                                {
                                    width  = screenWidth / 2;
                                    height = screenHeight;
                                    x      = 0;
                                    y      = 0;
                                }
                                else
                                if (mouseX > screenWidth - DRAGGING_GAP)
                                {
                                    width  = screenWidth / 2;
                                    height = screenHeight;
                                    x      = screenWidth - width;
                                    y      = 0;
                                }
                                else
                                if (mouseY < DRAGGING_GAP)
                                {
                                    state = WindowState.Maximized;
                                }
                            }

                            DestroyReplacement();

                            mMouseState   = MouseState.NoState;
                            mMouseContext = null;

                            Vector3[] corners = Utils.GetWindowCorners(mWindowTransform);

                            if (
                                mouseX < corners[0].x + SHADOW_WIDTH || mouseX > corners[3].x - SHADOW_WIDTH
                                ||
                                mouseY < corners[0].y + SHADOW_WIDTH || mouseY > corners[3].y - SHADOW_WIDTH
                               )
                            {
                                mMouseLocation = MouseLocation.Outside;
                            }
                        }
                    }
                    break;

                    case MouseState.Resizing:
                    {
                        float mouseX = Mouse.scaledX;
                        float mouseY = Mouse.scaledY;

                        #region Calculate new geometry
                        float screenWidth  = Utils.scaledScreenWidth;
                        float screenHeight = Utils.scaledScreenHeight;

                        #region West
                        if (
                            mMouseLocation == MouseLocation.West
                            ||
                            mMouseLocation == MouseLocation.NorthWest
                            ||
                            mMouseLocation == MouseLocation.SouthWest
                           )
                        {
                            float newX     = mMouseContext.previousX     + mouseX - mMouseContext.previousMouseX;
                            float newWidth = mMouseContext.previousWidth - mouseX + mMouseContext.previousMouseX;

                            if (newWidth < MINIMAL_WIDTH)
                            {
                                newX     -= MINIMAL_WIDTH - newWidth;
                                newWidth  = MINIMAL_WIDTH;
                            }

                            if (mMinimumWidth != 0 && newWidth < mMinimumWidth)
                            {
                                newX     -= mMinimumWidth - newWidth;
                                newWidth  = mMinimumWidth;
                            }

                            if (mMaximumWidth != 0 && newWidth > mMaximumWidth)
                            {
                                newX     -= mMaximumWidth - newWidth;
                                newWidth  = mMaximumWidth;
                            }

                            if (newX > screenWidth - DRAGGING_GAP)
                            {
                                newWidth += newX - (screenWidth - DRAGGING_GAP);
                                newX      = screenWidth - DRAGGING_GAP;
                            }

                            x     = newX;
                            width = newWidth;
                        }
                        #endregion
                        else
                        #region East
                        if (
                            mMouseLocation == MouseLocation.East
                            ||
                            mMouseLocation == MouseLocation.NorthEast
                            ||
                            mMouseLocation == MouseLocation.SouthEast
                           )
                        {
                            float newWidth = mMouseContext.previousWidth + mouseX - mMouseContext.previousMouseX;

                            if (mMouseContext.previousX + newWidth < DRAGGING_GAP)
                            {
                                newWidth = -mMouseContext.previousX + DRAGGING_GAP;
                            }

                            width = newWidth;
                        }
                        #endregion

                        #region North
                        if (
                            mMouseLocation == MouseLocation.North
                            ||
                            mMouseLocation == MouseLocation.NorthWest
                            ||
                            mMouseLocation == MouseLocation.NorthEast
                           )
                        {
                            float newY      = mMouseContext.previousY      + mouseY - mMouseContext.previousMouseY;
                            float newHeight = mMouseContext.previousHeight - mouseY + mMouseContext.previousMouseY;

                            if (newHeight < MINIMAL_HEIGHT)
                            {
                                newY      -= MINIMAL_HEIGHT - newHeight;
                                newHeight  = MINIMAL_HEIGHT;
                            }

                            if (mMinimumHeight != 0 && newHeight < mMinimumHeight)
                            {
                                newY      -= mMinimumHeight - newHeight;
                                newHeight  = mMinimumHeight;
                            }

                            if (mMaximumHeight != 0 && newHeight > mMaximumHeight)
                            {
                                newY      -= mMaximumHeight - newHeight;
                                newHeight  = mMaximumHeight;
                            }

                            if (newY < -mBorderTop + DRAGGING_GAP + SHADOW_WIDTH)
                            {
                                newHeight -= -mBorderTop + DRAGGING_GAP + SHADOW_WIDTH - newY;
                                newY       = -mBorderTop + DRAGGING_GAP + SHADOW_WIDTH;
                            }
                            else
                            if (newY > screenHeight - DRAGGING_GAP)
                            {
                                newHeight += newY - (screenHeight - DRAGGING_GAP);
                                newY       = screenHeight - DRAGGING_GAP;
                            }

                            y      = newY;
                            height = newHeight;
                        }
                        #endregion
                        else
                        #region South
                        if (
                            mMouseLocation == MouseLocation.South
                            ||
                            mMouseLocation == MouseLocation.SouthWest
                            ||
                            mMouseLocation == MouseLocation.SouthEast
                           )
                        {
                            height = mMouseContext.previousHeight + mouseY - mMouseContext.previousMouseY;
                        }
                        #endregion
                        #endregion

                        #region Show/Hide replacement
                        bool replacementVisible = false;

                        if (mouseY < DRAGGING_GAP)
                        {
                            if (
                                mMouseLocation == MouseLocation.North
                                ||
                                mMouseLocation == MouseLocation.NorthWest
                                ||
                                mMouseLocation == MouseLocation.NorthEast
                               )
                            {
                                replacementVisible = true;

                                if (mReplacementGameObject == null)
                                {
                                    CreateReplacementStretchVertical();
                                }
                                else
                                if (
                                    mMouseLocation == MouseLocation.NorthWest
                                    ||
                                    mMouseLocation == MouseLocation.NorthEast
                                   )
                                {
                                    mReplacementTransform.offsetMin = new Vector2(mWindowTransform.offsetMin.x, mReplacementTransform.offsetMin.y);
                                    mReplacementTransform.offsetMax = new Vector2(mWindowTransform.offsetMax.x, mReplacementTransform.offsetMax.y);
                                }
                            }
                        }
                        else
                        if (mouseY > screenHeight - DRAGGING_GAP)
                        {
                            if (
                                mMouseLocation == MouseLocation.South
                                ||
                                mMouseLocation == MouseLocation.SouthWest
                                ||
                                mMouseLocation == MouseLocation.SouthEast
                               )
                            {
                                replacementVisible = true;

                                if (mReplacementGameObject == null)
                                {
                                    CreateReplacementStretchVertical();
                                }
                                else
                                if (
                                    mMouseLocation == MouseLocation.SouthWest
                                    ||
                                    mMouseLocation == MouseLocation.SouthEast
                                   )
                                {
                                    mReplacementTransform.offsetMin = new Vector2(mWindowTransform.offsetMin.x, mReplacementTransform.offsetMin.y);
                                    mReplacementTransform.offsetMax = new Vector2(mWindowTransform.offsetMax.x, mReplacementTransform.offsetMax.y);
                                }
                            }
                        }

                        if (!replacementVisible)
                        {
                            DestroyReplacement();
                        }
                        #endregion

                        if (InputControl.GetMouseButtonUp(MouseButton.Left))
                        {
                            if (mReplacementGameObject != null)
                            {
                                y      = 0;
                                height = screenHeight;
                            }

                            DestroyReplacement();

                            mMouseState   = MouseState.NoState;
                            mMouseContext = null;

                            if (mouseX < mX + SHADOW_WIDTH || mouseX > mX + mWidth  - SHADOW_WIDTH
                                ||
                                mouseY < mY + SHADOW_WIDTH || mouseY > mY + mHeight - SHADOW_WIDTH)
                            {
#if !CURSORLESS_PLATFORM
                                RemoveCursor();
#endif

                                mMouseLocation = MouseLocation.Outside;
                            }
                        }
                    }
                    break;

                    default:
                    {
                        DebugEx.ErrorFormat("Unknown mouse state: {0}", mMouseState);
                    }
                    break;
                }
            }

            if (leftMouseButtonPressed)
            {
                List<RaycastResult> hits = new List<RaycastResult>();
                Mouse.RaycastAll(hits);

                bool isOk       = true;
                bool isSelected = false;

                if (hits.Count > 0)
                {
                    Transform curTransform = hits[0].gameObject.transform;

                    while (curTransform != null)
                    {
                        if (curTransform == transform)
                        {
                            isSelected = true;

                            break;
                        }

                        if (curTransform.GetComponent<WindowScript>() != null)
                        {
                            break;
                        }

                        if (curTransform.GetComponent<PopupMenuAreaScript>() != null)
                        {
                            isOk = false;

                            break;
                        }

                        curTransform = curTransform.parent;
                    }
                }

                if (isOk)
                {
                    SetSelected(isSelected);
                }
            }
        }

        /// <summary>
        /// Handler for screen resize event.
        /// </summary>
        public void OnScreenResize()
        {
            DebugEx.Verbose("WindowScript.OnScreenResize()");

            if (
                mState == WindowState.Maximized
                ||
                mState == WindowState.FullScreen
               )
            {
                OnResize();
            }
        }

        /// <summary>
        /// Handler for resize event.
        /// </summary>
        protected virtual void OnResize()
        {
            DebugEx.Verbose("WindowScript.OnResize()");
        }

        /// <summary>
        /// Sets window selected state.
        /// </summary>
        /// <param name="value">If set to <c>true</c> window is selected.</param>
        private void SetSelected(bool value)
        {
            DebugEx.VerboseFormat("WindowScript.SetSelected(value = {0})", value);

            if (value != (sSelectedWindow == this))
            {
                if (value)
                {
                    if (sSelectedWindow != null)
                    {
                        sSelectedWindow.SetSelected(false);
                    }

                    sSelectedWindow = this;
                    OnSelected();

                    if (sInstances.Count > 0)
                    {
                        if (sInstances[0] != this)
                        {
                            transform.SetAsLastSibling();
                        }
                    }
                    else
                    {
                        DebugEx.Fatal("Unexpected behaviour in WindowScript.SetSelected()");
                    }
                }
                else
                {
                    OnDeselected();
                    sSelectedWindow = null;
                }

                if (IsUICreated())
                {
                    if (IsFramePresent())
                    {
                        UpdateBorderImage();
                    }

                    switch (mFrame)
                    {
                        case WindowFrameType.Window:
                        case WindowFrameType.SubWindow:
                        {
                            if (mMinimizeImage != null)
                            {
                                if (mState != WindowState.Minimized)
                                {
                                    if (value)
                                    {
                                        mMinimizeImage.sprite = Assets.Common.Windows.Textures.minimizeButton.sprite;
                                    }
                                    else
                                    {
                                        mMinimizeImage.sprite = Assets.Common.Windows.Textures.minimizeButtonDeselected.sprite;
                                    }
                                }
                                else
                                {
                                    if (value)
                                    {
                                        mMinimizeImage.sprite = Assets.Common.Windows.Textures.normalizeButton.sprite;
                                    }
                                    else
                                    {
                                        mMinimizeImage.sprite = Assets.Common.Windows.Textures.normalizeButtonDeselected.sprite;
                                    }
                                }
                            }

                            if (mMaximizeImage != null)
                            {
                                if (mState != WindowState.Maximized)
                                {
                                    if (value)
                                    {
                                        mMaximizeImage.sprite = Assets.Common.Windows.Textures.maximizeButton.sprite;
                                    }
                                    else
                                    {
                                        mMaximizeImage.sprite = Assets.Common.Windows.Textures.maximizeButtonDeselected.sprite;
                                    }
                                }
                                else
                                {
                                    if (value)
                                    {
                                        mMaximizeImage.sprite = Assets.Common.Windows.Textures.normalizeButton.sprite;
                                    }
                                    else
                                    {
                                        mMaximizeImage.sprite = Assets.Common.Windows.Textures.normalizeButtonDeselected.sprite;
                                    }
                                }
                            }

                            if (mCloseImage != null)
                            {
                                if (value)
                                {
                                    mCloseImage.sprite = Assets.Common.Windows.Textures.closeButton.sprite;
                                }
                                else
                                {
                                    mCloseImage.sprite = Assets.Common.Windows.Textures.closeButtonDeselected.sprite;
                                }
                            }
                        }
                        break;

                        case WindowFrameType.Drawer:
                        {
                            if (mCloseImage != null)
                            {
                                if (value)
                                {
                                    mCloseImage.sprite = Assets.Common.Windows.Textures.toolCloseButton.sprite;
                                }
                                else
                                {
                                    mCloseImage.sprite = Assets.Common.Windows.Textures.toolCloseButtonDeselected.sprite;
                                }
                            }
                        }
                        break;

                        case WindowFrameType.SingleFrame:
                        case WindowFrameType.Frameless:
                        {
                            // Nothing
                        }
                        break;

                        default:
                        {
                            DebugEx.ErrorFormat("Unknown window frame: {0}", mFrame);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Select this window.
        /// </summary>
        public void Select()
        {
            DebugEx.Verbose("WindowScript.Select()");

            SetSelected(true);
        }

        /// <summary>
        /// Handler for select event.
        /// </summary>
        protected virtual void OnSelected()
        {
            DebugEx.Verbose("WindowScript.OnSelected()");
        }

        /// <summary>
        /// Handler for deselect event.
        /// </summary>
        protected virtual void OnDeselected()
        {
            DebugEx.Verbose("WindowScript.OnDeselected()");
        }

        /// <summary>
        /// Adds this instance to fullscreen list.
        /// </summary>
        private void AddToFullscreenList()
        {
            DebugEx.Verbose("WindowScript.AddToFullscreenList()");

            if (mState == WindowState.FullScreen)
            {
                List<WindowScript> visibleWindows = new List<WindowScript>();

                foreach (WindowScript window in sInstances)
                {
                    if (window != this && window.IsVisible())
                    {
                        visibleWindows.Add(window);
                        window.Hide();
                    }
                }

                sFullscreenWindows.Add(new Pair<WindowScript, List<WindowScript>>(this, visibleWindows));
            }
            else
            {
                DebugEx.Fatal("Unexpected behaviour in WindowScript.AddToFullscreenList()");
            }
        }

        /// <summary>
        /// Removes this instance from fullscreen list.
        /// </summary>
        private void RemoveFromFullscreenList()
        {
            DebugEx.Verbose("WindowScript.RemoveFromFullscreenList()");

            int index = -1;

            for (int i = sFullscreenWindows.Count - 1; i >= 0; --i)
            {
                if (sFullscreenWindows[i].first == this)
                {
                    index = i;

                    break;
                }
            }

            if (index >= 0)
            {
                if (index == sFullscreenWindows.Count - 1)
                {
                    foreach (WindowScript window in sFullscreenWindows[index].second)
                    {
                        window.Show();
                    }
                }
                else
                {
                    sFullscreenWindows[index + 1].second.AddRange(sFullscreenWindows[index].second);
                }

                sFullscreenWindows.RemoveAt(index);
            }
            else
            {
                DebugEx.Fatal("Unexpected behaviour in WindowScript.RemoveFromFullscreenList()");
            }
        }

        /// <summary>
        /// Save window state.
        /// </summary>
        public virtual void Save(string key)
        {
            DebugEx.VerboseFormat("WindowScript.Save(key = {0})", key);

            PlayerPrefs.SetFloat( "Windows." + key + ".X",         x);
            PlayerPrefs.SetFloat( "Windows." + key + ".Y",         y);
            PlayerPrefs.SetFloat( "Windows." + key + ".Width",     width);
            PlayerPrefs.SetFloat( "Windows." + key + ".Height",    height);
            PlayerPrefs.SetString("Windows." + key + ".Maximized", (mState == WindowState.Maximized) ? "True" : "False");

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load window state.
        /// </summary>
        public virtual void Load(string key)
        {
            DebugEx.VerboseFormat("WindowScript.Load(key = {0})", key);

            float tempWidth = PlayerPrefs.GetFloat("Windows." + key + ".Width", 0f);

            if (tempWidth > 0f)
            {
                x      = PlayerPrefs.GetFloat("Windows." + key + ".X",      x);
                y      = PlayerPrefs.GetFloat("Windows." + key + ".Y",      y);
                width  = tempWidth;
                height = PlayerPrefs.GetFloat("Windows." + key + ".Height", height);

                if (PlayerPrefs.GetString("Windows." + key + ".Maximized", (mState == WindowState.Maximized) ? "True" : "False").ToLower() == "true")
                {
                    state = WindowState.Maximized;
                }
            }
        }

        /// <summary>
        /// Show window.
        /// </summary>
        public void Show()
        {
            DebugEx.Verbose("WindowScript.Show()");

            gameObject.SetActive(true);

            SetSelected(true);
        }

        /// <summary>
        /// Hide window.
        /// </summary>
        public void Hide()
        {
            DebugEx.Verbose("WindowScript.Hide()");

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Determines whether this window is visible.
        /// </summary>
        /// <returns><c>true</c> if this window is visible; otherwise, <c>false</c>.</returns>
        public bool IsVisible()
        {
            DebugEx.Verbose("WindowScript.IsVisible()");

            return gameObject.activeSelf;
        }

        /// <summary>
        /// Determines whether frame present or not.
        /// </summary>
        /// <returns><c>true</c> if frame present; otherwise, <c>false</c>.</returns>
        private bool IsFramePresent()
        {
            DebugEx.VeryVeryVerbose("WindowScript.IsFramePresent()");

            return (mFrame != WindowFrameType.Frameless && mState != WindowState.FullScreen);
        }

        /// <summary>
        /// Determines whether user interface created or not.
        /// </summary>
        /// <returns><c>true</c> if user interface created; otherwise, <c>false</c>.</returns>
        private bool IsUICreated()
        {
            DebugEx.Verbose("WindowScript.IsUICreated()");

            return (mWindowTransform != null);
        }
    }
}
