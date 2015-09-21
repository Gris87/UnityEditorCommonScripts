using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



namespace Common
{
    /// <summary>
    /// Class that provide mouse position in normal coordinate system.
    /// </summary>
    public static class Mouse
    {
        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>The x coordinate.</value>
        public static float x
        {
            get
            {
                UpdatePosition();

				DebugEx.VeryVeryVerboseFormat("Mouse.x = {0}", sX);

                return sX;
            }
        }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>The y coordinate.</value>
        public static float y
        {
            get
            {
                UpdatePosition();

				DebugEx.VeryVeryVerboseFormat("Mouse.y = {0}", sY);

                return sY;
            }
        }

        /// <summary>
        /// Gets the scaled x coordinate.
        /// </summary>
        /// <value>The scaled x coordinate.</value>
        public static float scaledX
        {
            get
			{
				float res = x / Utils.canvasScale;

				DebugEx.VeryVeryVerboseFormat("Mouse.scaledX = {0}", res);

				return res;
			}
        }

        /// <summary>
        /// Gets the scaled y coordinate.
        /// </summary>
        /// <value>The scaled y coordinate.</value>
        public static float scaledY
        {
            get
			{
				float res = y / Utils.canvasScale;
				
				DebugEx.VeryVeryVerboseFormat("Mouse.scaledY = {0}", res);
				
				return res;
			}
        }



        private static int                 sLastUpdate;
        private static float               sX;
        private static float               sY;
        private static List<RaycastResult> sHits;



        /// <summary>
        /// Initializes the <see cref="Common.Mouse"/> class.
        /// </summary>
        static Mouse()
        {
			DebugEx.Verbose("Static class Mouse initialized");

            sLastUpdate = -1;
            sX          = -1;
            sY          = -1;
            sHits       = null;
        }

        /// <summary>
        /// Updates position if needed.
        /// </summary>
        private static void UpdatePosition()
        {
			DebugEx.VeryVeryVerbose("Mouse.UpdatePosition()");

            if (sLastUpdate != Time.frameCount)
            {
                sLastUpdate = Time.frameCount;

                Vector3 mousePos = InputControl.mousePosition;

                sX = mousePos.x;
                sY = Screen.height - mousePos.y;

                sHits = null;
            }
        }

        /// <summary>
        /// Raycasts all.
        /// </summary>
        /// <param name="hits">List of raycast results.</param>
        public static void RaycastAll(List<RaycastResult> hits)
        {
			DebugEx.VeryVeryVerbose("Mouse.RaycastAll()");

            UpdatePosition();

            if (sHits == null)
            {
                PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
                pointerEvent.position = InputControl.mousePosition;

                sHits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEvent, sHits);
            }

            hits.AddRange(sHits);
        }
    }
}
