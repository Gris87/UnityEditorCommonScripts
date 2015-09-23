using UnityEngine.UI;



namespace Common.App.ResourceTypes.Loaders
{
    /// <summary>
    /// Class that loads state sprites when needed.
    /// </summary>
    public class SpriteStateLoader
    {
        /// <summary>
        /// Gets state sprites.
        /// </summary>
        /// <value>State sprites.</value>
        public SpriteState spriteState
        {
            get
            {
                if (!mInitialized)
                {
                    mSpriteState = new SpriteState();

                    mSpriteState.disabledSprite    = mDisabled.sprite;
                    mSpriteState.highlightedSprite = mHighlighted.sprite;
                    mSpriteState.pressedSprite     = mPressed.sprite;

                    mInitialized = true;
                }

                DebugEx.VeryVeryVerboseFormat("SpriteStateLoader.spriteState = {0}", mSpriteState);

                return mSpriteState;
            }
        }



        private SpriteLoader mDisabled;
        private SpriteLoader mHighlighted;
        private SpriteLoader mPressed;
        private SpriteState  mSpriteState;
        private bool         mInitialized;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.App.ResourceTypes.Loaders.SpriteStateLoader"/> class.
        /// </summary>
        /// <param name="disabled">Disabled sprite.</param>
        /// <param name="highlighted">Highlighted sprite.</param>
        /// <param name="pressed">Pressed sprite.</param>
        public SpriteStateLoader(SpriteLoader disabled, SpriteLoader highlighted, SpriteLoader pressed)
        {
            DebugEx.VerboseFormat("Created SpriteStateLoader(disabled = {0}, highlighted = {1}, pressed = {2}) object", disabled, highlighted, pressed);

            mDisabled    = disabled;
            mHighlighted = highlighted;
            mPressed     = pressed;
            mInitialized = false;
        }
    }
}
