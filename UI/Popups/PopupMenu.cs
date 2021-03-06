using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Common.UI.MenuItems;



namespace Common.UI.Popups
{
    #region Internal namespace
    namespace Internal
    {
        /// <summary>
        /// Popup menu common things.
        /// </summary>
        static class PopupMenuCommon
        {
            public static Navigation  defaultNavigation;
            public static Navigation  noneNavigation;



            /// <summary>
            /// Initializes the <see cref="Common.UI.Popups.Internal.PopupMenuCommon"/> class.
            /// </summary>
            static PopupMenuCommon()
            {
                DebugEx.Verbose("Static class PopupMenuCommon initialized");

                defaultNavigation   = Navigation.defaultNavigation;
                noneNavigation      = new Navigation();
                noneNavigation.mode = Navigation.Mode.None;
            }
        }
    }
    #endregion



    /// <summary>
    /// Popup menu.
    /// </summary>
    public class PopupMenu
    {
        private const float SHADOW_WIDTH     = 5f;
        private const float AUTO_POPUP_DELAY = 500f;



        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>The menu items.</value>
        public TreeNode<CustomMenuItem> items
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("PopupMenu.items = {0}", mItems);

                return mItems;
            }
        }

        /// <summary>
        /// Gets the destroy event handler.
        /// </summary>
        /// <value>The destroy event handler.</value>
        public UnityEvent onDestroy
        {
            get
            {
                DebugEx.VeryVeryVerboseFormat("PopupMenu.onDestroy = {0}", mOnDestroy);

                return mOnDestroy;
            }
        }



        private TreeNode<CustomMenuItem> mItems;
        private UnityEvent               mOnDestroy;

        private GameObject mGameObject;
        private PopupMenu  mChildPopupMenu;



        /// <summary>
        /// Initializes a new instance of the <see cref="Common.UI.Popups.PopupMenu"/> class with specified menu items.
        /// </summary>
        /// <param name="items">Items.</param>
        public PopupMenu(TreeNode<CustomMenuItem> items)
        {
            DebugEx.VerboseFormat("Created PopupMenu(items = {0}) object", items);

            mItems     = items;
            mOnDestroy = new UnityEvent();

            mGameObject     = null;
            mChildPopupMenu = null;
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            DebugEx.Verbose("PopupMenu.Destroy()");

            if (mChildPopupMenu != null)
            {
                mChildPopupMenu.Destroy();
                mChildPopupMenu = null;
            }

            if (mGameObject != null)
            {
                UnityEngine.Object.Destroy(mGameObject);
                mGameObject = null;
            }

            PopupMenuAreaScript.DeregisterPopupMenu(this);

            mOnDestroy.Invoke();
        }

        /// <summary>
        /// Show popup menu at the specified coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="left">Left edge for button of parent popup if present.</param>
        /// <param name="bottom">Bottom edge for button of parent popup if present.</param>
        public void Show(float x, float y, float left = -1, float bottom = -1)
        {
            DebugEx.VerboseFormat("PopupMenu.Show(x = {0}, y = {1}, left = {2}, bottom = {3})", x, y, left, bottom);

            PopupMenuAreaScript.RegisterPopupMenu(this);

            //***************************************************************************
            // PopupMenu GameObject
            //***************************************************************************
            #region PopupMenu GameObject
            mGameObject = new GameObject("PopupMenu");
            Utils.InitUIObject(mGameObject, PopupMenuAreaScript.geometry);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            RectTransform popupMenuTransform = mGameObject.AddComponent<RectTransform>();
            #endregion

            //===========================================================================
            // CanvasRenderer Component
            //===========================================================================
            #region CanvasRenderer Component
            mGameObject.AddComponent<CanvasRenderer>();
            #endregion

            //===========================================================================
            // Image Component
            //===========================================================================
            #region Image Component
            Image popupMenuImage = mGameObject.AddComponent<Image>();

            popupMenuImage.sprite = Assets.Common.Popups.Textures.popupBackground.sprite;
            popupMenuImage.type   = Image.Type.Sliced;
            #endregion

            //***************************************************************************
            // ScrollArea GameObject
            //***************************************************************************
            #region ScrollArea GameObject
            GameObject scrollArea = new GameObject("ScrollArea");
            Utils.InitUIObject(scrollArea, mGameObject.transform);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            RectTransform scrollAreaTransform = scrollArea.AddComponent<RectTransform>();
            Utils.AlignRectTransformStretchStretch(
                                                     scrollAreaTransform
                                                   , popupMenuImage.sprite.border.x
                                                   , popupMenuImage.sprite.border.w
                                                   , popupMenuImage.sprite.border.z
                                                   , popupMenuImage.sprite.border.y
                                                  );
            #endregion

            //***************************************************************************
            // Content GameObject
            //***************************************************************************
            #region Content GameObject
            GameObject scrollAreaContent = new GameObject("Content");
            Utils.InitUIObject(scrollAreaContent, scrollArea.transform);

            //===========================================================================
            // RectTransform Component
            //===========================================================================
            #region RectTransform Component
            RectTransform scrollAreaContentTransform = scrollAreaContent.AddComponent<RectTransform>();
            #endregion

            float contentWidth  = 0f;
            float contentHeight = 0f;

            //===========================================================================
            // Fill content
            //===========================================================================
            #region Fill content
            // Create menu item buttons
            ReadOnlyCollection<TreeNode<CustomMenuItem>> menuItems = mItems.children;

            foreach (TreeNode<CustomMenuItem> menuItem in menuItems)
            {
                if (menuItem.data.visible)
                {
                    if (menuItem.data is MenuSeparatorItem)
                    {
                        //***************************************************************************
                        // Separator GameObject
                        //***************************************************************************
                        #region Separator GameObject
                        GameObject menuSeparator = new GameObject("Separator");
                        Utils.InitUIObject(menuSeparator, scrollAreaContent.transform);

                        //===========================================================================
                        // RectTransform Component
                        //===========================================================================
                        #region RectTransform Component
                        float separatorHeight = 8;

                        RectTransform menuItemSeparatorTransform = menuSeparator.AddComponent<RectTransform>();

                        Utils.AlignRectTransformTopStretch(menuItemSeparatorTransform, separatorHeight, contentHeight, 28f);

                        contentHeight += separatorHeight;
                        #endregion

                        //===========================================================================
                        // Image Component
                        //===========================================================================
                        #region Image Component
                        Image image = menuSeparator.AddComponent<Image>();

                        image.sprite = Assets.Common.Popups.Textures.separator.sprite;
                        #endregion
                        #endregion
                    }
                    else
                    if (menuItem.data is MenuItem)
                    {
                        MenuItem item = menuItem.data as MenuItem;

                        bool hasChildren = menuItem.HasChildren();
                        bool enabled     = item.enabled && (hasChildren || item.onClick != null);

                        //***************************************************************************
                        // Button GameObject
                        //***************************************************************************
                        #region Button GameObject
                        GameObject menuItemButton = new GameObject(item.name);
                        Utils.InitUIObject(menuItemButton, scrollAreaContent.transform);

                        //===========================================================================
                        // RectTransform Component
                        //===========================================================================
                        #region RectTransform Component
                        RectTransform menuItemButtonTransform = menuItemButton.AddComponent<RectTransform>();
                        #endregion

                        //===========================================================================
                        // CanvasRenderer Component
                        //===========================================================================
                        #region CanvasRenderer Component
                        menuItemButton.AddComponent<CanvasRenderer>();
                        #endregion

                        //===========================================================================
                        // Image Component
                        //===========================================================================
                        #region Image Component
                        Image menuItemButtonImage = menuItemButton.AddComponent<Image>();

                        menuItemButtonImage.sprite = Assets.Common.Popups.Textures.button.sprite;
                        menuItemButtonImage.type   = Image.Type.Sliced;
                        #endregion

                        //===========================================================================
                        // Button Component
                        //===========================================================================
                        #region Button Component
                        Button menuItemButtonButton = menuItemButton.AddComponent<Button>();

                        menuItemButtonButton.targetGraphic = menuItemButtonImage;
                        menuItemButtonButton.transition    = Selectable.Transition.SpriteSwap;

                        if (enabled)
                        {
                            menuItemButtonButton.spriteState = Assets.Common.Popups.SpriteStates.button.spriteState;
                            menuItemButtonButton.navigation  = Internal.PopupMenuCommon.defaultNavigation;

                            if (hasChildren)
                            {
                                TreeNode<CustomMenuItem> currentMenuItem = menuItem;

                                menuItemButtonButton.onClick.AddListener(() => OnShowMenuSubItems(currentMenuItem));

                                //===========================================================================
                                // AutoPopupItemScript Component
                                //===========================================================================
                                #region AutoPopupItemScript Component
                                AutoPopupItemScript menuItemButtonAutoPopup = menuItemButton.AddComponent<AutoPopupItemScript>();

                                menuItemButtonAutoPopup.delay = AUTO_POPUP_DELAY;
                                #endregion
                            }
                            else
                            {
                                menuItemButtonButton.onClick.AddListener(item.onClick);

                                if (item.radioGroup != null)
                                {
                                    menuItemButtonButton.onClick.AddListener(() => OnSelectItem(item));
                                }

                                menuItemButtonButton.onClick.AddListener(PopupMenuAreaScript.DestroyAll);
                            }
                        }
                        else
                        {
                            menuItemButtonButton.spriteState = Assets.Common.Popups.SpriteStates.buttonDisabled.spriteState;
                            menuItemButtonButton.navigation  = Internal.PopupMenuCommon.noneNavigation;
                        }
                        #endregion
                        #endregion

                        //***************************************************************************
                        // Text GameObject
                        //***************************************************************************
                        #region Text GameObject
                        GameObject menuItemText = new GameObject("Text");
                        Utils.InitUIObject(menuItemText, menuItemButton.transform);

                        //===========================================================================
                        // RectTransform Component
                        //===========================================================================
                        #region RectTransform Component
                        RectTransform menuItemTextTransform = menuItemText.AddComponent<RectTransform>();
                        Utils.AlignRectTransformStretchStretch(menuItemTextTransform, Assets.Common.Popups.Textures.background.sprite.border.x + 4f);
                        #endregion

                        //===========================================================================
                        // Text Component
                        //===========================================================================
                        #region Text Component
                        Text menuItemTextText = menuItemText.AddComponent<Text>();

                        if (enabled)
                        {
                            Assets.Common.Popups.TextStyles.button.Apply(menuItemTextText);
                        }
                        else
                        {
                            Assets.Common.Popups.TextStyles.buttonDisabled.Apply(menuItemTextText);
                        }

                        menuItemTextText.text = item.text;
                        #endregion
                        #endregion

                        float buttonWidth  = menuItemTextText.preferredWidth + Assets.Common.Popups.Textures.background.sprite.border.x + 16f;
                        float buttonHeight = 22f; // TODO: [Trivial] Report request for prefferedHeight for specified width

                        Utils.AlignRectTransformTopStretch(menuItemButtonTransform, buttonHeight, contentHeight);

                        if (buttonWidth > contentWidth)
                        {
                            contentWidth = buttonWidth;
                        }

                        contentHeight += buttonHeight;
                    }
                    else
                    {
                        DebugEx.Error("Unknown menu item type");
                    }
                }
            }

            //***************************************************************************
            // Shortcuts
            //***************************************************************************
            #region Shortcuts
            float shortcutWidth = 0f;
            int index = 0;

            for (int i = 0; i < menuItems.Count; ++i)
            {
                if (menuItems[i].data.visible)
                {
                    if (menuItems[i].data is MenuItem)
                    {
                        MenuItem item   = menuItems[i].data as MenuItem;
                        string shortcut = item.shortcut;

                        if (shortcut != null)
                        {
                            Transform menuItemButtonTransform = scrollAreaContentTransform.GetChild(index);

                            //***************************************************************************
                            // Text GameObject
                            //***************************************************************************
                            #region Text GameObject
                            GameObject menuItemText = menuItemButtonTransform.GetChild(0).gameObject; // Button/Text
                            GameObject shortcutText = UnityEngine.Object.Instantiate(menuItemText) as GameObject;

                            Utils.InitUIObject(shortcutText, menuItemButtonTransform);
                            shortcutText.name = "Shortcut";

                            //===========================================================================
                            // Text Component
                            //===========================================================================
                            #region Text Component
                            Text text      = shortcutText.GetComponent<Text>();

                            text.text      = shortcut;
                            text.alignment = TextAnchor.MiddleRight;

                            float shortcutTextWidth = text.preferredWidth;

                            if (shortcutTextWidth > shortcutWidth)
                            {
                                shortcutWidth = shortcutTextWidth;
                            }
                            #endregion
                            #endregion
                        }
                    }

                    ++index;
                }
            }

            if (shortcutWidth > 0f)
            {
                index = 0;

                for (int i = 0; i < menuItems.Count; ++i)
                {
                    if (menuItems[i].data.visible)
                    {
                        if (menuItems[i].data is MenuItem)
                        {
                            MenuItem item   = menuItems[i].data as MenuItem;
                            string shortcut = item.shortcut;

                            if (shortcut != null)
                            {
                                Transform menuItemButtonTransform = scrollAreaContentTransform.GetChild(index);

                                //***************************************************************************
                                // Text GameObject
                                //***************************************************************************
                                #region Text GameObject
                                GameObject menuItemText = menuItemButtonTransform.GetChild(0).gameObject; // Button/Text
                                GameObject shortcutText = menuItemButtonTransform.GetChild(1).gameObject; // Button/Text

                                //===========================================================================
                                // RectTransform Component
                                //===========================================================================
                                #region RectTransform Component
                                RectTransform menuItemTextTransform = menuItemText.transform as RectTransform;

                                menuItemTextTransform.offsetMax = new Vector2(-shortcutWidth, 0f);
                                #endregion

                                //===========================================================================
                                // RectTransform Component
                                //===========================================================================
                                #region RectTransform Component
                                RectTransform shortcutTextTransform = shortcutText.transform as RectTransform;

                                Utils.AlignRectTransformStretchRight(shortcutTextTransform, shortcutWidth, 4);
                                #endregion
                                #endregion
                            }
                        }

                        ++index;
                    }
                }

                contentWidth += shortcutWidth + 8;
            }
            #endregion

            //***************************************************************************
            // Arrow
            //***************************************************************************
            #region Arrow
            float arrowWidth = 0f;

            for (int i = 0; i < menuItems.Count; ++i)
            {
                if (menuItems[i].data.visible)
                {
                    if (menuItems[i].data is MenuItem)
                    {
                        if (menuItems[i].HasChildren())
                        {
                            arrowWidth = 16f;

                            break;
                        }
                    }
                }
            }

            if (arrowWidth > 0f)
            {
                index = 0;

                for (int i = 0; i < menuItems.Count; ++i)
                {
                    if (menuItems[i].data.visible)
                    {
                        if (menuItems[i].data is MenuItem)
                        {
                            Transform menuItemButtonTransform = scrollAreaContentTransform.GetChild(index);

                            //***************************************************************************
                            // Text GameObject
                            //***************************************************************************
                            #region Text GameObject
                            GameObject menuItemText = menuItemButtonTransform.GetChild(0).gameObject; // Button/Text
                            GameObject shortcutText = null;

                            if (menuItemButtonTransform.childCount > 1)
                            {
                                shortcutText = menuItemButtonTransform.GetChild(1).gameObject; // Button/Text
                            }

                            //===========================================================================
                            // RectTransform Component
                            //===========================================================================
                            #region RectTransform Component
                            RectTransform menuItemTextTransform = menuItemText.transform as RectTransform;

                            menuItemTextTransform.offsetMax = new Vector2(menuItemTextTransform.offsetMax.x - arrowWidth, 0f);
                            #endregion

                            if (shortcutText != null)
                            {
                                //===========================================================================
                                // RectTransform Component
                                //===========================================================================
                                #region RectTransform Component
                                RectTransform shortcutTextTransform = shortcutText.transform as RectTransform;

                                shortcutTextTransform.offsetMin = new Vector2(shortcutTextTransform.offsetMin.x - arrowWidth, 0f);
                                shortcutTextTransform.offsetMax = new Vector2(shortcutTextTransform.offsetMax.x - arrowWidth, 0f);
                                #endregion
                            }
                            #endregion

                            if (menuItems[i].HasChildren())
                            {
                                //***************************************************************************
                                // Image GameObject
                                //***************************************************************************
                                #region Image GameObject
                                GameObject arrow = new GameObject("Arrow");
                                Utils.InitUIObject(arrow, menuItemButtonTransform);

                                //===========================================================================
                                // RectTransform Component
                                //===========================================================================
                                #region RectTransform Component
                                RectTransform arrowTransform = arrow.AddComponent<RectTransform>();

                                Utils.AlignRectTransformStretchRight(arrowTransform, arrowWidth, 4, 3, 3);
                                #endregion

                                //===========================================================================
                                // CanvasRenderer Component
                                //===========================================================================
                                #region CanvasRenderer Component
                                arrow.AddComponent<CanvasRenderer>();
                                #endregion

                                //===========================================================================
                                // Image Component
                                //===========================================================================
                                #region Image Component
                                Image arrowImage = arrow.AddComponent<Image>();

                                arrowImage.sprite = Assets.Common.Popups.Textures.arrow.sprite;
                                arrowImage.type   = Image.Type.Sliced;
                                #endregion
                                #endregion
                            }
                        }

                        ++index;
                    }
                }

                contentWidth += arrowWidth + 8;
            }
            #endregion

            //***************************************************************************
            // Checkbox
            //***************************************************************************
            #region Checkbox
            float checkboxWidth = 22f;
            index = 0;

            for (int i = 0; i < menuItems.Count; ++i)
            {
                if (menuItems[i].data.visible)
                {
                    if (menuItems[i].data is MenuItem)
                    {
                        MenuItem item = menuItems[i].data as MenuItem;

                        if (item.radioGroup != null && item.radioGroup.selectedItem == item)
                        {
                            Transform menuItemButtonTransform = scrollAreaContentTransform.GetChild(index);

                            //***************************************************************************
                            // Image GameObject
                            //***************************************************************************
                            #region Image GameObject
                            GameObject checkbox = new GameObject("Checkbox");
                            Utils.InitUIObject(checkbox, menuItemButtonTransform);

                            //===========================================================================
                            // RectTransform Component
                            //===========================================================================
                            #region RectTransform Component
                            RectTransform checkboxTransform = checkbox.AddComponent<RectTransform>();

                            Utils.AlignRectTransformStretchLeft(checkboxTransform, checkboxWidth);
                            #endregion

                            //===========================================================================
                            // CanvasRenderer Component
                            //===========================================================================
                            #region CanvasRenderer Component
                            checkbox.AddComponent<CanvasRenderer>();
                            #endregion

                            //===========================================================================
                            // Image Component
                            //===========================================================================
                            #region Image Component
                            Image checkboxImage = checkbox.AddComponent<Image>();

                            checkboxImage.sprite = Assets.Common.Popups.Textures.checkbox.sprite;
                            checkboxImage.type   = Image.Type.Sliced;
                            #endregion
                            #endregion
                        }
                    }

                    ++index;
                }
            }
            #endregion
            #endregion

            Utils.AlignRectTransformTopStretch(scrollAreaContentTransform, contentHeight);
            #endregion

            //===========================================================================
            // ScrollRect Component
            //===========================================================================
            #region ScrollRect Component
            ScrollRect scrollAreaScrollRect = scrollArea.AddComponent<ScrollRect>();

            scrollAreaScrollRect.content    = scrollAreaContentTransform;
            scrollAreaScrollRect.horizontal = false;
            #endregion

            //===========================================================================
            // CanvasRenderer Component
            //===========================================================================
            #region CanvasRenderer Component
            scrollArea.AddComponent<CanvasRenderer>();
            #endregion

            //===========================================================================
            // Image Component
            //===========================================================================
            #region Image Component
            Image scrollAreaImage = scrollArea.AddComponent<Image>();

            scrollAreaImage.sprite = Assets.Common.Popups.Textures.background.sprite;
            scrollAreaImage.type   = Image.Type.Sliced;
            #endregion

            //===========================================================================
            // Mask Component
            //===========================================================================
            #region Mask Component
            scrollArea.AddComponent<Mask>();
            #endregion
            #endregion

            float popupMenuWidth  = contentWidth  + 11;
            float popupMenuHeight = contentHeight + 11;

            Utils.FitRectTransformToScreen(popupMenuTransform, popupMenuWidth, popupMenuHeight, x, y, left, bottom, SHADOW_WIDTH, SHADOW_WIDTH);
            #endregion
        }

        /// <summary>
        /// Handler for menu item selection.
        /// </summary>
        /// <param name="item">Item.</param>
        public void OnSelectItem(MenuItem item)
        {
            DebugEx.VerboseFormat("PopupMenu.OnSelectItem(item = {0})", item);

            if (item.radioGroup != null)
            {
                item.radioGroup.selectedItem = item;
            }
            else
            {
                DebugEx.Fatal("Unexpected behaviour in PopupMenu.OnSelectItem()");
            }
        }

        /// <summary>
        /// Creates and displays popup menu for specified menu item.
        /// </summary>
        /// <param name="node"><see cref="Common.TreeNode`1"/> instance.</param>
        public void OnShowMenuSubItems(TreeNode<CustomMenuItem> node)
        {
            DebugEx.VerboseFormat("PopupMenu.OnShowMenuSubItems(node = {0})", node);

            if (node.data is MenuItem)
            {
                MenuItem item = node.data as MenuItem;
                DebugEx.UserInteractionFormat("PopupMenu.OnShowMenuSubItems({0})", item.name);

                if (mChildPopupMenu != null)
                {
                    mChildPopupMenu.Destroy();
                }

                mChildPopupMenu = new PopupMenu(node);
                mChildPopupMenu.mOnDestroy.AddListener(OnPopupMenuDestroyed);

                int index = node.parent.children.IndexOf(node);

                RectTransform menuItemTransform = mGameObject.transform.GetChild(0).GetChild(0).GetChild(index).transform as RectTransform; // ScrollArea/Content/NODE
                Vector3[] menuItemCorners = Utils.GetWindowCorners(menuItemTransform);

                mChildPopupMenu.Show(
                                     menuItemCorners[1].x,
                                     menuItemCorners[1].y,
                                     menuItemCorners[2].x,
                                     menuItemCorners[2].y
                                    );
            }
            else
            {
                DebugEx.Error("Unknown menu item type");
            }
        }

        /// <summary>
        /// Handler for popup menu destroy event.
        /// </summary>
        public void OnPopupMenuDestroyed()
        {
            DebugEx.Verbose("PopupMenu.OnPopupMenuDestroyed()");

            mChildPopupMenu = null;
        }
    }
}
