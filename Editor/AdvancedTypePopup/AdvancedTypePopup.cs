using System;
using System.Collections.Generic;
using System.Linq;
using ManagedReference.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManagedReference.Editor
{
    public class AdvancedTypePopupItem : AdvancedDropdownItem
    {
        public Type Type { get; }

        public AdvancedTypePopupItem(Type type, string name) : base(name)
        {
            Type = type;
        }
    }

    /// <summary>
    /// A type popup with a fuzzy finder.
    /// </summary>
    public class AdvancedTypePopup : AdvancedDropdown
    {
        private static readonly string NullLabel = "Null";

        private readonly Dictionary<Type, List<Type>> _types;
        private readonly Action<Type> _onSelect;

        public AdvancedTypePopup(Dictionary<Type, List<Type>> types,Action<Type> onSelect, AdvancedDropdownState state) : base(state)
        {
            _types = types;
            _onSelect = onSelect;
            minimumSize = new Vector2(minimumSize.x, EditorGUIUtility.singleLineHeight * 2f);
        }

        private static void AddTo(AdvancedDropdownItem root, Dictionary<Type, List<Type>> types)
        {
            // Add null item.
            List<AdvancedDropdownItem> items = new();
            root.AddChild(new AdvancedTypePopupItem(null, NullLabel));
            root.AddSeparator();

            foreach (var mainType in types)
            {
                foreach (var type in mainType.Value.OrderByDescending(x=> x.GetCategory()))
                {
                    string category = type.GetCategory();
                    if (string.IsNullOrEmpty(category))
                    {
                        items.Add(new AdvancedTypePopupItem(type, type.Name));
                    }
                    else
                    {
                        var categoryGroup = GetOrCreateCategoryGroup(items, category);
                        categoryGroup.AddChild(new AdvancedTypePopupItem(type, type.Name));
                    }
                }

                foreach (var item in items.OrderBy(x => x.name))
                {
                    root.AddChild(item);
                }
                root.AddSeparator();
            }
        }

        private static AdvancedDropdownItem GetOrCreateCategoryGroup(List<AdvancedDropdownItem> items, string category)
        {
            var item = items.Find(x => x.name == category);
            if (item == null)
            {
                item = new AdvancedDropdownItem(category);
                items.Add(item);
            }
            return item;
        }

        static AdvancedDropdownItem GetItem(AdvancedDropdownItem parent, string name)
        {
            foreach (AdvancedDropdownItem item in parent.children)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            return null;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Select Type");
            AddTo(root, _types);
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is AdvancedTypePopupItem typePopupItem)
            {
                _onSelect?.Invoke(typePopupItem.Type);
            }
        }
    }
}