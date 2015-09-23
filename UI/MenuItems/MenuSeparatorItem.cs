namespace Common.UI.MenuItems
{
    /// <summary>
    /// Menu separator item.
    /// </summary>
    public class MenuSeparatorItem : CustomMenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.MenuItems.MenuSeparatorItem"/> class.
        /// </summary>
        private MenuSeparatorItem()
            : base()
        {
			DebugEx.Verbose("Created MenuSeparatorItem object");
        }

        /// <summary>
        /// Creates <see cref="Common.UI.MenuItems.MenuSeparatorItem"/> instance that representing separator and adds it to
        /// <see cref="Common.TreeNode`1"/> instance.
        /// </summary>
        /// <param name="owner"><see cref="Common.TreeNode`1"/> instance.</param>
        public static TreeNode<CustomMenuItem> Create(TreeNode<CustomMenuItem> owner)
        {
			DebugEx.VerboseFormat("MenuSeparatorItem.Create(owner = {0})", owner);

            MenuSeparatorItem        item = new MenuSeparatorItem();
            TreeNode<CustomMenuItem> node = owner.AddChild(item);

            item.mNode = node;

            return node;
        }
    }
}
