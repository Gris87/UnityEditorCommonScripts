using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



namespace Common.UI.Popups
{
    /// <summary>
    /// Script that realize behaviour for auto popup item.
    /// </summary>
    public class AutoPopupItemScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Delay before showing popup.
        /// </summary>
        public float delay
        {
            get
			{
				DebugEx.VeryVeryVerboseFormat("AutoPopupItemScript.delay = {0}", mDelay);

				return mDelay;
			}

            set
			{
				DebugEx.VeryVerboseFormat("AutoPopupItemScript.delay: {0} => {1}", mDelay, value);

				mDelay = value; 
			}
        }



		private Button mButton;
		private float  mDelay  = 0f;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
			DebugEx.Verbose("AutoPopupItemScript.Start()");

            mButton = GetComponent<Button>();

            if (mButton == null)
            {
                DebugEx.Error("Button component not found");
            }
        }

        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        void OnDestroy()
        {
			DebugEx.Verbose("AutoPopupItemScript.OnDestroy()");

            PopupMenuAreaScript.OnAutoPopupItemDestroy(this);
        }

        /// <summary>
        /// Handler for disable event.
        /// </summary>
        void OnDisable()
        {
			DebugEx.Verbose("AutoPopupItemScript.OnDisable()");

            PopupMenuAreaScript.OnAutoPopupItemDisable(this);
        }

        /// <summary>
        /// Handler for pointer enter event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
			DebugEx.VerboseFormat("AutoPopupItemScript.OnPointerEnter(eventData = {0})", eventData);

            PopupMenuAreaScript.OnAutoPopupItemEnter(this);
        }

        /// <summary>
        /// Handler for pointer exit event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
			DebugEx.VerboseFormat("AutoPopupItemScript.OnPointerExit(eventData = {0})", eventData);

            PopupMenuAreaScript.OnAutoPopupItemExit(this);
        }

        /// <summary>
        /// Click the button.
        /// </summary>
        public void Click()
        {
			DebugEx.Verbose("AutoPopupItemScript.Click()");

            mButton.onClick.Invoke();
        }
    }
}
