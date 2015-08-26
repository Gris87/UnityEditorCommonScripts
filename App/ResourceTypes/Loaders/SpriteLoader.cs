using UnityEngine;



namespace Common.App.ResourceTypes.Loaders
{
    /// <summary>
    /// Class that loads sprite when needed.
    /// </summary>
    public class SpriteLoader
    {
        /// <summary>
        /// Gets the sprite.
        /// </summary>
        /// <value>The sprite.</value>
        public Sprite sprite
        {
            get
            {
                if (mSprite == null)
                {
                    mSprite = AssetUtils.LoadResource<Sprite>(mPath);
                }

                return mSprite;
            }
        }



        private string mPath;
        private Sprite mSprite;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.App.ResourceTypes.Loaders.SpriteLoader"/> class.
        /// </summary>
        /// <param name="path">Path to sprite.</param>
        public SpriteLoader(string path)
        {
            mPath   = path;
            mSprite = null;
        }
    }
}
