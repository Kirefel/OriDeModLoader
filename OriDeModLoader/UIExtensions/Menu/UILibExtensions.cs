using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace OriDeModLoader.UIExtensions
{
    internal static class UILibExtensions
    {
        internal static void AddMenuItem(this CleverMenuItemSelectionManager selectionManager, string label, Action onPress)
        {
            selectionManager.AddMenuItem(label, selectionManager.MenuItems.Count - 1, onPress);
        }

        internal static void AddMenuItem(this CleverMenuItemSelectionManager selectionManager, string label, int index, Action onPress)
        {
            CleverMenuItemLayout component = selectionManager.gameObject.GetComponent<CleverMenuItemLayout>();
            if (component != null)
            {
                selectionManager.AddMenuItem(label, index, component, onPress);
                return;
            }
        }

        internal static void AddMenuItem(this CleverMenuItemSelectionManager selectionManager, string label, int index, CleverMenuItemLayout layout, Action onPress)
        {
            CleverMenuItem cleverMenuItem = UnityEngine.Object.Instantiate<CleverMenuItem>(selectionManager.MenuItems[0]);
            cleverMenuItem.gameObject.name = label;
            cleverMenuItem.transform.SetParent(selectionManager.MenuItems[1].transform.parent);
            if (onPress != null)
                cleverMenuItem.PressedCallback += onPress;
            cleverMenuItem.gameObject.GetComponentInChildren<MessageBox>().SetMessage(new MessageDescriptor(label));
            cleverMenuItem.ApplyColors();
            selectionManager.MenuItems.Insert(index, cleverMenuItem);
            layout.AddItem(cleverMenuItem, index);
        }

        internal static void AddItem(this CleverMenuItemLayout layout, CleverMenuItem item)
        {
            layout.MenuItems.Add(item);
            layout.Sort();
        }

        internal static void AddItem(this CleverMenuItemLayout layout, CleverMenuItem item, int index)
        {
            layout.MenuItems.Insert(index, item);
            layout.Sort();
        }

        internal static void AddItem(this CleverMenuItemGroup group, CleverMenuItem item, CleverMenuItemGroupBase itemGroup)
        {
            CleverMenuItemGroup.CleverMenuItemGroupItem cleverMenuItemGroupItem = new CleverMenuItemGroup.CleverMenuItemGroupItem
            {
                ItemGroup = itemGroup,
                MenuItem = item
            };
            cleverMenuItemGroupItem.ItemGroup.IsActive = false;
            itemGroup.OnBackPressed = (Action)Delegate.Combine(itemGroup.OnBackPressed, new Action(group.OnOptionBackPressed));
            group.Options.Add(cleverMenuItemGroupItem);
        }

        internal static void Reset(this TransparencyAnimator animator)
        {
            var childTransparencyAnimators = Traverse.Create(animator).Field("m_childTransparencyAnimators").GetValue<IList>();
            var cleverMenuItems = Traverse.Create(animator).Field("m_cleverMenuItems").GetValue<IList>();
            var rendererData = Traverse.Create(animator).Field("m_rendererData").GetValue<IList>();
            var renderers = Traverse.Create(animator).Field("m_renderers").GetValue<HashSet<Renderer>>();

            if (childTransparencyAnimators != null)
                childTransparencyAnimators.Clear();
            if (cleverMenuItems != null)
                cleverMenuItems.Clear();
            if (rendererData != null)
                rendererData.Clear();
            if (renderers != null)
                renderers.Clear();
        }
    }
}
