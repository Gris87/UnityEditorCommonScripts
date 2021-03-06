using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Common.UI.DragAndDrop;
using Common.UI.Windows;



namespace Common.UI.DockWidgets
{
    /// <summary>
    /// Button component for docking group tab.
    /// </summary>
    public class DockingTabButton : Button, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Gets or sets the dock widget.
        /// </summary>
        /// <value>The dock widget.</value>
        public DockWidgetScript dockWidget
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DockingTabButton.dockWidget = {0}", mDockWidget);

                return mDockWidget;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DockingTabButton.dockWidget: {0} => {1}", mDockWidget, value);

                mDockWidget = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Common.UI.DockWidgets.DockingTabButton"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool active
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DockingTabButton.active = {0}", mActive);

                return mActive;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DockingTabButton.active: {0} => {1}", mActive, value);

                if (mActive != value)
                {
                    mActive = value;

                    UpdateImage();
                }
            }
        }



        private DockWidgetScript mDockWidget;
        private bool             mActive;

        List<DockingAreaScript> mDockingAreas;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.DockWidgets.DockingTabButton"/> class.
        /// </summary>
        public DockingTabButton()
            : base()
        {
            DebugEx.Verbose("Created DockingTabButton object");

            transition = Selectable.Transition.SpriteSwap;

            mDockWidget = null;
            mActive     = false;

            mDockingAreas = null;

            onClick.AddListener(ButtonClicked);
        }

        /// <summary>
        /// Script starting callback.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            DebugEx.Verbose("DockingTabButton.Start()");

            UpdateImage();
        }

        /// <summary>
        /// Handler for pointer down event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            DebugEx.VerboseFormat("DockingTabButton.OnPointerDown(eventData = {0})", eventData);

            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                mDockWidget.Destroy();
            }
        }

        /// <summary>
        /// Handler for begin drag event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            DebugEx.VerboseFormat("DockingTabButton.OnBeginDrag(eventData = {0})", eventData);

            ButtonClicked();

            DragInfoHolder.dockWidget    = mDockWidget;
            DragInfoHolder.minimum       = float.MaxValue;
            DragInfoHolder.dockingArea   = null;
            DragInfoHolder.mouseLocation = DragInfoHolder.MouseLocation.Outside;

            mDockingAreas = new List<DockingAreaScript>(DockingAreaScript.instances);

            foreach (DockingAreaScript dockingArea in mDockingAreas)
            {
                dockingArea.CacheDragInfo();
            }

            StartCoroutine(CreateDraggingImage(eventData));
        }

        /// <summary>
        /// Handler for drag event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnDrag(PointerEventData eventData)
        {
            DebugEx.VeryVerboseFormat("DockingTabButton.OnDrag(eventData = {0})", eventData);

            DragInfoHolder.minimum       = float.MaxValue;
            DragInfoHolder.dockingArea   = null;
            DragInfoHolder.mouseLocation = DragInfoHolder.MouseLocation.Outside;

            RaycastResult     raycastResult  = eventData.pointerCurrentRaycast;
            DockingAreaScript hitDockingArea = null;

            if (raycastResult.gameObject != null)
            {
                hitDockingArea = Utils.FindInParents<DockingAreaScript>(raycastResult.gameObject);
            }

            if (hitDockingArea != null && hitDockingArea.HasDragInfo())
            {
                hitDockingArea.PreprocessDockWidgetDrag(eventData);
            }
            else
            {
                for (int i = mDockingAreas.Count - 1; i >= 0; --i)
                {
                    mDockingAreas[i].PreprocessDockWidgetDrag(eventData);
                }
            }

            if (DragInfoHolder.dockingArea != null)
            {
                DragInfoHolder.dockingArea.ProcessDockWidgetDrag(eventData);
            }

            if (DragInfoHolder.dockingArea != null)
            {
                DragData.HideImage();
            }
            else
            {
                DummyDockWidgetScript.DestroyInstance();

                DragData.ShowImage();
                DragData.Drag();
            }
        }

        /// <summary>
        /// Handler for end drag event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            DebugEx.VerboseFormat("DockingTabButton.OnEndDrag(eventData = {0})", eventData);

            foreach (DockingAreaScript dockingArea in mDockingAreas)
            {
                dockingArea.ClearDragInfo();
            }

            mDockingAreas = null;

            if (DummyDockWidgetScript.instance != null)
            {
                int index = DummyDockWidgetScript.instance.parent.children.IndexOf(DummyDockWidgetScript.instance);

                if (index >= 0)
                {
                    DummyDockWidgetScript.instance.parent.InsertDockWidget(DragInfoHolder.dockWidget, index);
                    DummyDockWidgetScript.instance.parent.selectedIndex = index;
                }
                else
                {
                    DebugEx.Fatal("Unexpected behaviour in DockingTabButton.OnEndDrag()");
                }

                DummyDockWidgetScript.DestroyInstance();
            }
            else
            if (DragInfoHolder.dockingArea == null)
            {
                WindowScript parentWindow = Utils.FindInParents<WindowScript>(gameObject);

                if (parentWindow != null)
                {
                    //***************************************************************************
                    // DockingWindow GameObject
                    //***************************************************************************
                    #region DockingWindow GameObject
                    GameObject dockingWindow = new GameObject("DockingWindow");
                    Utils.InitUIObject(dockingWindow, parentWindow.transform.parent);

                    //===========================================================================
                    // DockingWindowScript Component
                    //===========================================================================
                    #region DockingWindowScript Component
                    DockingWindowScript dockingWindowScript = dockingWindow.AddComponent<DockingWindowScript>();

                    dockingWindowScript.dockWidget = DragInfoHolder.dockWidget;

                    dockingWindowScript.x      = DragData.x      - 8f;
                    dockingWindowScript.y      = DragData.y      - 15f; // - 8f      - 7f
                    dockingWindowScript.width  = DragData.width  + 16f; // + 8f + 8f
                    dockingWindowScript.height = DragData.height + 23f; // + 8f + 8f + 7f

                    dockingWindowScript.Show();
                    #endregion
                    #endregion
                }
                else
                {
                    DebugEx.Fatal("Unexpected behaviour in DockingTabButton.OnEndDrag()");
                }
            }

            DragInfoHolder.dockWidget    = null;
            DragInfoHolder.minimum       = float.MaxValue;
            DragInfoHolder.dockingArea   = null;
            DragInfoHolder.mouseLocation = DragInfoHolder.MouseLocation.Outside;

            DragData.EndDrag();
        }

        /// <summary>
        /// Creates the dragging image.
        /// </summary>
        /// <returns>The dragging image.</returns>
        /// <param name="eventData">Pointer data.</param>
        public IEnumerator CreateDraggingImage(PointerEventData eventData)
        {
            DebugEx.VerboseFormat("DockingTabButton.CreateDraggingImage(eventData = {0})", eventData);

            yield return new WaitForEndOfFrame();

            Vector3[] corners = Utils.GetWindowCorners(mDockWidget.parent.transform as RectTransform);

            float screenWidth  = Screen.width;
            float screenHeight = Screen.height;

            float left   = corners[0].x * Utils.canvasScale;
            float top    = corners[0].y * Utils.canvasScale;
            float right  = corners[3].x * Utils.canvasScale;
            float bottom = corners[3].y * Utils.canvasScale;

            if (left < 0f)
            {
                left = 0f;
            }

            if (top < 0f)
            {
                top = 0f;
            }

            if (right > screenWidth - 1)
            {
                right = screenWidth - 1;
            }

            if (bottom > screenHeight - 1)
            {
                bottom = screenHeight - 1;
            }

            int widgetX      = Mathf.CeilToInt(left);
            int widgetY      = Mathf.CeilToInt(top);
            int widgetWidth  = Mathf.FloorToInt(right  - left);
            int widgetHeight = Mathf.FloorToInt(bottom - top);

            float dragPosX = (eventData.pressPosition.x                - widgetX) / Utils.canvasScale;
            float dragPosY = (screenHeight - eventData.pressPosition.y - widgetY) / Utils.canvasScale;

            DragData.BeginDrag(
                                 DraggingType.DockWidget
                               , gameObject
                               , Sprite.Create(
                                                 Utils.TakeScreenshot(widgetX, widgetY, widgetWidth, widgetHeight)
                                               , new Rect(0, 0, widgetWidth, widgetHeight)
                                               , new Vector2(0.5f, 0.5f)
                                              )
                               , widgetWidth  / Utils.canvasScale
                               , widgetHeight / Utils.canvasScale
                               , dragPosX
                               , dragPosY
                              );
        }

        /// <summary>
        /// Button click handler.
        /// </summary>
        private void ButtonClicked()
        {
            DebugEx.Verbose("DockingTabButton.ButtonClicked()");

            mDockWidget.Select();
        }

        /// <summary>
        /// Updates background image.
        /// </summary>
        private void UpdateImage()
        {
            DebugEx.Verbose("DockingTabButton.UpdateImage()");

            if (mActive)
            {
                image.sprite = Assets.Common.DockWidgets.Textures.tabActive.sprite;
                spriteState = Assets.Common.DockWidgets.SpriteStates.activeButton.spriteState;
            }
            else
            {
                image.sprite = Assets.Common.DockWidgets.Textures.tab.sprite;
                spriteState = Assets.Common.DockWidgets.SpriteStates.button.spriteState;
            }
        }
    }
}
