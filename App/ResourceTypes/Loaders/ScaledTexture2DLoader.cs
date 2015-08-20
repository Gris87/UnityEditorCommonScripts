using UnityEngine;



namespace Common.App.ResourceTypes.Loaders
{
	/// <summary>
	/// Class that loads texture when needed.
	/// </summary>
	public class ScaledTexture2DLoader
	{
		/// <summary>
		/// Gets the texture.
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D texture
		{
			get
			{
				if (mTexture2D == null)
				{
					mTexture2D = AssetUtils.LoadScaledTexture2D(mPath);
				}

				return mTexture2D;
			}
		}



		private string    mPath;
		private Texture2D mTexture2D;



		/// <summary>
		/// Initializes a new instance of the <see cref="Common.App.ResourceTypes.ScaledTexture2DLoader"/> class.
		/// </summary>
		/// <param name="path">Path to texture.</param>
		public ScaledTexture2DLoader(string path)
		{
			mPath      = path;
			mTexture2D = null;
		}
	}
}

