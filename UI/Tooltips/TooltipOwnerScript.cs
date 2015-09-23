using UnityEngine;
using UnityEngine.EventSystems;
using UnityTranslation;



namespace Common.UI.Tooltips
{
    /// <summary>
    /// Script that realize behaviour for tooltip owner.
    /// </summary>
    public class TooltipOwnerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Gets or sets token ID for translation.
        /// </summary>
        /// <value>Token ID for translation.</value>
        public R.sections.Tooltips.strings tokenId
        {
            get
			{
				DebugEx.VeryVeryVerboseFormat("TooltipOwnerScript.tokenId = {0}", mTokenId);

				return mTokenId; 
			}

            set
			{
				DebugEx.VeryVerboseFormat("TooltipOwnerScript.tokenId: {0} => {1}", mTokenId, value);

				mTokenId = value; 
			}
        }



        private R.sections.Tooltips.strings mTokenId = R.sections.Tooltips.strings.Count;



        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        void OnDestroy()
        {
			DebugEx.Verbose("TooltipOwnerScript.OnDestroy()");

            TooltipAreaScript.OnTooltipOwnerDestroy(this);
        }

        /// <summary>
        /// Handler for disable event.
        /// </summary>
        void OnDisable()
        {
			DebugEx.Verbose("TooltipOwnerScript.OnDisable()");

            TooltipAreaScript.OnTooltipOwnerDisable(this);
        }

        /// <summary>
        /// Handler for pointer enter event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
			DebugEx.VerboseFormat("TooltipOwnerScript.OnPointerEnter(eventData = {0})", eventData);

            TooltipAreaScript.OnTooltipOwnerEnter(this);
        }

        /// <summary>
        /// Handler for pointer exit event.
        /// </summary>
        /// <param name="eventData">Pointer data.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
			DebugEx.VerboseFormat("TooltipOwnerScript.OnPointerExit(eventData = {0})", eventData);

            TooltipAreaScript.OnTooltipOwnerExit(this);
        }
    }
}
