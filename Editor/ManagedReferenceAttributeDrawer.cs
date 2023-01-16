using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ManagedReference.Editor
{
    [CustomPropertyDrawer(typeof(ManagedReferenceAttribute))]
    public class ManagedReferenceAttributeDrawer : PropertyDrawer
    {
        static readonly GUIContent _isNotManagedReferenceLabel =
            new GUIContent("The property type is not manage reference.");

        static readonly GUIContent _nullLabel = new GUIContent("Null");

        protected List<Type> types = null;

        private bool _hasChanged = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                if (types == null)
                {
                    var managedAttribute = attribute as ManagedReferenceAttribute;
                    if (managedAttribute.customAttribute == null)
                    {
                        Type targetType = property.GetManagedReferenceFieldType();
                        if (managedAttribute.genericAttribute)
                        {
                            InitTypes(targetType,
                                managedAttribute.genericType ?? property.GenericTargetTypeArgumentDeep(managedAttribute.genericAttributeOrder)); 
                        }
                        else
                            InitTypes(targetType);
                    }
                    else
                    {
                        InitTypesByAttribute(managedAttribute.customAttribute);
                    }
                }

                Rect dropDownRect = position;
                dropDownRect.width -= EditorGUIUtility.labelWidth;
                dropDownRect.x += EditorGUIUtility.labelWidth;
                dropDownRect.height = EditorGUIUtility.singleLineHeight;

                if (EditorGUI.DropdownButton(dropDownRect, GetTypeName(property), FocusType.Keyboard))
                {
                    //Debug.Log($"{property.displayName} DropdownButton");
                    AddDropdown(property);
                }

                ChangeNameInArraysContent(ref label, property);
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                EditorGUI.LabelField(position, label, _isNotManagedReferenceLabel);
            }

            if (_hasChanged)
            {
                GUI.changed = true;
                _hasChanged = false;
            }

            EditorGUI.EndProperty();
        }

        private void ChangeNameInArraysContent(ref GUIContent content, SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
                return;

            Type type = property.GetManagedReferenceType();
            content.text = type.Name;
        }

        private GUIContent GetTypeName(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                GUI.backgroundColor = Color.red;
                return _nullLabel;
            }

            Type type = property.GetManagedReferenceType();
            return new GUIContent(type.Name);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        private void AddDropdown(SerializedProperty property)
        {
            GenericMenu nodesMenu = new GenericMenu();
            nodesMenu.AddItem(_nullLabel, false, (x) => { OnSelect((Type)x, property); }, null);
            foreach (var type in this.types)
            {
                ManagedReferenceGroupAttribute group =
                    (ManagedReferenceGroupAttribute)Attribute.GetCustomAttribute(type,
                        typeof(ManagedReferenceGroupAttribute));

                string name = group == null ? type.Name : $"{group.name}/{type.Name}";
                nodesMenu.AddItem(new GUIContent(name), false, (x) => { OnSelect((Type)x, property); }, type);
            }

            nodesMenu.ShowAsContext();
        }

        private void OnSelect(Type type, SerializedProperty property)
        {
            //Debug.Log($"{_TargetProperty.displayName} OnSelect");

            property.SetManagedReference((Type)type);
            property.isExpanded = true;
            property.serializedObject.ApplyModifiedProperties();
            _hasChanged = true;
        }

        private void InitTypes(Type type)
        {
            types = TypeCache
                .GetTypesDerivedFrom(type)
                .Where(p =>
                    (p.IsPublic || p.IsNestedPublic) &&
                    !p.IsAbstract &&
                    !p.IsGenericType &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(p) &&
                    Attribute.IsDefined(p, typeof(SerializableAttribute)))
                .ToList();
        }

        private void InitTypes(Type type, Type genericType)
        {
            types = TypeCache
                .GetTypesDerivedFrom(type)
                .Where(p =>
                    (p.IsPublic || p.IsNestedPublic) &&
                    !p.IsAbstract &&
                    p.ContainsGenericInterfaceTypeArgumentDeep(genericType)&&
                    !typeof(UnityEngine.Object).IsAssignableFrom(p) &&
                    Attribute.IsDefined(p, typeof(SerializableAttribute)))
                .ToList();
        }

        private void InitTypesByAttribute(Type typeAttribute)
        {
            types = TypeCache
                .GetTypesWithAttribute(typeAttribute)
                .Where(p =>
                    (p.IsPublic || p.IsNestedPublic) &&
                    !p.IsAbstract &&
                    !p.IsGenericType &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(p) &&
                    Attribute.IsDefined(p, typeof(SerializableAttribute)))
                .ToList();
        }
    }
}