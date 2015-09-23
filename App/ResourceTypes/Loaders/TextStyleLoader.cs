using UnityEngine.UI;



namespace Common.App.ResourceTypes.Loaders
{
    /// <summary>
    /// Class that loads text style when needed.
    /// </summary>
    public class TextStyleLoader
    {
        private string    mPath;
        private TextStyle mTextStyle;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.App.ResourceTypes.Loaders.TextStyleLoader"/> class.
        /// </summary>
        /// <param name="path">Path to text style.</param>
        public TextStyleLoader(string path)
        {
			DebugEx.VerboseFormat("Created TextStyleLoader(path = {0}) object", path);

            mPath      = path;
            mTextStyle = null;
        }

        /// <summary>
        /// Apply text style to specified text component.
        /// </summary>
        /// <param name="text">Text component.</param>
        public void Apply(Text text)
        {
			DebugEx.VerboseFormat("TextStyleLoader.Apply(text = {0})", text);

            if (mTextStyle == null)
            {
                mTextStyle = AssetUtils.LoadTextStyle(mPath);
            }

            mTextStyle.Apply(text);
        }
    }
}
