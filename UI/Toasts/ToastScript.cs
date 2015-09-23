using UnityEngine;
using UnityEngine.UI;
using UnityTranslation;



namespace Common.UI.Toasts
{
    /// <summary>
    /// Script that realize toast behaviour.
    /// </summary>
    public class ToastScript : MonoBehaviour
    {
        private const float FADE_TIME = 300f / 1000f;



        public string                    text;
        public R.sections.Toasts.strings tokenId;
        public object[]                  tokenArguments;
        public float                     duration;



        private float mDelay;

        private CanvasGroup mToastCanvasGroup;
        private Text        mToastText;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.Toasts.ToastScript"/> class.
        /// </summary>
        public ToastScript()
            : base()
        {
            DebugEx.Verbose("Created ToastScript object");

            mDelay = 0f;

            mToastCanvasGroup = null;
            mToastText        = null;

            enabled = false;
        }

        /// <summary>
        /// Handler for destroy event.
        /// </summary>
        void OnDestroy()
        {
            DebugEx.Verbose("ToastScript.OnDestroy()");

            if (tokenId != R.sections.Toasts.strings.Count)
            {
                Translator.RemoveLanguageChangedListener(UpdateText);
            }

            Toast.ToastDestroyed(this);
        }

        /// <summary>
        /// Handler for disable event.
        /// </summary>
        void OnDisable()
        {
            DebugEx.Verbose("ToastScript.OnDisable()");

            UnityEngine.Object.DestroyObject(gameObject);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
            DebugEx.VeryVeryVerbose("ToastScript.Update()");

            mDelay -= Time.deltaTime;

            if (mDelay <= 0f)
            {
                DestroyToast();
            }
            else
            {
                if (mDelay < FADE_TIME)
                {
                    mToastCanvasGroup.alpha = mDelay / FADE_TIME;
                }
            }
        }

        /// <summary>
        /// Show this toast.
        /// </summary>
        public void Show()
        {
            DebugEx.Verbose("ToastScript.Show()");

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
            Image toastImage = gameObject.AddComponent<Image>();

            toastImage.sprite = Assets.Common.Toasts.Textures.toastBackground.sprite;
            toastImage.type   = Image.Type.Sliced;
            #endregion

            //===========================================================================
            // Image CanvasGroup
            //===========================================================================
            #region CanvasGroup Component
            mToastCanvasGroup = gameObject.AddComponent<CanvasGroup>();

            mToastCanvasGroup.blocksRaycasts = false;
            #endregion

            //***************************************************************************
            // ToastText GameObject
            //***************************************************************************
            #region ToastText GameObject
            GameObject toastTextObject = new GameObject("Text");
            Utils.InitUIObject(toastTextObject, transform);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            RectTransform toastTextTransform = toastTextObject.AddComponent<RectTransform>();
            Utils.AlignRectTransformStretchStretch(
                                                     toastTextTransform
                                                   , toastImage.sprite.border.x
                                                   , toastImage.sprite.border.w
                                                   , toastImage.sprite.border.z
                                                   , toastImage.sprite.border.y
                                                  );
            #endregion

            //===========================================================================
            // Text Component
            //===========================================================================
            #region Text Component
            mToastText = toastTextObject.AddComponent<Text>();

            Assets.Common.Toasts.TextStyles.toastText.Apply(mToastText);

            if (tokenId != R.sections.Toasts.strings.Count)
            {
                Translator.AddLanguageChangedListener(UpdateText);
                UpdateText();
            }
            else
            {
                mToastText.text = text;
            }
            #endregion
            #endregion

            mDelay = duration / 1000f;
            enabled = true;
        }

        /// <summary>
        /// Updates the text.
        /// </summary>
        public void UpdateText()
        {
            DebugEx.Verbose("ToastScript.UpdateText()");

            if (tokenArguments == null || tokenArguments.Length == 0)
            {
                mToastText.text = Translator.GetString(tokenId);
            }
            else
            {
                mToastText.text = Translator.GetString(tokenId, tokenArguments);
            }
        }

        /// <summary>
        /// Destroies the toast.
        /// </summary>
        private void DestroyToast()
        {
            DebugEx.Verbose("ToastScript.DestroyToast()");

            UnityEngine.Object.DestroyObject(gameObject);
        }
    }
}
