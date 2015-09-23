using UnityEngine;
using UnityEngine.UI;



namespace Common.App.ResourceTypes
{
    /// <summary>
    /// Text style.
    /// </summary>
    public class TextStyle
    {
        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>Font.</value>
        public Font font
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TextStyle.font = {0}", mFont);

                return mFont;
            }

            set
            {
                DebugEx.VeryVerboseFormat("TextStyle.font: {0} => {1}", mFont, value);

                mFont = value;
            }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public FontStyle fontStyle
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TextStyle.fontStyle = {0}", mFontStyle);

                return mFontStyle;
            }

            set
            {
                DebugEx.VeryVerboseFormat("TextStyle.fontStyle: {0} => {1}", mFontStyle, value);

                mFontStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public int fontSize
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TextStyle.fontSize = {0}", mFontSize);

                return mFontSize;
            }

            set
            {
                DebugEx.VeryVerboseFormat("TextStyle.fontSize: {0} => {1}", mFontSize, value);

                if (value < 0)
                {
                    value = 0;
                }

                mFontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the line spacing.
        /// </summary>
        /// <value>The line spacing.</value>
        public float lineSpacing
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TextStyle.lineSpacing = {0}", mLineSpacing);

                return mLineSpacing;
            }

            set
            {
                DebugEx.VeryVerboseFormat("TextStyle.lineSpacing: {0} => {1}", mLineSpacing, value);

                if (value < 0f)
                {
                    value = 0f;
                }

                mLineSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        /// <value>The alignment.</value>
        public TextAnchor alignment
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TextStyle.alignment = {0}", mAlignment);

                return mAlignment;
            }

            set
            {
                DebugEx.VeryVerboseFormat("TextStyle.alignment: {0} => {1}", mAlignment, value);

                mAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the font color.
        /// </summary>
        /// <value>Font color.</value>
        public Color color
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TextStyle.color = {0}", mColor);

                return mColor;
            }

            set
            {
                DebugEx.VeryVerboseFormat("TextStyle.color: {0} => {1}", mColor, value);

                mColor = value;
            }
        }



        private Font       mFont;
        private FontStyle  mFontStyle;
        private int        mFontSize;
        private float      mLineSpacing;
        private TextAnchor mAlignment;
        private Color      mColor;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.App.ResourceTypes.TextStyle"/> class.
        /// </summary>
        public TextStyle()
        {
            DebugEx.Verbose("Created TextStyle object");

            mFont        = null;
            mFontStyle   = FontStyle.Normal;
            mFontSize    = 12;
            mLineSpacing = 1f;
            mAlignment   = TextAnchor.UpperLeft;
            mColor       = new Color(0f, 0f, 0f);
        }

        /// <summary>
        /// Apply text style to specified text component.
        /// </summary>
        /// <param name="text">Text component.</param>
        public void Apply(Text text)
        {
            DebugEx.VerboseFormat("TextStyle.Apply(text = {0})", text);

            text.font        = mFont;
            text.fontStyle   = mFontStyle;
            text.fontSize    = mFontSize;
            text.lineSpacing = mLineSpacing;
            text.alignment   = mAlignment;
            text.color       = mColor;
        }
    }
}
