namespace Common.UI.MenuItems
{
    /// <summary>
    /// Common menu item.
    /// </summary>
    public class CustomMenuItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Common.UI.MenuItems.CustomMenuItem"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool visible
        {
            get
			{
				DebugEx.VeryVeryVerboseFormat("CustomMenuItem.visible = {0}", mVisible);

				return mVisible;
			}

            set
			{
				DebugEx.VeryVerboseFormat("CustomMenuItem.visible: {0} => {1}", mVisible, value);

				mVisible = value;
			}
        }

        /// <summary>
        /// Gets the assigned <see cref="Common.TreeNode`1"/> instance.
        /// </summary>
        /// <value>The assigned <see cref="Common.TreeNode`1"/> instance.</value>
        public TreeNode<CustomMenuItem> node
        {
            get
			{
				DebugEx.VeryVeryVerboseFormat("CustomMenuItem.node = {0}", mNode);

				return mNode; 
			}
        }



        private bool mVisible;

        /// <summary>
        /// The <see cref="Common.TreeNode`1"/> instance assigned with this menu item.
        /// </summary>
        protected TreeNode<CustomMenuItem> mNode;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.MenuItems.CustomMenuItem"/> class.
        /// </summary>
        public CustomMenuItem()
        {
			DebugEx.Verbose("Created CustomMenuItem object");

            mVisible = true;

            mNode = null;
        }
    }
}
