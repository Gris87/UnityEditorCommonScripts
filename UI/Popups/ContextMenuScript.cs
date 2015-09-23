using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;



namespace Common.UI.Popups
{
    /// <summary>
    /// Script that realize behaviour for context menu handling.
    /// </summary>
    public class ContextMenuScript : MonoBehaviour, IPointerDownHandler
    {
        /// <summary>
        /// Gets or sets the source object.
        /// </summary>
        /// <value>The source object.</value>
        public object sourceObject
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("ContextMenuScript.sourceObject = {0}", mSourceObject);

                return mSourceObject;
            }

            set
            {
                DebugEx.VeryVerboseFormat("ContextMenuScript.sourceObject: {0} => {1}", mSourceObject, value);

                mSourceObject = value;
            }
        }

        /// <summary>
        /// Gets or sets show context menu callback.
        /// </summary>
        /// <value>Show context menu callback.</value>
        public UnityAction<object> onShowContextMenu
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("ContextMenuScript.onShowContextMenu = {0}", mOnShowContextMenu);

                return mOnShowContextMenu;
            }

            set
            {
                DebugEx.VeryVerboseFormat("ContextMenuScript.onShowContextMenu: {0} => {1}", mOnShowContextMenu, value);

                mOnShowContextMenu = value;
            }
        }



        private object              mSourceObject;
        private UnityAction<object> mOnShowContextMenu;



        /// <summary>
        /// Handler for pointer down event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            DebugEx.VerboseFormat("ContextMenuScript.OnPointerDown(eventData = {0})", eventData);

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                mOnShowContextMenu.Invoke(mSourceObject);
            }
        }
    }
}
