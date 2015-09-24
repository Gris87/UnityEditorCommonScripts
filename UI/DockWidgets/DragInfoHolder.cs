using UnityEngine.Events;
using UnityEngine.EventSystems;



namespace Common.UI.DockWidgets
{
    /// <summary>
    /// Drag info holder.
    /// </summary>
    public static class DragInfoHolder
    {
        /// <summary>
        /// Mouse location.
        /// </summary>
        public enum MouseLocation
        {
            /// <summary>
            /// Outside of this docking area.
            /// </summary>
            Outside
            ,
            /// <summary>
            /// Inside of this docking area.
            /// </summary>
            Inside
            ,
            /// <summary>
            /// The left section.
            /// </summary>
            LeftSection
            ,
            /// <summary>
            /// The right section.
            /// </summary>
            RightSection
            ,
            /// <summary>
            /// The bottom section.
            /// </summary>
            BottomSection
            ,
            /// <summary>
            /// In tabs area of docking group.
            /// </summary>
            Tabs
        }



        /// <summary>
        /// Gets or sets the dock widget.
        /// </summary>
        /// <value>The dock widget.</value>
        public static DockWidgetScript dockWidget
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DragInfoHolder.dockWidget = {0}", sDockWidget);

                return sDockWidget;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DragInfoHolder.dockWidget: {0} => {1}", sDockWidget, value);

                sDockWidget = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        public static float minimum
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DragInfoHolder.minimum = {0}", sMinimum);

                return sMinimum;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DragInfoHolder.minimum: {0} => {1}", sMinimum, value);

                sMinimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the docking area.
        /// </summary>
        /// <value>The docking area.</value>
        public static DockingAreaScript dockingArea
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DragInfoHolder.dockingArea = {0}", sDockingArea);

                return sDockingArea;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DragInfoHolder.dockingArea: {0} => {1}", sDockingArea, value);

                sDockingArea = value;
            }
        }

        /// <summary>
        /// Gets or sets the mouse location.
        /// </summary>
        /// <value>The mouse location.</value>
        public static MouseLocation mouseLocation
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("DragInfoHolder.mouseLocation = {0}", sMouseLocation);

                return sMouseLocation;
            }

            set
            {
                DebugEx.VeryVerboseFormat("DragInfoHolder.mouseLocation: {0} => {1}", sMouseLocation, value);

                sMouseLocation = value;
            }
        }



        private static DockWidgetScript  sDockWidget;
        private static float             sMinimum;
        private static DockingAreaScript sDockingArea;
        private static MouseLocation     sMouseLocation;



        /// <summary>
        /// Initializes the <see cref="Common.UI.DockWidgets.DragInfoHolder"/> class.
        /// </summary>
        static DragInfoHolder()
        {
            DebugEx.Verbose("Created DragInfoHolder object");

            sDockWidget    = null;
            sMinimum       = float.MaxValue;
            sDockingArea   = null;
            sMouseLocation = MouseLocation.Outside;
        }
    }
}
