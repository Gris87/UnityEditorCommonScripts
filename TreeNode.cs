using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace Common
{
    /// <summary>
    /// Class that representing tree node with children.
    /// </summary>
    public class TreeNode<T>
    {
        /// <summary>
        /// Gets or sets the data value.
        /// </summary>
        /// <value>The data value.</value>
        public T data
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TreeNode<T>.data = {0}", mData);

                return mData;
            }

            set
            {
                DebugEx.VeryVerboseFormat("TreeNode<T>.data: {0} => {1}", mData, value);

                mData = value;
            }
        }

        /// <summary>
        /// Gets the parent <see cref="Common.TreeNode`1"/> instance.
        /// </summary>
        /// <value>The parent <see cref="Common.TreeNode`1"/> instance.</value>
        public TreeNode<T> parent
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("TreeNode<T>.parent = {0}", mParent);

                return mParent;
            }
        }

        /// <summary>
        /// Gets the children list.
        /// </summary>
        /// <value>The children list.</value>
        public ReadOnlyCollection<TreeNode<T>> children
        {
            get
            {
                if (mChildren == null)
                {
                    DebugEx.VeryVeryVerbose("TreeNode<T>.children = null");

                    return null;
                }

                DebugEx.VeryVeryVerboseFormat("TreeNode<T>.children = List({0})", mChildren.Count);

                return mChildren.AsReadOnly();
            }
        }



        private T                 mData;
        private TreeNode<T>       mParent;
        private List<TreeNode<T>> mChildren;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.TreeNode`1"/> class with specified data.
        /// </summary>
        /// <param name="data">Data value.</param>
        public TreeNode(T data)
        {
            DebugEx.VerboseFormat("Created TreeNode<T>(data = {0}) object", data);

            mData     = data;
            mParent   = null;
            mChildren = null;
        }

        /// <summary>
        /// Creates a new <see cref="Common.TreeNode`1"/> instance and adds it to the children list.
        /// </summary>
        /// <returns>The child <see cref="Common.TreeNode`1"/> instance.</returns>
        /// <param name="data">Data value.</param>
        public TreeNode<T> AddChild(T data)
        {
            DebugEx.VerboseFormat("TreeNode<T>.AddChild(data = {0})", data);

            if (mChildren == null)
            {
                mChildren = new List<TreeNode<T>>();
            }

            TreeNode<T> node = new TreeNode<T>(data);

            node.mParent = this;
            mChildren.Add(node);

            return node;
        }

        /// <summary>
        /// Creates a new <see cref="Common.TreeNode`1"/> instance for each child and adds them to the children list.
        /// </summary>
        /// <returns>The array of <see cref="Common.TreeNode`1"/> instances.</returns>
        /// <param name="values">Data values.</param>
        public TreeNode<T>[] AddChildren(params T[] values)
        {
            DebugEx.VerboseFormat("TreeNode<T>.AddChildren(values = T[{0}])", values.Length);

            TreeNode<T>[] res = new TreeNode<T>[values.Length];

            for (int i = 0; i < values.Length; ++i)
            {
                res[i] = AddChild(values[i]);
            }

            return res;
        }

        /// <summary>
        /// Removes child from the children list.
        /// </summary>
        /// <returns><c>true</c>, if child was removed, <c>false</c> otherwise.</returns>
        /// <param name="node">The child <see cref="Common.TreeNode`1"/> instance.</param>
        public bool RemoveChild(TreeNode<T> node)
        {
            DebugEx.VerboseFormat("TreeNode<T>.RemoveChild(node = {0})", node);

            if (mChildren == null)
            {
                return false;
            }

            bool res = mChildren.Remove(node);

            if (res)
            {
                node.mParent = null;

                if (mChildren.Count == 0)
                {
                    mChildren = null;
                }
            }

            return res;
        }

        /// <summary>
        /// Determines whether this instance has children.
        /// </summary>
        /// <returns><c>true</c> if this instance has children; otherwise, <c>false</c>.</returns>
        public bool HasChildren()
        {
            DebugEx.Verbose("TreeNode<T>.HasChildren()");

            return (mChildren != null && mChildren.Count > 0);
        }
    }
}
