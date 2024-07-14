using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ManagedReference.Editor
{
    public abstract class BaseReferenceAttributeDrawer : PropertyDrawer
    {
        private static readonly GUIContent
            IsNotManagedReferenceLabel = new("The property type is not manage reference.");

        private static readonly GUIContent NullLabel = new("Null");
        private static List<Type> _customPropertyDrawersCache;

        protected Dictionary<Type, List<Type>> Types { get; private set; }
        private bool _hasChanged;

        //private long _lastId;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                Rect dropDownRect = position;
                dropDownRect.width -= EditorGUIUtility.labelWidth;
                dropDownRect.x += EditorGUIUtility.labelWidth;
                dropDownRect.height = EditorGUIUtility.singleLineHeight;

                GUI.color = Color.green;
                if (EditorGUI.DropdownButton(dropDownRect, GetTypeName(property), FocusType.Keyboard))
                {
                    CacheTypes(property);
                    CreateDropdown(property);
                    //CreateAdvancedDropdown(property,dropDownRect);
                }

                GUI.color = Color.white;

                PropertyDrawerHandlers.PropertyField(position, property, label, true);
            }
            else
            {
                EditorGUI.LabelField(position, label, IsNotManagedReferenceLabel);
            }

            //call on validate and Gui changed when change type/changes values etc
            //if (_hasChanged || EditorGUI.EndChangeCheck() || property.managedReferenceId != _lastId)
            if (_hasChanged || EditorGUI.EndChangeCheck())
            {
                GUI.changed = true;
                _hasChanged = false;
                CallOnValidateManagedReference(property);
            }

            EditorGUI.EndProperty();
            //_lastId = property.managedReferenceId;
        }

        private void CreateDropdown(SerializedProperty property)
        {
            GenericMenu nodesMenu = new GenericMenu();
            nodesMenu.AddItem(NullLabel, false, x => { OnSelect((Type)x, property); }, null);
            if (Types != null)
                foreach (var value in Types)
                {
                    nodesMenu.AddItem(new GUIContent(string.Empty), false, x => { }, null);
                    foreach (var type in value.Value)
                    {
                        nodesMenu.AddItem(new GUIContent(type.GetNameWithCategory()), false,
                            x => { OnSelect((Type)x, property); }, type);
                    }
                }

            nodesMenu.ShowAsContext();
        }

        /*
        private void CreateAdvancedDropdown(SerializedProperty property, Rect position)
        {
            var popup = new AdvancedTypePopup(Types, x => { OnSelect(x, property); }, new AdvancedDropdownState());
            popup.Show(position);
        }
        */

        private void OnSelect(Type type, SerializedProperty property)
        {
            property.SetManagedReferenceWithCopyValues(type);
            property.isExpanded = true;
            property.serializedObject.ApplyModifiedProperties();
            _hasChanged = true;
        }

        protected abstract void CacheTypes(SerializedProperty serializedProperty);

        private static void CallOnValidateManagedReference(SerializedProperty property)
        {
            var methodInfo = property.managedReferenceValue?.GetType().GetMethod("OnValidate",
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            methodInfo?.Invoke(property.managedReferenceValue, Array.Empty<object>());
        }

        private GUIContent GetTypeName(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                GUI.color = Color.red;
                return NullLabel;
            }

            Type type = property.GetManagedReferenceType();
            return new GUIContent(type.Name);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            PropertyDrawerHandlers.GetPropertyHeight(property, label);


        protected void InitTypes(Type parentType)
        {
            Types = new() { { parentType, ManagedReferenceExtensions.GetTypes(parentType) } };
        }

        protected void InitTypes(Type parentType, params Type[] genericType)
        {
            Types = genericType
                .Where(x => x != null)
                .Distinct()
                .ToDictionary(k => k, v => ManagedReferenceExtensions.GetTypes(parentType, v));
            //Types = new Dictionary<Type, List<Type>>();
            //foreach (var gType in genericType)
            //{
            //    Types.Add(gType, ManagedReferenceExtensions.GetTypes(parentType, gType));
            //}
        }

        protected void InitTypesByAttribute(Type typeAttribute)
        {
            Types = new() { { typeAttribute, ManagedReferenceExtensions.GetTypesByAttribute(typeAttribute) } };
        }
    }
}