using System.Collections.Generic;
using UnityEngine;



namespace Common.UI.MenuItems
{
    /// <summary>
    /// Menu radio group.
    /// </summary>
    public class MenuRadioGroup
    {
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public MenuItem selectedItem
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("MenuRadioGroup.selectedItem = {0}", mSelectedItem);

                return mSelectedItem;
            }

            set
            {
                DebugEx.VeryVerboseFormat("MenuRadioGroup.selectedItem: {0} => {1}", mSelectedItem, value);

                if (mSelectedItem != value)
                {
                    if (value.radioGroup == this)
                    {
                        mSelectedItem = value;
                    }
                    else
                    {
                        DebugEx.ErrorFormat("Trying to select item \"{0}\" that is not registered in this radio group", value.name);
                    }
                }
            }
        }



        private List<MenuItem> mItems;
        private MenuItem       mSelectedItem;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.MenuItems.MenuRadioGroup"/> class.
        /// </summary>
        public MenuRadioGroup()
        {
            DebugEx.Verbose("Created MenuRadioGroup object");

            mItems        = new List<MenuItem>();
            mSelectedItem = null;
        }

        /// <summary>
        /// Register the specified menu item to radio group.
        /// </summary>
        /// <param name="item">Menu item.</param>
        public void Register(MenuItem item)
        {
            DebugEx.VerboseFormat("MenuRadioGroup.Register(item = {0})", item);

            if (item.radioGroup == null)
            {
                mItems.Add(item);

                item.radioGroup = this;

                if (mItems.Count == 1)
                {
                    mSelectedItem = item;
                }
            }
            else
            {
                DebugEx.ErrorFormat("Item \"{0}\" already registered in radio group", item.name);
            }
        }

        /// <summary>
        /// Deregister the specified menu item from radio group.
        /// </summary>
        /// <param name="item">Menu item.</param>
        public void Deregister(MenuItem item)
        {
            DebugEx.VerboseFormat("MenuRadioGroup.Deregister(item = {0})", item);

            if (item.radioGroup == this)
            {
                if (mItems.Remove(item))
                {
                    item.radioGroup = null;

                    if (mSelectedItem == item)
                    {
                        if (mItems.Count == 0)
                        {
                            mSelectedItem = null;
                        }
                        else
                        {
                            mSelectedItem = mItems[0];
                        }
                    }
                }
                else
                {
                    DebugEx.ErrorFormat("Failed to deregister item \"{0}\" from radio froup", item.name);
                }
            }
            else
            {
                DebugEx.ErrorFormat("Item \"{0}\" is not registered in this radio group", item.name);
            }
        }
    }
}
