using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ManagedReference.Editor
{
    public abstract class BaseReferenceAttributeDrawer : PropertyDrawer
    {
        private static readonly GUIContent
            IsNotManagedReferenceLabel = new("The property type is not manage reference.");

        private static readonly GUIContent NullLabel = new("Null");
        private static List<Type> _customPropertyDrawersCache; 
        
        protected List<Type> Types => _types;
        private bool _hasChanged;

        private List<Type> _types;
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
                }

                GUI.color = Color.white;

                
                var customDrawer = PropertyDrawerHandlers.GetPropertyDrawer(property);
                if(customDrawer != null)
                    customDrawer.OnGUI(position, property, label);
                else
                    EditorGUI.PropertyField(position, property, label, true);
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
                foreach (var type in Types)
                {
                    nodesMenu.AddItem(new GUIContent(type.GetNameWithCategory()), false,
                        x => { OnSelect((Type)x, property); }, type);
                }

            nodesMenu.ShowAsContext();
        }

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
            EditorGUI.GetPropertyHeight(property, true);


        protected void InitTypes(Type type)
        {
            _types = ManagedReferenceExtensions.GetTypes(type);
        }

        protected void InitTypes(Type type, Type genericType)
        {
            _types = ManagedReferenceExtensions.GetTypes(type, genericType);
        }

        protected void InitTypesByAttribute(Type typeAttribute)
        {
            _types = ManagedReferenceExtensions.GetTypesByAttribute(typeAttribute);
        }
    }
}