using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityTranslation;



namespace Common.UI.Tooltips
{
    /// <summary>
    /// Script that realize behaviour for tooltip controller.
    /// </summary>
    public class TooltipAreaScript : MonoBehaviour
    {
        private const float SHOW_DELAY = 500f;
        private const float HIDE_DELAY = 500f;



        private static TooltipAreaScript sInstance = null;



        private TooltipOwnerScript mCurrentOwner;
        private TooltipOwnerScript mNextOwner;
        private Timer              mTimer;



        /// <summary>
        /// Script starting callback.
        /// </summary>
        void Start()
        {
            DebugEx.Verbose("TooltipAreaScript.Start()");

            if (sInstance == null)
            {
                sInstance = this;
            }
            else
            {
                DebugEx.Error("Two instances of TooltipAreaScript not supported");
            }

            mCurrentOwner  = null;
            mNextOwner     = null;
            mTimer         = new Timer();
        }

        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        void OnDestroy()
        {
            DebugEx.Verbose("TooltipAreaScript.OnDestroy()");

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
            DebugEx.VeryVeryVerbose("TooltipAreaScript.Update()");

            mTimer.Update();

            if (mCurrentOwner != null)
            {
                if (InputControl.GetMouseButtonDown(MouseButton.Left))
                {
                    DestroyTooltip();
                }
            }
        }

        /// <summary>
        /// Handler on owner destroy event.
        /// </summary>
        /// <param name="owner">Tooltip owner.</param>
        public static void OnTooltipOwnerDestroy(TooltipOwnerScript owner)
        {
            DebugEx.VerboseFormat("TooltipAreaScript.OnTooltipOwnerDestroy(owner = {0})", owner);

            if (sInstance != null)
            {
                if (sInstance.mCurrentOwner == owner)
                {
                    sInstance.DestroyTooltip();
                }
                else
                if (sInstance.mNextOwner == owner)
                {
                    sInstance.mNextOwner = null;
                    sInstance.mTimer.Stop();
                }
            }
            else
            {
                DebugEx.Error("There is no TooltipAreaScript instance");
            }
        }

        /// <summary>
        /// Handler on owner disable event.
        /// </summary>
        /// <param name="owner">Tooltip owner.</param>
        public static void OnTooltipOwnerDisable(TooltipOwnerScript owner)
        {
            DebugEx.VerboseFormat("TooltipAreaScript.OnTooltipOwnerDisable(owner = {0})", owner);

            if (sInstance != null)
            {
                if (sInstance.mCurrentOwner == owner)
                {
                    sInstance.DestroyTooltip();
                }
                else
                if (sInstance.mNextOwner == owner)
                {
                    sInstance.mNextOwner = null;
                    sInstance.mTimer.Stop();
                }
            }
            else
            {
                DebugEx.Error("There is no TooltipAreaScript instance");
            }
        }

        /// <summary>
        /// Handler on owner pointer enter event.
        /// </summary>
        /// <param name="owner">Tooltip owner.</param>
        public static void OnTooltipOwnerEnter(TooltipOwnerScript owner)
        {
            DebugEx.VerboseFormat("TooltipAreaScript.OnTooltipOwnerEnter(owner = {0})", owner);

            if (sInstance != null)
            {
                if (sInstance.mCurrentOwner != null)
                {
                    if (sInstance.mCurrentOwner == owner)
                    {
                        sInstance.mNextOwner = null;
                        sInstance.mTimer.Stop();
                    }
                    else
                    {
                        sInstance.mNextOwner = owner;

                        if (sInstance.mTimer.active)
                        {
                            sInstance.CreateTooltip();
                            sInstance.mTimer.Stop();
                        }
                        else
                        {
                            sInstance.mTimer.Start(sInstance.OnShowTimeout, SHOW_DELAY);
                        }
                    }
                }
                else
                {
                    sInstance.mNextOwner = owner;
                    sInstance.mTimer.Start(sInstance.OnShowTimeout, SHOW_DELAY);
                }
            }
            else
            {
                DebugEx.Error("There is no TooltipAreaScript instance");
            }
        }

        /// <summary>
        /// Handler on owner pointer exit event.
        /// </summary>
        /// <param name="owner">Tooltip owner.</param>
        public static void OnTooltipOwnerExit(TooltipOwnerScript owner)
        {
            DebugEx.VerboseFormat("TooltipAreaScript.OnTooltipOwnerExit(owner = {0})", owner);

            if (sInstance != null)
            {
                sInstance.mNextOwner = null;

                if (sInstance.mCurrentOwner != null)
                {
                    if (sInstance.mCurrentOwner == owner)
                    {
                        sInstance.mTimer.Start(sInstance.OnHideTimeout, HIDE_DELAY);
                    }
                }
                else
                {
                    sInstance.mTimer.Stop();
                }
            }
            else
            {
                DebugEx.Error("There is no TooltipAreaScript instance");
            }
        }

        /// <summary>
        /// Creates tooltip for current tooltip owner.
        /// </summary>
        private void CreateTooltip()
        {
            DebugEx.Verbose("TooltipAreaScript.CreateTooltip()");

            DestroyTooltip();

            mCurrentOwner = mNextOwner;
            mNextOwner    = null;

            //***************************************************************************
            // Tooltip GameObject
            //***************************************************************************
            #region Tooltip GameObject
            GameObject tooltip = new GameObject("Tooltip");
            Utils.InitUIObject(tooltip, transform);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            RectTransform tooltipTransform = tooltip.AddComponent<RectTransform>();
            #endregion

            //===========================================================================
            // CanvasRenderer Component
            //===========================================================================
            #region CanvasRenderer Component
            tooltip.AddComponent<CanvasRenderer>();
            #endregion

            //===========================================================================
            // Image Component
            //===========================================================================
            #region Image Component
            Image tooltipImage = tooltip.AddComponent<Image>();

            tooltipImage.sprite = Assets.Common.Tooltips.Textures.tooltipBackground.sprite;
            tooltipImage.type   = Image.Type.Sliced;

            Vector4 tooltipBorders = tooltipImage.sprite.border;

            float tooltipBorderLeft   = tooltipBorders.x + 2f;
            float tooltipBorderTop    = tooltipBorders.w + 4f;
            float tooltipBorderRight  = tooltipBorders.y + 2f;
            float tooltipBorderBottom = tooltipBorders.z + 4f;
            #endregion

            //===========================================================================
            // Image CanvasGroup
            //===========================================================================
            #region CanvasGroup Component
            CanvasGroup tooltipCanvasGroup = tooltip.AddComponent<CanvasGroup>();

            tooltipCanvasGroup.blocksRaycasts = false;
            #endregion

            //***************************************************************************
            // TooltipText GameObject
            //***************************************************************************
            #region TooltipText GameObject
            GameObject tooltipTextObject = new GameObject("Text");
            Utils.InitUIObject(tooltipTextObject, tooltip.transform);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            RectTransform tooltipTextTransform = tooltipTextObject.AddComponent<RectTransform>();
            Utils.AlignRectTransformStretchStretch(
                                                     tooltipTextTransform
                                                   , tooltipBorderLeft
                                                   , tooltipBorderTop
                                                   , tooltipBorderRight
                                                   , tooltipBorderBottom
                                                  );
            #endregion

            //===========================================================================
            // Text Component
            //===========================================================================
            #region Text Component
            Text tooltipText = tooltipTextObject.AddComponent<Text>();

            Assets.Common.Tooltips.TextStyles.tooltipText.Apply(tooltipText);

            tooltipText.text = Translator.GetString(mCurrentOwner.tokenId);
            #endregion
            #endregion

            float mouseX = Mouse.scaledX;
            float mouseY = Mouse.scaledY;

            float tooltipWidth = tooltipText.preferredWidth + tooltipBorderLeft + tooltipBorderRight;
            float screenWidth  = Utils.scaledScreenWidth;

            if (tooltipWidth > screenWidth)
            {
                tooltipWidth = screenWidth;
            }

            tooltipTransform.sizeDelta = new Vector2(tooltipWidth, 0f);
            float tooltipHeight = tooltipText.preferredHeight + tooltipBorderTop + tooltipBorderBottom;

            Utils.FitRectTransformToScreen(tooltipTransform, tooltipWidth, tooltipHeight, mouseX, mouseY);
            #endregion
        }

        /// <summary>
        /// Destroies previously created tooltip if present.
        /// </summary>
        private void DestroyTooltip()
        {
            DebugEx.Verbose("TooltipAreaScript.DestroyTooltip()");

            if (transform.childCount > 0)
            {
                if (transform.childCount == 1)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(0).gameObject);
                }
                else
                {
                    DebugEx.Fatal("Unexpected behaviour in TooltipAreaScript.DestroyTooltip()");
                }
            }

            mCurrentOwner = null;
        }

        /// <summary>
        /// Handler for show timeout event.
        /// </summary>
        private void OnShowTimeout()
        {
            CreateTooltip();

            mTimer.Stop();
        }

        /// <summary>
        /// Handler for hide timeout event.
        /// </summary>
        private void OnHideTimeout()
        {
            DestroyTooltip();

            mTimer.Stop();
        }
    }
}
