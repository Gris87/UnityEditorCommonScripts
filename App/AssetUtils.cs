using System;
using System.Collections.Generic;
using UnityEngine;

using Common.App.ResourceTypes;



namespace Common.App
{
    /// <summary>
    /// Class with usefull stuff for asset loading.
    /// </summary>
    public static class AssetUtils
    {
        #region Assets for Fonts
        /// <summary>
        /// Assets for Fonts.
        /// </summary>
        public static class Fonts
        {
            public static Font defaultFont;



            private static Dictionary<string, Font> sFonts;
            private static string[]                 sOsFonts;



            /// <summary>
            /// Initializes the <see cref="Common.App.AssetUtils+Fonts"/> class.
            /// </summary>
            static Fonts()
            {
				DebugEx.Verbose("Static class AssetUtils.Fonts initialized");

                sFonts   = new Dictionary<string, Font>();
                sOsFonts = Font.GetOSInstalledFontNames();

                ResetValues();
            }

            /// <summary>
            /// Resets values.
            /// </summary>
            public static void ResetValues()
            {
				DebugEx.Verbose("AssetUtils.Fonts.ResetValues()");

                defaultFont = AssetUtils.LoadResource<Font>("Common/Fonts/Default");

                sFonts.Clear();

                LoadFonts("Common/Fonts/");
                LoadFonts("Fonts/");
            }

            /// <summary>
            /// Loads fonts from specified path.
            /// </summary>
            /// <param name="path">Path to fonts.</param>
            private static void LoadFonts(string path)
            {
				DebugEx.VerboseFormat("AssetUtils.Fonts.LoadFonts(path = {0})", path);

                Font[] fontList = Resources.LoadAll<Font>(path);

                foreach (Font font in fontList)
                {
                    string[] fontNames = font.fontNames;

                    foreach (string fontName in fontNames)
                    {
                        if (!sFonts.ContainsKey(fontName))
                        {
                            sFonts.Add(fontName, font);
                        }
                        else
                        {
							DebugEx.WarningFormat("Already has a font with name: {0}", fontName);
                        }
                    }
                }
            }

            /// <summary>
            /// Gets the font with specified name.
            /// </summary>
            /// <returns>Font.</returns>
            /// <param name="fontName">Font name.</param>
            /// <param name="fontSize">Font size.</param>
            public static Font GetFont(string fontName, int fontSize = 12)
            {
				DebugEx.VerboseFormat("AssetUtils.Fonts.GetFont(fontName = {0}, fontSize = {1})", fontName, fontSize);

                Font res;

                if (sFonts.TryGetValue(fontName, out res))
                {
                    return res;
                }

                string nameLower = fontName.ToLower();
                string bestFont  = "";

                foreach (string osFont in sOsFonts)
                {
                    if (osFont == fontName)
                    {
                        bestFont = osFont;
                        break;
                    }

                    string osNameLower = osFont.ToLower();

                    if (osNameLower == nameLower)
                    {
                        bestFont = osFont;
                        break;
                    }

                    if (osNameLower.Contains(nameLower))
                    {
                        if (bestFont == "" || osFont.Length < bestFont.Length)
                        {
                            bestFont = osFont;
                        }
                    }
                }

                if (bestFont != "")
                {
                    res = Font.CreateDynamicFontFromOSFont(bestFont, fontSize);
                    sFonts.Add(fontName, res);

                    return res;
                }

                return defaultFont;
            }
        }
        #endregion



        /// <summary>
        /// Loads an asset stored at path in a resources.
        /// </summary>
        /// <returns>The asset at path if it can be found otherwise returns null.</returns>
        /// <param name="path">Pathname of the target asset.</param>
        /// <typeparam name="T">Type of resource.</typeparam>
        public static T LoadResource<T>(string path) where T : UnityEngine.Object
        {
			DebugEx.VerboseFormat("AssetUtils.LoadResource<{0}>(path = {1})", typeof(T).FullName, path);

            T res = Resources.Load<T>(path);

            if (res == null)
            {
				DebugEx.ErrorFormat("Resource \"{0}\" is not found", path);
            }

            return res;
        }

        /// <summary>
        /// Loads texture 2D stored at path in a resources and scales it.
        /// </summary>
        /// <returns>The asset at path if it can be found otherwise returns null.</returns>
        /// <param name="path">Pathname of the target asset.</param>
        public static Texture2D LoadScaledTexture2D(string path)
        {
			DebugEx.VerboseFormat("AssetUtils.LoadScaledTexture2D(path = {0})", path);

            Texture2D res = LoadResource<Texture2D>(path);

            if (res != null && Utils.canvasScale != 1f)
            {
                TextureScale.Point(res, (int)(res.width * Utils.canvasScale), (int)(res.height * Utils.canvasScale));
            }

            return res;
        }

        /// <summary>
        /// Loads color asset stored at path in a resources.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="path">Pathname of the target asset.</param>
        public static Color LoadColor(string path)
        {
			DebugEx.VerboseFormat("AssetUtils.LoadColor(path = {0})", path);

            Color res = new Color(0f, 0f, 0f);

            TextAsset asset = LoadResource<TextAsset>(path);

            if (asset != null)
            {
                IniFile iniFile = new IniFile(asset);
                LoadColorFromIniFile(iniFile, ref res);
            }

            return res;
        }

        /// <summary>
        /// Loads text style asset stored at path in a resources.
        /// </summary>
        /// <returns>The text style.</returns>
        /// <param name="path">Pathname of the target asset.</param>
        public static TextStyle LoadTextStyle(string path)
        {
			DebugEx.VerboseFormat("AssetUtils.LoadTextStyle(path = {0})", path);

            TextAsset asset = LoadResource<TextAsset>(path);

            if (asset == null)
            {
                return null;
            }

            TextStyle res = new TextStyle();
            Color color   = new Color(0f, 0f, 0f);



            IniFile iniFile = new IniFile(asset);
            iniFile.BeginGroup("Font");

            string font        = iniFile.Get("Font",        "Microsoft Sans Serif");
            string fontStyle   = iniFile.Get("FontStyle",   "Normal");
            int    fontSize    = iniFile.Get("FontSize",    12);
            float  lineSpacing = iniFile.Get("LineSpacing", 1);
            string alignment   = iniFile.Get("Alignment",   "UpperLeft");
            LoadColorFromIniFile(iniFile, ref color);

            iniFile.EndGroup();



            res.font = Fonts.GetFont(font, fontSize);

            try
            {
                res.fontStyle = (FontStyle) Enum.Parse(typeof(FontStyle), fontStyle);
            }
            catch (Exception)
            {
				DebugEx.ErrorFormat("Invalid font style value \"{0}\" for text style: {1}", fontStyle, path);
            }

            res.fontSize    = fontSize;
            res.lineSpacing = lineSpacing;

            try
            {
                res.alignment = (TextAnchor) Enum.Parse(typeof(TextAnchor), alignment);
            }
            catch (Exception)
            {
				DebugEx.ErrorFormat("Invalid alignment value \"{0}\" for text style: {1}", alignment, path);
            }

            res.color = color;



            return res;
        }

        /// <summary>
        /// Loads the color from ini file.
        /// </summary>
        /// <param name="iniFile">Ini file.</param>
        /// <param name="color">Result color.</param>
        private static void LoadColorFromIniFile(IniFile iniFile, ref Color color)
        {
			DebugEx.VerboseFormat("AssetUtils.LoadColorFromIniFile(iniFile = {0}, color = {1})", iniFile, color);

            iniFile.BeginGroup("Color");

            color.r = iniFile.Get("Red",   0f);
            color.g = iniFile.Get("Green", 0f);
            color.b = iniFile.Get("Blue",  0f);
            color.a = iniFile.Get("Alpha", 1f);

            iniFile.EndGroup();
        }
    }
}
